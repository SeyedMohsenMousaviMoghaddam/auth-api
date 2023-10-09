using Service.Identity.Application.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.UserPolcies.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Domain.UserPolcies;
using Service.Identity.Domain.Common;

namespace Service.Identity.Application.UserPolcies.Consumers;

public class UserPolicyCreateConsumer : IConsumer<UserPolicyCreateRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public UserPolicyCreateConsumer(IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<UserPolicyCreateRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;

            var user = await _unitOfWork.Users.Table.FirstOrDefaultAsync(x => x.Id.Equals(request.UserId), cancellationToken);
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

            var policies = request.PolicyIds.ToList();
            var userPolicies = await _unitOfWork.UserPolicies.TableNoTracking.Where(x => x.UserId.Equals(user.Id))
                                                                             .ToListAsync(cancellationToken);
            var policiesShouldRemove = userPolicies.Where(x => policies.All(y => !x.PolicyId.Equals(y))).ToList();
            var policiesShouldAssign = policies.Where(x => userPolicies.All(y => !x.Equals(y.PolicyId))).ToList();

            if (policiesShouldRemove.Any())
                await _unitOfWork.UserPolicies.DeleteRangeAsync(policiesShouldRemove, cancellationToken);

            if (policiesShouldAssign.Any())
            {
                var list = new List<UserPolicy>();
                policiesShouldAssign.ForEach(policy => list.Add(new UserPolicy { PolicyId = policy, UserId =  user.Id }));
                await _unitOfWork.UserPolicies.AddRangeAsync(list, cancellationToken);
            }

            await Task.CompletedTask;
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