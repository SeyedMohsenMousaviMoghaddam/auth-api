using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Policies.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.Policies.Consumers;

public class PolicyGetConsumer : IConsumer<PolicyGetRequestModel>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public PolicyGetConsumer(IMapper mapper, IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<PolicyGetRequestModel> context)
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

        var result = await _unitOfWork.Policies.TableNoTracking.Where(x => grades.Any(grade => grade.Equals(x.Grade.ToString())))
                                                               .FirstOrDefaultAsync(c => c.Id.Equals(request.Id), cancellationToken);

        if (result != null)
        {
            var mappedResult = _mapper.Map<PolicyResponseModel>(result);

            await context.RespondAsync<ConsumerAccepted<PolicyResponseModel>>(new
            {
                Data = mappedResult,
                StatusCode = ConsumerStatusCode.Success,
                Message = ConsumerMessage.GET_SUCCESSFULLY("Policy")
            });
        }
        else
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                Errors = new[]
                {
                    ConsumerMessage.NOTFOUND("Policy")
                },
                StatusCode = ConsumerStatusCode.NotFound
            });
        }
    }
}