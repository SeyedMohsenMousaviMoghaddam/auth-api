using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.RolePolicies.Contracts;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.RolePolicies.Consumers;

public class RolePolicyDeleteConsumer : IConsumer<RolePolicyDeleteRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public RolePolicyDeleteConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<RolePolicyDeleteRequestModel> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        var role = await _unitOfWork.Roles.Table.Include(x => x.RolePolicies).FirstOrDefaultAsync(x => x.Id.Equals(message.RoleId), cancellationToken);

        if (role != null)
        {
            var policies = message.PoliciesIds.ToList();
            policies.ForEach(policyId => role.RemoveRolePolicy(policyId));
            await _unitOfWork.Roles.UpdateAsync(role, cancellationToken);
        }

        await Task.CompletedTask;
    }
}