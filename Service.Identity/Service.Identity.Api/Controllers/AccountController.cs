
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Identity.Api.Util;
using Service.Identity.Application.Common;
using Service.Identity.Application.Constants;
using Service.Identity.Application.Identity.Contracts;
using System.Net;
using static MassTransit.ValidationResultExtensions;

namespace Service.Identity.Api.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ConsumerAccepted<IdentityTokenResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody] IdentityLoginRequestModel model,
        CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<IdentityLoginRequestModel>();

        var (accepted, rejected) =
            await requestClient.GetResponse<ConsumerAccepted<IdentityTokenResponseModel>, ConsumerRejected>(model,
                cancellationToken);

        return new GenericResult<ConsumerAccepted<IdentityTokenResponseModel>>(accepted, rejected);
    }

    [HttpPost("verify-login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ConsumerAccepted<IdentityTokenResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> VerifyLogin([FromBody] IdentityVerifyLoginRequestModel command,
        CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<IdentityVerifyLoginRequestModel>();

        var (accepted, rejected) =
            await requestClient.GetResponse<ConsumerAccepted<IdentityTokenResponseModel>, ConsumerRejected>(
                command, cancellationToken);

        return new GenericResult<ConsumerAccepted<IdentityTokenResponseModel>>(accepted, rejected);
    }

    [HttpPost("login-submit")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemaConsts.TemporarySchema)]
    [ProducesResponseType(typeof(ConsumerAccepted<IdentityTokenResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SetTenantAndBranch([FromBody] IdentitySubmitLoginRequestModel model,
        CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<IdentitySubmitLoginRequestModel>();

        var (accepted, rejected) =
            await requestClient.GetResponse<ConsumerAccepted<IdentityTokenResponseModel>, ConsumerRejected>(model,
                cancellationToken);

        return new GenericResult<ConsumerAccepted<IdentityTokenResponseModel>>(accepted, rejected);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ConsumerAccepted<IdentityTokenResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RefreshToken([FromBody] IdentityRefreshTokenRequest model,
        CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<IdentityRefreshTokenRequest>();

        var (accepted, rejected) =
            await requestClient.GetResponse<ConsumerAccepted<IdentityTokenResponseModel>, ConsumerRejected>(model,
                cancellationToken);

        return new GenericResult<ConsumerAccepted<IdentityTokenResponseModel>>(accepted, rejected);
    }

    [HttpPost("change-password")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemaConsts.DefaultSchema)]
    [ProducesResponseType(typeof(ConsumerAccepted<IdentityChangePasswordResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangePassword([FromBody] IdentityChangePasswordRequestModel command,
        CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<IdentityChangePasswordRequestModel>();

        var (accepted, rejected) =
            await requestClient.GetResponse<ConsumerAccepted<IdentityChangePasswordResponseModel>, ConsumerRejected>(
                command, cancellationToken);

        return new GenericResult<ConsumerAccepted<IdentityChangePasswordResponseModel>>(accepted, rejected);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ConsumerAccepted<IdentityForgotPasswordResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] IdentityForgotPasswordRequestModel command,
        CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<IdentityForgotPasswordRequestModel>();

        var (accepted, rejected) =
            await requestClient.GetResponse<ConsumerAccepted<IdentityForgotPasswordResponseModel>, ConsumerRejected>(
                command, cancellationToken);

        return new GenericResult<ConsumerAccepted<IdentityForgotPasswordResponseModel>>(accepted, rejected);
    }

    [HttpPost("verify-forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ConsumerAccepted<IdentityVerifyForgotPasswordResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> VerifyForgotPassword([FromBody] IdentityVerifyForgotPasswordRequestModel command,
        CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<IdentityVerifyForgotPasswordRequestModel>();

        var (accepted, rejected) =
            await requestClient.GetResponse<ConsumerAccepted<IdentityVerifyForgotPasswordResponseModel>, ConsumerRejected>(
                command, cancellationToken);

        return new GenericResult<ConsumerAccepted<IdentityVerifyForgotPasswordResponseModel>>(accepted, rejected);
    }
}