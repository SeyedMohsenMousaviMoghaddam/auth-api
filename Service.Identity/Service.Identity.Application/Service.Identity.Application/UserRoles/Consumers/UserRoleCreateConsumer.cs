using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.UserRoles.Contracts;
using Service.Identity.Domain;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;
using Service.Identity.Domain.UserRoles;

namespace Service.Identity.Application.UserRoles.Consumers;

public class UserRoleCreateConsumer : IConsumer<UserRoleCreateRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public UserRoleCreateConsumer(IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<UserRoleCreateRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;

            var user = await _unitOfWork.Users.TableNoTracking.FirstOrDefaultAsync(x => x.Id.Equals(request.UserId), cancellationToken);
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

            var roles = request.RoleIds;
            var userRoles = await _unitOfWork.UserRoles.TableNoTracking.Where(x => x.UserId.Equals(user.Id))
                                                                       .ToListAsync(cancellationToken);

            var rolesShouldRemove = userRoles.Where(x => roles.All(y => !x.RoleId.Equals(y))).ToList();
            var rolesShouldAssign = roles.Where(x => userRoles.All(y => !x.Equals(y.RoleId))).ToList();

            if (rolesShouldRemove.Any())
                await _unitOfWork.UserRoles.DeleteRangeAsync(rolesShouldRemove, cancellationToken);

            if (rolesShouldAssign.Any())
            {
                var list = new List<UserRole>();
                rolesShouldAssign.ForEach(roleId => list.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId,
                    CreatedById = _userInfo.UserId.Value,
                    CreatedDate = DateTime.Now
                }));

                await _unitOfWork.UserRoles.AddRangeAsync(list, cancellationToken);
            }

            await context.RespondAsync(new ConsumerAccepted<UserRoleCreateResponseModel>()
            {
                Data = default(UserRoleCreateResponseModel),
                Message = ConsumerMessage.CREATE_SUCCESSFULLY("UserRole"),
                StatusCode = ConsumerStatusCode.Success
            });
        }
        catch (Exception ex)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                Reason = ex.InnerException != null ? ex.InnerException.Message : ex.Message,
                StatusCode = ConsumerStatusCode.BadRequest
            });
        }
    }
}