using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Identity.Api.Util;
using Service.Identity.Application.Common;
using Service.Identity.Application.Constants;
using Service.Identity.Application.UserPolcies.Contracts;
using Service.Identity.Application.UserRoles.Contracts;
using Service.Identity.Application.Users.Contracts;
using Service.Pharmacy.Api.Filters;
using System.Net;

namespace PIS.Identity.Api.Controllers;

[Route("api/users")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemaConsts.DefaultSchema)]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [Policy(new[] { UserPolicy.READ })]
    [ProducesResponseType(typeof(ConsumerAccepted<UserReadResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<UserGetRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<UserReadResponseModel>, ConsumerRejected>(new UserGetRequestModel { Id = id }, cancellationToken);

        return new GenericResult<ConsumerAccepted<UserReadResponseModel>>(accepted, rejected);
    }
    
    [HttpGet("is-exist-user-by-nationalcode")]
    [Policy(new[] { UserPolicy.READ })]
    [ProducesResponseType(typeof(ConsumerAccepted<UserIsExistByNationalCodeResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> IsExistUserByNationalCode([FromQuery] UserIsExistByNationalCodeRequestModel command, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<UserIsExistByNationalCodeRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<UserIsExistByNationalCodeResponseModel>, ConsumerRejected>(command, cancellationToken);

        return new GenericResult<ConsumerAccepted<UserIsExistByNationalCodeResponseModel>>(accepted, rejected);
    }

    [HttpPost("paginated")]
    [Policy(new[] { UserPolicy.READ })]
    [ProducesResponseType(typeof(ConsumerPaginatedListAccepted<UserReadResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get(UserGetPaginatedRequestModel query, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<UserGetPaginatedRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerPaginatedListAccepted<UserReadResponseModel>, ConsumerRejected>(query, cancellationToken);

        return new GenericResult<ConsumerPaginatedListAccepted<UserReadResponseModel>>(accepted, rejected);
    }
    
    [HttpPost("user-has-policy")]
    [ProducesResponseType(typeof(UserHasPolicyResponseModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UserHasPolicy(UserHasPolicyRequestModel query, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<UserHasPolicyRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<UserHasPolicyResponseModel, ConsumerRejected>(query, cancellationToken);

        return new GenericResult<UserHasPolicyResponseModel>(accepted, rejected);
    }

    [HttpGet("search")]
    [Policy(new[] { UserPolicy.READ })]
    [ProducesResponseType(typeof(ConsumerListAccepted<UserReadResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Search([FromQuery] UserSearchRequestModel model, CancellationToken cancellationToken)
    {
        var request = _mediator.CreateRequestClient<UserSearchRequestModel>();

        var (accepted, rejected) = await request.GetResponse<ConsumerListAccepted<UserReadResponseModel>, ConsumerRejected>(model, cancellationToken);

        return new GenericResult<ConsumerListAccepted<UserReadResponseModel>>(accepted, rejected);
    }

    [HttpPost]
    [Policy(new[] { UserPolicy.CREATE })]
    [ProducesResponseType(typeof(ConsumerAccepted<UserCreateResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Post([FromBody] UserCreateRequestModel command, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<UserCreateRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<UserCreateResponseModel>, ConsumerRejected>(command, cancellationToken);

        return new GenericResult<ConsumerAccepted<UserCreateResponseModel>>(accepted, rejected);
    }

    [HttpPost("roles")]
    [Policy(new[] { UserPolicy.CREATE })]
    [ProducesResponseType(typeof(ConsumerAccepted<UserRoleCreateResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostUserRoles([FromBody] UserRoleCreateRequestModel command, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<UserRoleCreateRequestModel>();
        
        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<UserRoleCreateResponseModel>, ConsumerRejected>(command, cancellationToken);
        
        return new GenericResult<ConsumerAccepted<UserRoleCreateResponseModel>>(accepted, rejected);
    }

    [HttpPut]
    [Policy(new[] { UserPolicy.UPDATE })]
    [ProducesResponseType(typeof(ConsumerAccepted<UserUpdateResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Put([FromBody] UserUpdateRequestModel command, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<UserUpdateRequestModel>();
        
        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<UserUpdateResponseModel>, ConsumerRejected>(command, cancellationToken);

        return new GenericResult<ConsumerAccepted<UserUpdateResponseModel>>(accepted, rejected);
    }

    [HttpDelete("{id}")]
    [Policy(new[] { UserPolicy.DELETE })]
    [ProducesResponseType(typeof(ConsumerAccepted<UserDeleteResponseModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<UserDeleteRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<UserDeleteResponseModel>, ConsumerRejected>(new UserDeleteRequestModel { UserId = id }, cancellationToken);

        return new GenericResult<ConsumerAccepted<UserDeleteResponseModel>>(accepted, rejected);
    }

    [HttpDelete("roles")]
    [Policy(new[] { UserPolicy.DELETE })]
    [ProducesResponseType(typeof(UserRoleDeleteRequestModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteUserRoles([FromBody] UserRoleDeleteRequestModel command, CancellationToken cancellationToken)
    {
        await _mediator.Send<UserRoleDeleteRequestModel>(command, cancellationToken);
        return Ok();
    }
}