using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Constants;
using Service.Identity.Application.UserPolcies.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.UserPolcies.Consumers;

public class UserHasPolicyConsumer : IConsumer<UserHasPolicyRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public UserHasPolicyConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UserHasPolicyRequestModel> context)
    {
        var result = false;
        var request = context.Message;
        var cancellationToken = context.CancellationToken;
        var roleIds = new long[] { };

        var user = await _unitOfWork.Users.TableNoTracking.ExcludeSoftDelete()
            .FirstOrDefaultAsync(x => x.Id.Equals(request.UserId) && x.IsActive.Value, cancellationToken);
        if (user is null)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.NotFound,
                Errors = new[]
                {
                    ConsumerMessage.NOTFOUND("User")
                }
            });
            return;
        }

        var grades = user.Grade.Replace(" ", "").Split(",");
        bool isPharmacy = request.TenantId.HasValue && request.BranchId.HasValue &&
                          grades.Any(grade => grade.Equals(UserTypesConstants.EndUser)) && grades.Count().Equals(1);

        // end user
        if (isPharmacy)
        {
            roleIds = _unitOfWork.UserRoles.TableNoTracking.Where(x => x.UserId.Equals(request.UserId) &&
                                                                       x.TenantId.Value.Equals(request.TenantId.Value))
                .Where(x => grades.Any(g => g.Equals(x.Role.Grade.ToString())))
                .Select(x => x.RoleId)
                .ToArray();
        }
        // backup user and back office and external user
        else
        {
            roleIds = _unitOfWork.UserRoles.TableNoTracking.Where(x => x.UserId.Equals(request.UserId) &&
                                                                       !x.TenantId.HasValue)
                .Where(x => grades.Any(g => g.Equals(x.Role.Grade.ToString())))
                .Select(x => x.RoleId)
                .ToArray();
        }

        var userPolicies = _unitOfWork.UserPolicies.TableNoTracking.Include(x => x.Policy)
            .Where(x => request.TenantId.HasValue &&
                        x.TenantId.Equals(request.TenantId.Value) &&
                        x.UserId.Equals(request.UserId))
            .Where(x => grades.Any(grade => grade.Equals(x.Policy.Grade.ToString())))
            .Select(x => x.Policy);

        var rolePolicies = _unitOfWork.RolePolicies.TableNoTracking.Include(x => x.Policy)
            .Include(x => x.Role)
            .Where(x => roleIds.Any(y => y.Equals(x.RoleId)))
            .Where(x => grades.Any(grade => grade.Equals(x.Policy.Grade.ToString())))
            .Where(x => grades.Any(grade => grade.Equals(x.Role.Grade.ToString())))
            .Select(x => x.Policy);

        var policies = userPolicies.Union(rolePolicies).Select(x => $"{x.ApiResource}.{x.ApiScope}.{x.Value}").ToList();

        if (request.Condition)
            result = request.Policies.All(x => policies.Any(p => p.Equals(x)));

        if (!request.Condition)
            result = request.Policies.Any(x => policies.Any(p => p.Equals(x)));

        if (request.Policies.Any(x => x.Equals(string.Empty)))
            result = true;

        await context.RespondAsync(new UserHasPolicyResponseModel { UserHasPolicy = result });
    }
}