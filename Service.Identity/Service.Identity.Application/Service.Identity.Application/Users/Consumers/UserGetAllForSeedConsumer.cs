using AutoMapper;
using Service.Identity.Application.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.Users.Consumers;

public class UserGetAllForSeedConsumer : IConsumer<UserGetAllForSeedRequestModel>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UserGetAllForSeedConsumer(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UserGetAllForSeedRequestModel> context)
    {
        var cancellationToken = context.CancellationToken;
        var result = await _unitOfWork.Users.TableNoTracking.ToListAsync(cancellationToken);
        var mappedResult = _mapper.Map<List<UserSeedResponseModel>>(result);

        await context.RespondAsync<ConsumerListAccepted<UserSeedResponseModel>>(new
        {
            Data = mappedResult
        });
    }
}