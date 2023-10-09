using Service.Identity.Application.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.UserPolcies.Contracts;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.UserPolcies.Consumers;

public class UserPolicyDeleteConsumer : IConsumer<UserPolicyDeleteRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public UserPolicyDeleteConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UserPolicyDeleteRequestModel> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;

        var user = await _unitOfWork.Users.Table.FirstOrDefaultAsync(x => x.Id.Equals(request.UserId), cancellationToken);
        if (user is null)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Errors = new[]
                {
                    ConsumerMessage.NOTFOUND("User")
                }
            });
            return;
        }

        user.RemoveUserPolicy(request.PolicyId, request.TenantId);

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await context.ConsumeCompleted;
    }
}