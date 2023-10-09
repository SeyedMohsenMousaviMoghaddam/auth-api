using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.Users.Consumers;

public class UserUpdateConsumer : IConsumer<UserUpdateRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;
    private readonly IMapper _mapper;

    public UserUpdateConsumer(IUnitOfWork unitOfWork, IUserInfo userInfo, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<UserUpdateRequestModel> context)
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

            var updatedModel = _mapper.Map(request, modelForUpdate);

            await _unitOfWork.Users.UpdateAsync(updatedModel, cancellationToken);

            await context.RespondAsync<ConsumerAccepted<UserUpdateResponseModel>>(new
            {
                Data = new UserUpdateResponseModel { Id = request.Id },
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