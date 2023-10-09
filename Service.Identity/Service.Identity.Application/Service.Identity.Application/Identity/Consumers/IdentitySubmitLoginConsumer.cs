using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Configuration.Token;
using Service.Identity.Application.Constants;
using Service.Identity.Application.Identity.Contracts;
using Service.Identity.Domain.Configuration;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using static MassTransit.ValidationResultExtensions;
using Service.Identity.Domain.Common;
using Service.Identity.Application.Common;

namespace Service.Identity.Application.Identity.Consumers;

public class IdentitySubmitLoginConsumer : IConsumer<IdentitySubmitLoginRequestModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenHandler _tokenHandler;
    private readonly IUserInfo _userInfo;
    private readonly IConfiguration _configuration;

    public IdentitySubmitLoginConsumer(IUnitOfWork unitOfWork, ITokenHandler tokenHandler, IUserInfo userInfo,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _tokenHandler = tokenHandler;
        _userInfo = userInfo;
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<IdentitySubmitLoginRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;
            List<string> userRoles;

            var isPharmacy = request.TenantId.HasValue && request.BranchId.HasValue;
            string branchColor = null;

            #region validation

            var user = await _unitOfWork.Users.TableNoTracking.FirstOrDefaultAsync(x => x.Id.Equals(_userInfo.UserId),
                cancellationToken);
            if (user is null)
            {
                await context.RespondAsync<ConsumerRejected>(new
                {
                    StatusCode = ConsumerStatusCode.UnAuthorized,
                    Errors = new[]
                    {
                        "Unauthorized"
                    }
                });

                return;
            }


            var grades = user.Grade.Replace(" ", "").Split(",");
   
            var isBackup = grades.Any(grade => grade.Equals(UserTypesConstants.BackUp));

      

            #endregion

            #region get roles

            // end user
            if (isPharmacy && grades.Any(grade => grade.Equals(UserTypesConstants.EndUser)) && grades.Count().Equals(1))
            {
                userRoles = await _unitOfWork.UserRoles.TableNoTracking.Include(x => x.Role)
                    .Where(x => x.UserId.Equals(_userInfo.UserId) &&
                                x.TenantId.Value.Equals(request.TenantId))
                    .Where(x => grades.Any(g => g.Equals(x.Role.Grade.ToString())))
                    .Select(x => x.Role.Name)
                    .ToListAsync(cancellationToken);
            }
            // backup user and back office and external user
            else
            {
                userRoles = await _unitOfWork.UserRoles.TableNoTracking.Include(x => x.Role)
                    .Where(x => x.UserId.Equals(_userInfo.UserId) && !x.TenantId.HasValue)
                    .Where(x => grades.Any(g => g.Equals(x.Role.Grade.ToString())))
                    .Select(x => x.Role.Name)
                    .ToListAsync(cancellationToken);
            }

            #endregion

            #region set claims and generate token

            var userClaims = _tokenHandler.GenerateClaims(user, userRoles);

   

            var token = _tokenHandler.Generate(userClaims, false, user!.CanUseRefreshToken);
            token.BranchColor = branchColor;
            if (!string.IsNullOrEmpty(token.RefreshToken))
            {
                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out var refreshTokenValidityInDays);
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(refreshTokenValidityInDays);
                await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            }

            //_unitOfWork.Users.AddUserToWhiteList()

            #endregion

            await context.RespondAsync<ConsumerAccepted<IdentityTokenResponseModel>>(new
            {
                Data = token,
                StatusCode = ConsumerStatusCode.Success,
                Message = "submit_login_successfully"
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