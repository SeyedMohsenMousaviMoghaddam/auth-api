using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Identity.Application.Common;
using Service.Identity.Application.Configuration.Token;
using Service.Identity.Application.Constants;
using Service.Identity.Application.Identity.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Domain.Users;

namespace Service.Identity.Application.Identity.Consumers;

public class IdentityRefreshTokenConsumer : IConsumer<IdentityRefreshTokenRequest>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenHandler _tokenHandler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public IdentityRefreshTokenConsumer(UserManager<User> userManager, ITokenHandler tokenHandler,
        IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _userManager = userManager;
        _tokenHandler = tokenHandler;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<IdentityRefreshTokenRequest> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;


        var principal = _tokenHandler.GetPrincipalFromExpiredToken(request.Token);
        if (principal is null)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Errors = new[]
                    {
                        "Invalid access token or refresh token"
                    }
            });

            return;
        }

        var dispatchedToken = _tokenHandler.ReadToken(request.Token);

        var user = await _unitOfWork.Users.TableNoTracking.FirstOrDefaultAsync(
            x => x.Id.Equals(int.Parse(dispatchedToken.sub)),
            cancellationToken);
        if (user is not { CanUseRefreshToken: true } || user.RefreshToken != request.RefreshToken ||
            user.RefreshTokenExpiryTime < DateTimeOffset.UtcNow)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Errors = new[]
                    {
                        "Invalid access token or refresh token"
                    }
            });
            return;
        }


        _ = Guid.TryParse(dispatchedToken.tenantId, out var tenantId);
        _ = long.TryParse(dispatchedToken.branchId, out var branchId);

        var userRoles = new List<string>();

        bool isBackup;

        #region validation


        var grades = user.Grade.Replace(" ", "").Split(",");


        #endregion

        #region get roles

        // end user

        // backup user and back office and external user

            userRoles = await _unitOfWork.UserRoles.TableNoTracking.Include(x => x.Role)
                .Where(x => x.UserId.Equals(int.Parse(dispatchedToken.sub)) && !x.TenantId.HasValue)
                .Where(x => grades.Any(g => g.Equals(x.Role.Grade.ToString())))
                .Select(x => x.Role.Name)
                .ToListAsync(cancellationToken);
        

        #endregion

        #region set claims and generate token

        var userClaims = _tokenHandler.GenerateClaims(user, userRoles);
        userClaims.Add(new Claim("tenant_id", tenantId == Guid.Empty ? "" : tenantId.ToString()));
        userClaims.Add(new Claim("branch_id", branchId == 0 ? "" : branchId.ToString()));


        #endregion

        var newToken = _tokenHandler.Generate(userClaims, false, true);
        if (!string.IsNullOrEmpty(newToken.RefreshToken))
        {
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out var refreshTokenValidityInDays);
            user.RefreshToken = newToken.RefreshToken;
            user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(refreshTokenValidityInDays);
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        }

        await context.RespondAsync<ConsumerAccepted<IdentityTokenResponseModel>>(new
        {
            Data = newToken,
            StatusCode = ConsumerStatusCode.Success,
            Message = "submit_login_successfully"
        });
    }
}