using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.Users.Consumers;

public class UserDeleteConsumer : IConsumer<UserDeleteRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public UserDeleteConsumer(IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<UserDeleteRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;

       

                var modelToDelete = await _unitOfWork.Users.Table.FirstOrDefaultAsync(x => x.Id.Equals(request.UserId), cancellationToken);
                if (modelToDelete is null)
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

                await _unitOfWork.Users.DeleteAsync(modelToDelete, cancellationToken);
            

            await context.RespondAsync<ConsumerAccepted<UserDeleteResponseModel>>(new
            {
                Data = new UserDeleteResponseModel { Id = request.UserId },
                Message = ConsumerMessage.DELETE_SUCCESSFULLY("User"),
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