using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.RolePolicies.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Domain.RolePolicies;
using Service.Identity.Domain.Roles;

namespace Service.Identity.Application.RolePolicies.Consumers;

public class RolePolicyCreateConsumer : IConsumer<RolePolicyCreateRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public RolePolicyCreateConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<RolePolicyCreateRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;

            var role = await _unitOfWork.Roles.Table.FirstOrDefaultAsync(x => x.Id.Equals(request.RoleId), cancellationToken);
            if (role is null)
            {
                await context.RespondAsync<ConsumerRejected>(new
                {
                    StatusCode = ConsumerStatusCode.NotFound,
                    Errors = new[]
                    {
                        ConsumerMessage.NOTFOUND("Role")
                    }
                });
                return;
            }

            var policies = request.PoliciesIds.ToList();
            var rolePolicies = await _unitOfWork.RolePolicies.TableNoTracking.Where(x => x.RoleId.Equals(role.Id))
                                                                             .ToListAsync(cancellationToken);
            var policiesShouldRemove = rolePolicies.Where(x => policies.All(y => !x.PolicyId.Equals(y))).ToList();
            var policiesShouldAssign = policies.Where(x => rolePolicies.All(y => !x.Equals(y.PolicyId))).ToList();

            if (policiesShouldRemove.Any())
                await _unitOfWork.RolePolicies.DeleteRangeAsync(policiesShouldRemove, cancellationToken);

            if (policiesShouldAssign.Any())
            {
                var list = new List<RolePolicy>();
                policiesShouldAssign.ForEach(policy => list.Add(new RolePolicy { RoleId = role.Id, PolicyId = policy }));
                await _unitOfWork.RolePolicies.AddRangeAsync(list, cancellationToken);
            }

            await context.RespondAsync<ConsumerAccepted<RolePolicyCreateResponseModel>>(new
            {
                Data = default(RolePolicyCreateResponseModel),
                StatusCode = ConsumerStatusCode.Success
            });
        }
        catch (Exception ex)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Reason = ex.InnerException != null ? ex.InnerException.Message : ex.Message
            });
        }
    }
}