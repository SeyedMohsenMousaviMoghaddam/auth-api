using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Policies.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.Policies.Consumers;

public class PolicyGetAllConsumer : IConsumer<PolicyGetAllRequestModel>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public PolicyGetAllConsumer(IMapper mapper, IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<PolicyGetAllRequestModel> context)
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

        var grades = user.Grade.Replace(" ", "").Split(",");

        var result = _unitOfWork.Policies.TableNoTracking.Where(x => grades.Any(grade => grade.Equals(x.Grade.ToString())))
                                                         .WithPaging(request.Search, request.Page, request.RowsPerPage, request.SortBy, request.Descending);

        var listResult = await result.Data.ToListAsync(cancellationToken);
        var mappedResult = _mapper.Map<List<PolicyResponseModel>>(listResult);

        await context.RespondAsync<ConsumerPaginatedListAccepted<PolicyResponseModel>>(new
        {
            Data = mappedResult,
            result.Pagination
        });
    }
}