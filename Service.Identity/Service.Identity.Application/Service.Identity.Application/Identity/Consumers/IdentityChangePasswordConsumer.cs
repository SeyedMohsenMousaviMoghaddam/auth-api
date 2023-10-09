using MassTransit;
using Microsoft.AspNetCore.Identity;
using Service.Identity.Application.Common;
using Service.Identity.Application.Identity.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Users;

namespace Service.Identity.Application.Identity.Consumers;

public class IdentityChangePasswordConsumer : IConsumer<IdentityChangePasswordRequestModel>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserInfo _userInfo;

    public IdentityChangePasswordConsumer(UserManager<User> userManager, IUserInfo userInfo)
    {
        _userManager = userManager;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<IdentityChangePasswordRequestModel> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;

        var user = await _userManager.FindByIdAsync(_userInfo.UserId.Value.ToString());
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

        if (result.Succeeded)
        {
            await context.RespondAsync<ConsumerAccepted<IdentityChangePasswordResponseModel>>(new
            {
                Data = default(IdentityChangePasswordResponseModel),
                StatusCode = ConsumerStatusCode.Success,
                Message = "Change password successfully!"
            });
        }
        else
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Errors = new[]
                    {
                        string.Join(" | ", result.Errors.Select(x => x.Description))
                    }
            });
        }
    }
}