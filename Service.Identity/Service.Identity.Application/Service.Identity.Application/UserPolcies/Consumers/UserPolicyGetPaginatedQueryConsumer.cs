using AutoMapper;
using AutoMapper.QueryableExtensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Policies.Contracts;
using Service.Identity.Application.UserPolcies.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.UserPolcies.Consumers;

public class UserPolicyGetPaginatedQueryConsumer : IConsumer<UserPolicyGetPaginatedRequestModel>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UserPolicyGetPaginatedQueryConsumer(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UserPolicyGetPaginatedRequestModel> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;

        var result = _unitOfWork.Users.TableNoTracking.ExcludeSoftDelete()
                                                      .Include(x => x.UserPolicies.Where(p => p.TenantId.Equals(request.TenantId) && x.Grade.Contains(p.Policy.Grade.ToString())))
                                                      .ThenInclude(x => x.Policy)
                                                      .Where(x => x.Id.Equals(request.UserId))
                                                      .SelectMany(x => x.UserPolicies.Select(p => p.Policy))
                                                      .WithPaging(request.Search, request.Page, request.RowsPerPage, request.SortBy, request.Descending);

        var listResult = await result.Data.ProjectTo<PolicyResponseModel>(_mapper.ConfigurationProvider)
                                          .ToListAsync(cancellationToken);

        await context.RespondAsync<ConsumerPaginatedListAccepted<PolicyResponseModel>>(new
        {
            Data = listResult,
            result.Pagination
        });
    }
}