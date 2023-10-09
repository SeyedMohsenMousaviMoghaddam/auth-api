using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Identity.Api.Util;
using Service.Identity.Application.Common;
using Service.Identity.Application.Constants;
using Service.Identity.Application.Roles.Contracts;
using Service.Pharmacy.Api.Filters;

namespace Service.Identity.Api.Controllers;

[Route("api/roles")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemaConsts.DefaultSchema)]
public class RoleController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [Policy(new[] { RolePolicy.READ })]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<RoleGetRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<RoleReadResponseModel>, ConsumerRejected>(new RoleGetRequestModel { Id = id }, cancellationToken);

        return new GenericResult<ConsumerAccepted<RoleReadResponseModel>>(accepted, rejected);
    }

    [HttpPost("paginated")]
    [Policy(new[] { RolePolicy.READ })]
    public async Task<IActionResult> Get(RoleGetAllRequestModel query, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<RoleGetAllRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerPaginatedListAccepted<RoleReadResponseModel>, ConsumerRejected>(query, cancellationToken);

        return new GenericResult<ConsumerPaginatedListAccepted<RoleReadResponseModel>>(accepted, rejected);
    }

    [HttpGet("get-available-roles")]
    [Policy(new[] { RolePolicy.READ })]
    public async Task<IActionResult> GetAvailableRoles([FromQuery] RoleGetAvailableRequestModel command, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<RoleGetAvailableRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerListAccepted<RoleReadResponseModel>, ConsumerRejected>(command, cancellationToken);

        return new GenericResult<ConsumerListAccepted<RoleReadResponseModel>>(accepted, rejected);
    }

    [HttpPost]
    [Policy(new[] { RolePolicy.CREATE })]
    public async Task<IActionResult> Post([FromBody] RoleCreateRequestModel command, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<RoleCreateRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<RoleCreateResponseModel>, ConsumerRejected>(command, cancellationToken);

        return new GenericResult<ConsumerAccepted<RoleCreateResponseModel>>(accepted, rejected);
    }

    [HttpPut]
    [Policy(new[] { RolePolicy.UPDATE })]
    public async Task<IActionResult> Put([FromBody] RoleUpdateRequestModel command, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<RoleUpdateRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<RoleUpdateResponseModel>, ConsumerRejected>(command, cancellationToken);

        return new GenericResult<ConsumerAccepted<RoleUpdateResponseModel>>(accepted, rejected);
    }

    [HttpDelete("{id}")]
    [Policy(new[] { RolePolicy.DELETE })]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<RoleDeleteRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<RoleDeleteResponseModel>, ConsumerRejected>(new RoleDeleteRequestModel { Id = id }, cancellationToken);

        return new GenericResult<ConsumerAccepted<RoleDeleteResponseModel>>(accepted, rejected);
    }
}