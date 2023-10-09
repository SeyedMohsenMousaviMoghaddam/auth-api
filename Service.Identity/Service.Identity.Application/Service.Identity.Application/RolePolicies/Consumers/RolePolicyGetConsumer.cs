using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Policies.Contracts;
using Service.Identity.Application.RolePolicies.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.RolePolicies.Consumers;

public class RolePolicyGetConsumer : IConsumer<RolePolicyGetRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserInfo _userInfo;

    public RolePolicyGetConsumer(IUnitOfWork unitOfWork, IMapper mapper, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<RolePolicyGetRequestModel> context)
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

        var result = _unitOfWork.RolePolicies.TableNoTracking.Include(x => x.Policy)
                                                             .Include(x => x.Role)
                                                             .Where(x => grades.Any(grade => grade.Equals(x.Policy.Grade.ToString())))
                                                             .Where(x => grades.Any(grade => grade.Equals(x.Role.Grade.ToString())))
                                                             .Where(x => x.RoleId.Equals(request.RoleId))
                                                             .Select(p => p.Policy);

        var listResult = await result.ToListAsync(cancellationToken);
        var mappedResult = _mapper.Map<List<PolicyResponseModel>>(listResult);

        await context.RespondAsync<ConsumerListAccepted<PolicyResponseModel>>(new
        {
            Data = mappedResult,
            StatusCode = ConsumerStatusCode.Success,
            Message = ConsumerMessage.GET_SUCCESSFULLY("RolePolicy")
        });
    }
}