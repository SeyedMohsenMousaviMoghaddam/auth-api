using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Policies.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Domain.Policies;

namespace Service.Identity.Application.Policies.Consumers;

public class PolicyCreateBulkConsumer : IConsumer<PolicyCreateBulkRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public PolicyCreateBulkConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<PolicyCreateBulkRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;

            var policies = new List<Policy>();
            foreach (var createApiScopePolicyDto in request.ApiScopes)
            {
                policies.AddRange(createApiScopePolicyDto.Policies.Select(item => new Policy
                {
                    ApiResource = request.ApiResource,
                    ApiScope = createApiScopePolicyDto.ApiScope,
                    Value = item.Policy,
                    Order = item.Order,
                    Grade = item.Grade.Value
                }));
            }

            var apiResourcePolicies = await _unitOfWork.Policies.TableNoTracking.Where(x => x.ApiResource.Equals(request.ApiResource))
                .ToListAsync(cancellationToken);

            var policiesShouldAdd = policies.Where(x => !apiResourcePolicies.Any(p => x.ApiResource.Equals(p.ApiResource) && x.ApiScope.Equals(p.ApiScope) && x.Value.Equals(p.Value))).ToList();
            var policiesShouldRemove = apiResourcePolicies.Where(x => !policies.Any(p => x.ApiResource.Equals(p.ApiResource) && x.ApiScope.Equals(p.ApiScope) && x.Value.Equals(p.Value))).ToList();

            if (policiesShouldAdd.Any())
                await _unitOfWork.Policies.AddRangeAsync(policiesShouldAdd, cancellationToken);


            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}