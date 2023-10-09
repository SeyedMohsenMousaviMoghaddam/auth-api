using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Policies.Contracts;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.Policies.Consumers;

public class PolicyDeleteConsumer : IConsumer<PolicyDeleteRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public PolicyDeleteConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<PolicyDeleteRequestModel> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        var policy = await _unitOfWork.Policies.Table.FirstOrDefaultAsync(x => x.Id == message.PolicyId, cancellationToken);
        if (policy is not null)
            await _unitOfWork.Policies.DeleteAsync(policy, cancellationToken);

        await Task.CompletedTask;
    }
}