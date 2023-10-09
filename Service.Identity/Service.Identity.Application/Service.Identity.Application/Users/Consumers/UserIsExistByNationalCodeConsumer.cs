using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.Users.Consumers;

public class UserIsExistByNationalCodeConsumer : IConsumer<UserIsExistByNationalCodeRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public UserIsExistByNationalCodeConsumer(IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<UserIsExistByNationalCodeRequestModel> context)
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
                    StatusCode = ConsumerStatusCode.UnAuthorized,
                    Errors = new[]
                    {
                        ConsumerMessage.NOTFOUND("User")
                    }
                });
                return;
            }

            var result = await _unitOfWork.Users.TableNoTracking.ExcludeSoftDelete()
                                                                .Where(x => user.Grade.Contains(x.Grade))
                                                                .FirstOrDefaultAsync(x => x.UserName.Equals(request.NationalCode), cancellationToken);
            if (result is null)
            {
                await context.RespondAsync<ConsumerAccepted<UserIsExistByNationalCodeResponseModel>>(new
                {
                    Data = default(UserIsExistByNationalCodeResponseModel),
                    StatusCode = ConsumerStatusCode.Success,
                    Message = ConsumerMessage.NOTFOUND("User")
                });
                return;
            }

            await context.RespondAsync<ConsumerAccepted<UserIsExistByNationalCodeResponseModel>>(new
            {
                Data = new UserIsExistByNationalCodeResponseModel { Id = result.Id },
                StatusCode = ConsumerStatusCode.Success,
                Message = ConsumerMessage.GET_SUCCESSFULLY("User")
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