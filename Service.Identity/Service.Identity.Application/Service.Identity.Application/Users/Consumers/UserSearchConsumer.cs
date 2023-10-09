using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.Users.Consumers;

public class UserSearchConsumer : IConsumer<UserSearchRequestModel>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public UserSearchConsumer(IMapper mapper, IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<UserSearchRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;
            var filter = request.Filter;

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

            // admin
                var result = await _unitOfWork.Users.TableNoTracking.ExcludeSoftDelete()
                                                                    .Where(x => filter == null || 
                                                                                x.FirstName.ToLower().Contains(filter.ToLower()) ||
                                                                                x.LastName.ToLower().Contains(filter.ToLower()) ||
                                                                                x.UserName.ToLower().Contains(filter.ToLower()) ||
                                                                                x.Email.ToLower().Contains(filter.ToLower()) ||
                                                                                x.FullName.ToLower().Contains(filter.ToLower()))
                                                                    .Where(x => user.Grade.Contains(x.Grade))
                                                                    .Take(request.Top)
                                                                    .ToListAsync(cancellationToken);

                var mappedResult = _mapper.Map<List<UserReadResponseModel>>(result);

                await context.RespondAsync<ConsumerListAccepted<UserReadResponseModel>>(new
                {
                    Data = mappedResult,
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