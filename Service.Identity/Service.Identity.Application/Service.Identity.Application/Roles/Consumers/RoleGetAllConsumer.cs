using AutoMapper;
using Service.Identity.Application.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Roles.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Domain.Common;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.Roles.Consumers;

public class RoleGetAllConsumer : IConsumer<RoleGetAllRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserInfo _userInfo;

    public RoleGetAllConsumer(IUnitOfWork unitOfWork, IMapper mapper, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<RoleGetAllRequestModel> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;

        var user = await _unitOfWork.Users.TableNoTracking.FirstOrDefaultAsync(x => x.Id.Equals(_userInfo.UserId));
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

        var grades = user.Grade.Replace(" ", "").Split(",");

        var result = _unitOfWork.Roles.TableNoTracking.Where(x => grades.Any(grade => grade.Equals(x.Grade.ToString())))
                                                      .Include(x => x.CreatedUser)
                                                      .Include(x => x.ModifiedUser)
                                                      .WithPaging(request.Search, request.Page, request.RowsPerPage, request.SortBy, request.Descending);

        var listResult = await result.Data.ToListAsync(cancellationToken);
        var mappedResult = _mapper.Map<List<RoleReadResponseModel>>(listResult);

        await context.RespondAsync<ConsumerPaginatedListAccepted<RoleReadResponseModel>>(new
        {
            Data = mappedResult,
            result.Pagination,
            StatusCode = ConsumerStatusCode.Success,
            Message = ConsumerMessage.GET_PAGINATED_SUCCESSFULLY("Role")
        });
    }
}