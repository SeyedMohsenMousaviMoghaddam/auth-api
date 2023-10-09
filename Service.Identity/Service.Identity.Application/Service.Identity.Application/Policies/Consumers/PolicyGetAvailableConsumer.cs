using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Constants;
using Service.Identity.Application.Policies.Contracts;
using Service.Identity.Application.UserPolcies.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.Policies.Consumers;

public class PolicyGetAvailableConsumer : IConsumer<PolicyGetAvailableRequestModel>
{
    private readonly IUserInfo _userInfo;
    private readonly IUnitOfWork _unitOfWork;

    public PolicyGetAvailableConsumer(IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<PolicyGetAvailableRequestModel> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;
        var roleIds = new long[] { };

        var user = await _unitOfWork.Users.TableNoTracking.ExcludeSoftDelete()
                                                          .FirstOrDefaultAsync(x => x.Id.Equals(_userInfo.UserId) && x.IsActive.Value);
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


        // end user
            roleIds = _unitOfWork.UserRoles.TableNoTracking.Where(x => x.UserId.Equals(_userInfo.UserId) && !x.TenantId.HasValue)
                                                           .Where(x => grades.Any(g => g.Equals(x.Role.Grade.ToString())))
                                                           .Select(x => x.RoleId)
                                                           .ToArray();
        

        var userPolicies = _unitOfWork.UserPolicies.TableNoTracking.Include(x => x.Policy)
                                                                   .Where(x => x.UserId.Equals(_userInfo.UserId))
                                                                   .Where(x => grades.Any(grade => grade.Equals(x.Policy.Grade.ToString())))
                                                                   .Select(x => x.Policy);

        var rolePolicies = _unitOfWork.RolePolicies.TableNoTracking.Include(x => x.Policy)
                                                                   .Include(x => x.Role)
                                                                   .Where(x => roleIds.Any(y => y.Equals(x.RoleId)))
                                                                   .Where(x => grades.Any(grade => grade.Equals(x.Policy.Grade.ToString())))
                                                                   .Where(x => grades.Any(grade => grade.Equals(x.Role.Grade.ToString())))
                                                                   .Select(x => x.Policy);

        var result = userPolicies.Union(rolePolicies).Select(x => $"{x.ApiResource}.{x.ApiScope}.{x.Value}").ToArray().DistinctBy(policy => policy);
        var policies = new UserPolicyReadResponseModel
        {
            Policies = string.Join(",", result)
        };

        await context.RespondAsync<ConsumerAccepted<UserPolicyReadResponseModel>>(new
        {
            Data = policies,
            StatusCode = ConsumerStatusCode.Success,
            Message = "get_available_policy_successfully"
        });
    }
}