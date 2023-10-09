using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Policies.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.Policies.Consumers;

public class PolicyGetSortedConsumer : IConsumer<PolicyGetSortedRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public PolicyGetSortedConsumer(IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<PolicyGetSortedRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;

            var user = await _unitOfWork.Users.TableNoTracking.FirstOrDefaultAsync(x => x.Id.Equals(_userInfo.UserId), cancellationToken);
            if (user is null)
            {
                await context.RespondAsync<ConsumerRejected>(new 
                {
                    StatusCode = ConsumerStatusCode.NotFound,
                    Errors = new[]
                    {
                        ConsumerMessage.NOTFOUND("User")
                    }
                });
                return;
            }

            var grades = user.Grade.Replace(" ", "").Split(",");

            var query = _unitOfWork.Policies.TableNoTracking.Where(x => grades.Any(grade => grade.Equals(x.Grade.ToString())))
                                                            .Select(x => new PolicyResponseModel
                                                            {
                                                                Id = x.Id,
                                                                ApiResource = x.ApiResource,
                                                                ApiScope = x.ApiScope,
                                                                Value = x.Value,
                                                                Order = x.Order
                                                            });

            var list = query.ToList();

            var result = list.DistinctBy(x => x.ApiResource).Select(x => new PoliciesReadResponseModel
            {
                ApiResource = x.ApiResource,
                ApiScopes = list.Where(y => y.ApiResource.Equals(x.ApiResource)).DistinctBy(y => y.ApiScope).Select(y => new ApiScopesListResponseModel
                {
                    ApiScope = y.ApiScope,
                    Policies = list.Where(z => z.ApiScope.Equals(y.ApiScope) && z.ApiResource.Equals(y.ApiResource)).Select(z => new MinimalPolicyResponseModel
                    {
                        Id = z.Id,
                        Policy = z.Value,
                        Order = z.Order
                    }).OrderBy(h => h.Order).ToList()
                }).ToList()
            }).ToList();

            await context.RespondAsync<ConsumerListAccepted<PoliciesReadResponseModel>>(new
            {
                Data = result,
                StatusCode = ConsumerStatusCode.Success
            });
        }
        catch (Exception ex)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Reason = ex.InnerException != null ? ex.InnerException.Message : ex.Message
            });
        }
    }
}