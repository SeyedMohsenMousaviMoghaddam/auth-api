using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Roles.Contracts;
using Service.Identity.Application.UserRoles.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.UserRoles.Consumers;

public class UserRolesGetPaginatedConsumer : IConsumer<UserRoleGetPaginatedRequestModel>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UserRolesGetPaginatedConsumer(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UserRoleGetPaginatedRequestModel> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;

        var result = _unitOfWork.UserRoles.TableNoTracking.Include(x => x.Role)
                                                          .Include(x => x.User)
                                                          .Where(x => x.UserId.Equals(request.UserId) && x.TenantId.Equals(request.TenantId))
                                                          .Where(x => x.User.Grade.Contains(x.Role.Grade.ToString()))
                                                          .Select(x => x.Role)
                                                          .WithPaging(request.Search, request.Page, request.RowsPerPage, request.SortBy, request.Descending);

        var listResult = await result.Data.ToListAsync(cancellationToken);
        var mappedResult = _mapper.Map<List<RoleReadResponseModel>>(listResult);

        await context.RespondAsync<ConsumerPaginatedListAccepted<RoleReadResponseModel>>(new
        {
            Data = mappedResult,
            result.Pagination
        });
    }
}