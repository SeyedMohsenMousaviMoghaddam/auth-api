using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Identity.Api.Util;
using Service.Identity.Application.Common;
using Service.Identity.Application.Constants;
using Service.Identity.Application.Policies.Contracts;
using Service.Identity.Application.RolePolicies.Contracts;
using Service.Identity.Application.UserPolcies.Contracts;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Api.Controllers;

[Route("api/policies")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemaConsts.DefaultSchema)]
public class PolicyController : ControllerBase
{
    private readonly IMediator _mediator;

    public PolicyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetPolicies([FromQuery] PolicyGetSortedRequestModel query, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<PolicyGetSortedRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerListAccepted<PoliciesReadResponseModel>, ConsumerRejected>(query, cancellationToken);

        return new GenericResult<ConsumerListAccepted<PoliciesReadResponseModel>>(accepted, rejected);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailablePolicies([FromQuery] PolicyGetAvailableRequestModel query, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<PolicyGetAvailableRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<UserPolicyReadResponseModel>, ConsumerRejected>(query, cancellationToken);

        return new GenericResult<ConsumerAccepted<UserPolicyReadResponseModel>>(accepted, rejected);
    }

    [HttpGet("role/{roleId}")]
    public async Task<IActionResult> GetRolePolicies(long roleId, CancellationToken cancellationToken)
    {
        var requestClient = _mediator.CreateRequestClient<RolePolicyGetRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerListAccepted<PolicyResponseModel>, ConsumerRejected>(new RolePolicyGetRequestModel { RoleId = roleId }, cancellationToken);

        return new GenericResult<ConsumerListAccepted<PolicyResponseModel>>(accepted, rejected);
    }

    [HttpPost("role/bulk/{roleId}")]
    public async Task<IActionResult> CreateRolePolicies([FromRoute] long roleId, [FromBody] long[] policiesId, CancellationToken cancellationToken)
    {
        var command = new RolePolicyCreateRequestModel()
        {
            RoleId = roleId,
            PoliciesIds = policiesId
        };

        var requestClient = _mediator.CreateRequestClient<RolePolicyCreateRequestModel>();

        var (accepted, rejected) = await requestClient.GetResponse<ConsumerAccepted<RolePolicyCreateResponseModel>, ConsumerRejected>(command, cancellationToken);

        return new GenericResult<ConsumerAccepted<RolePolicyCreateResponseModel>>(accepted, rejected);
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedBulkPolicies(CancellationToken cancellationToken)
    {
        var command = FileExtenion.DeserializeJsonFromFile<PolicyCreateBulkRequestModel>(@"wwwroot/Seed/Policies.json");

        await _mediator.Send(command, cancellationToken);

        return Ok(new
        {
            Success = true
        });
    }
}