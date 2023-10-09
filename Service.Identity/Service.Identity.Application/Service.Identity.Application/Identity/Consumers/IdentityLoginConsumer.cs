using MassTransit;
using Microsoft.AspNetCore.Identity;
using Service.Identity.Application.Common;
using Service.Identity.Application.Configuration.Token;
using Service.Identity.Application.Identity.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Domain.Users;

namespace Service.Identity.Application.Identity.Consumers;

public class IdentityLoginConsumer : IConsumer<IdentityLoginRequestModel>
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ITokenHandler _tokenHandler;
    private readonly IUnitOfWork _unitOfWork;

    public IdentityLoginConsumer(SignInManager<User> signInManager, UserManager<User> userManager, 
        ITokenHandler tokenHandler, IUnitOfWork unitOfWork)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _tokenHandler = tokenHandler;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<IdentityLoginRequestModel> context)
    {
        var request = context.Message;
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user is null || user.IsDeleted.HasValue && user.IsDeleted.Value ||
            user.IsActive.HasValue && !user.IsActive.Value)
        {
            await context.RespondAsync<ConsumerRejected>(new 
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Errors = new[]
                    {
                        "wrong credential"
                    }
            });
            return;
        }

        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, true);

        if (!result.Succeeded)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Errors = new[]
                    {
                        "wrong credential"
                    }
            });
            return;
        }
        else
        {
                var userClaims = _tokenHandler.GenerateClaims(user);
                var token = _tokenHandler.Generate(userClaims, true, false);
                token.TimeBase = DateTime.UtcNow;
                await context.RespondAsync<ConsumerAccepted<IdentityTokenResponseModel>>(new
                {
                    Data = token,
                    StatusCode = ConsumerStatusCode.Success,
                    Message = ConsumerMessage.CREATE_SUCCESSFULLY("Token")
                });          
        }
    }
}