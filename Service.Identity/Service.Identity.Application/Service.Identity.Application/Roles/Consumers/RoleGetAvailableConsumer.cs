using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Roles.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.Roles.Consumers;

public class RoleGetAvailableConsumer : IConsumer<RoleGetAvailableRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserInfo _userInfo;

    public RoleGetAvailableConsumer(IUnitOfWork unitOfWork, IMapper mapper, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<RoleGetAvailableRequestModel> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;

        var user = await _unitOfWork.Users.TableNoTracking.FirstOrDefaultAsync(x => x.Id.Equals(request.UserId), cancellationToken);
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

        var result = _unitOfWork.Roles.TableNoTracking
                                                      .Where(x => grades.Any(grade => grade.Equals(x.Grade.ToString())));
        var listResult = await result.ToListAsync(cancellationToken);
        var mappedResult = _mapper.Map<List<RoleReadResponseModel>>(listResult);

        await context.RespondAsync<ConsumerListAccepted<RoleReadResponseModel>>(new
        {
            Data = mappedResult,
            StatusCode = ConsumerStatusCode.Success
        });
    }
}