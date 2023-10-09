using Service.Identity.Application.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.UserRoles.Contracts;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.UserRoles.Consumers;

public class UserRoleDeleteConsumer : IConsumer<UserRoleDeleteRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public UserRoleDeleteConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UserRoleDeleteRequestModel> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;

        var userRole = await _unitOfWork.UserRoles.Table.FirstOrDefaultAsync(x => x.UserId.Equals(request.UserId) && x.RoleId.Equals(request.RoleId) &&
                                                                                  x.TenantId.Equals(request.TenantId), cancellationToken);

        if (userRole is null)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Errors = new[]
                {
                    ConsumerMessage.NOTFOUND("UserRole")
                }
            });
            return;
        }

        await _unitOfWork.UserRoles.DeleteAsync(userRole, cancellationToken);
        await context.ConsumeCompleted;
    }
}