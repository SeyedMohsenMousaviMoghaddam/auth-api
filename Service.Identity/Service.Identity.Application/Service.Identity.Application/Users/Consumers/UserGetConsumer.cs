using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Roles.Contracts;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Util;

namespace Service.Identity.Application.Users.Consumers;

public class UserGetConsumer : IConsumer<UserGetRequestModel>
{
    private readonly IMapper _mapper;
    private readonly IUserInfo _userInfo;
    private readonly IUnitOfWork _unitOfWork;

    public UserGetConsumer(IMapper mapper, IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<UserGetRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;

            var user = await _unitOfWork.Users.TableNoTracking.FirstOrDefaultAsync(x => x.Id.Equals(_userInfo.UserId), cancellationToken);
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

            // admin

                var result = await _unitOfWork.Users.TableNoTracking.ExcludeSoftDelete()
                                                                    .Include(x => x.CreatedUser)
                                                                    .Include(x => x.ModifiedUser)
                                                                    .Where(x => user.Grade.Contains(x.Grade))
                                                                    .FirstOrDefaultAsync(c => c.Id.Equals(request.Id), cancellationToken);

                if (result is null)
                {
                    await context.RespondAsync<ConsumerRejected>(new
                    {
                        Errors = new[]
                        {
                            ConsumerMessage.NOTFOUND("User")
                        },
                        StatusCode = ConsumerStatusCode.NotFound
                    });
                    return;
                }

                var mappedResult = _mapper.Map<UserReadResponseModel>(result);
                mappedResult.Roles = _unitOfWork.UserRoles.TableNoTracking.Include(x => x.Role)
                                                                          .Where(x => x.UserId.Equals(request.Id) && !x.TenantId.HasValue)
                                                                          .Where(x => grades.Any(grade => grade.Equals(x.Role.Grade.ToString())))
                                                                          .Select(x => new RoleReadResponseModel { Id = x.Role.Id, Name = x.Role.Name }).ToList();

                await context.RespondAsync<ConsumerAccepted<UserReadResponseModel>>(new
                {
                    Data = mappedResult,
                    StatusCode = ConsumerStatusCode.Success,
                    Message = ConsumerMessage.GET_SUCCESSFULLY("User")
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