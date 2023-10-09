using AutoMapper;
using Service.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Identity;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Domain.Users;
using Service.Identity.Domain.Common;

namespace Service.Identity.Application.Users.Consumers;

public class UserCreateConsumer : IConsumer<UserCreateRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly IUserInfo _userInfo;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UserCreateConsumer(IUnitOfWork unitOfWork, UserManager<User> userManager,
        IUserInfo userInfo, IMediator mediator, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _userInfo = userInfo;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<UserCreateRequestModel> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;

        var isDuplicated =
            await _unitOfWork.Users.IsDuplicated(request.PhoneNumber, request.NationalCode, cancellationToken);
        if (isDuplicated)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.Conflict,
                Errors = new[]
                {
                    ConsumerMessage.DUPLICATED("User")
                }
            });
            return;
        }

        var newUser = new User
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.NationalCode,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumberConfirmed = false,
            CreatedById = _userInfo.UserId,
            CreatedDate = DateTime.Now
        };


        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (result.Succeeded)
        {

            var mappedResult = _mapper.Map<UserCreateResponseModel>(newUser);

            

            await context.RespondAsync<ConsumerAccepted<UserCreateResponseModel>>(new
            {
                Data = mappedResult,
                Message = ConsumerMessage.CREATE_SUCCESSFULLY("User"),
                StatusCode = ConsumerStatusCode.Success
            });
        }
        else
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Errors = new[]
                {
                    result.Errors.FirstOrDefault()?.Code
                }
            });
        }
    }
}