using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.Users.Consumers;

public class UserGetPaginatedConsumer : IConsumer<UserGetPaginatedRequestModel>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public UserGetPaginatedConsumer(IMapper mapper, IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<UserGetPaginatedRequestModel> context)
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

            // admin
            var result = _unitOfWork.Users.TableNoTracking.ExcludeSoftDelete()
                                                              .Include(x => x.CreatedUser)
                                                              .Include(x => x.ModifiedUser)
                                                              .Where(x => user.Grade.Contains(x.Grade))
                                                              .WithPaging(request.Search, request.Page, request.RowsPerPage, request.SortBy, request.Descending);

                var listResult = await result.Data.ToListAsync(cancellationToken);
                var mappedResult = _mapper.Map<List<UserReadResponseModel>>(listResult);

                await context.RespondAsync<ConsumerPaginatedListAccepted<UserReadResponseModel>>(new
                {
                    Data = mappedResult,
                    result.Pagination,
                    StatusCode = ConsumerStatusCode.Success,
                    Message = ConsumerMessage.GET_PAGINATED_SUCCESSFULLY("User")
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