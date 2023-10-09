using Service.Identity.Application.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Roles.Contracts;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.Roles.Consumers;

public class RoleDeleteConsumer : IConsumer<RoleDeleteRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleDeleteConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<RoleDeleteRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;

            var modelToDelete = await _unitOfWork.Roles.Table.FirstOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken);
            if (modelToDelete is null)
            {
                await context.RespondAsync<ConsumerRejected>(new
                {
                    StatusCode = ConsumerStatusCode.NotFound,
                    Errors = new[]
                    {
                        ConsumerMessage.NOTFOUND("Role")
                    }
                });
                return;
            }

            await _unitOfWork.Roles.DeleteAsync(modelToDelete, cancellationToken);
            await context.RespondAsync<ConsumerAccepted<RoleDeleteResponseModel>>(new
            {
                Data = default(RoleDeleteResponseModel),
                Message = ConsumerMessage.DELETE_SUCCESSFULLY("Role"),
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