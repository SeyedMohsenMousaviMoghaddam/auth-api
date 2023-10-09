using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.Users.Consumers;

public class UserUpdateTwoFactorEnabledConsumer : IConsumer<UserUpdateTwoFactorEnabledRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;
    private readonly IMapper _mapper;

    public UserUpdateTwoFactorEnabledConsumer(IUnitOfWork unitOfWork,IUserInfo userInfo, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<UserUpdateTwoFactorEnabledRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;

            var modelForUpdate = await _unitOfWork.Users.Table.FirstOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken);
            if (modelForUpdate is null)
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
            modelForUpdate.TwoFactorEnabled = request.TwoFactorEnabled;

            await _unitOfWork.Users.UpdateAsync(modelForUpdate, cancellationToken);

            await context.RespondAsync<ConsumerAccepted<UserUpdateTwoFactorEnabledResponseModel>>(new
            {
                Data = new UserUpdateTwoFactorEnabledResponseModel { Id = request.Id },
                Message = ConsumerMessage.UPDATE_SUCCESSFULLY("User"),
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