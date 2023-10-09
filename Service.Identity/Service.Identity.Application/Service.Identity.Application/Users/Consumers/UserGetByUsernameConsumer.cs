using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.Users.Consumers;

public class UserGetByUsernameConsumer : IConsumer<UserGetByUsernameRequestModel>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public UserGetByUsernameConsumer(IMapper mapper, IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<UserGetByUsernameRequestModel> context)
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

        var result = await _unitOfWork.Users.TableNoTracking.ExcludeSoftDelete()
                                                            .Where(x => user.Grade.Contains(x.Grade))
                                                            .FirstOrDefaultAsync(c => c.UserName.Equals(request.Username), cancellationToken);

        if (result is not null)
        {
            var mappedResult = _mapper.Map<UserReadResponseModel>(result);

            await context.RespondAsync<ConsumerAccepted<UserReadResponseModel>>(new
            {
                Data = mappedResult,
                StatusCode = ConsumerStatusCode.Success,
                Message = ConsumerMessage.GET_SUCCESSFULLY("User")
            });
        }
        else
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                Errors = new[]
                {
                    ConsumerMessage.NOTFOUND("User")
                },
                StatusCode = ConsumerStatusCode.NotFound
            });
        }
    }
}