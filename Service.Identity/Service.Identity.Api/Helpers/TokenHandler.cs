using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Service.Identity.Application.Configuration.Token;
using Service.Identity.Application.Identity.Contracts;
using Service.Identity.Domain.Users;

namespace Service.Identity.Api.Helpers;

public class TokenHandler : ITokenHandler
{
    private readonly IConfiguration _configuration;

    public TokenHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IdentityTokenResponseModel Generate(List<Claim> authClaims, bool temp, bool canUseRefreshToken)
    {
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        string? secret;
        _ = int.TryParse(_configuration["JWT:TempTokenValidityInMinutes"], out var tempTokenValidityInMinutes);
        _ = int.TryParse(_configuration["JWT:PermanentTokenValidityInDays"], out var permanentTokenValidityInDays);

        if (temp)

            secret = isDevelopment
                ? _configuration[$"TempJWT:Secret"]
                : Environment.GetEnvironmentVariable("TEMP_SECRET");

        else
            secret = isDevelopment ? _configuration[$"JWT:Secret"] : Environment.GetEnvironmentVariable("SECRET");


        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var expiration = temp
            ? DateTime.Now.AddMinutes(tempTokenValidityInMinutes)
            : DateTime.Now.AddDays(permanentTokenValidityInDays);

        authClaims.AddRange(new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Exp, expiration.ToBinary().ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        });
        var token = new JwtSecurityToken(
            expires: expiration,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new IdentityTokenResponseModel()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = canUseRefreshToken ? GenerateRefreshToken() : null,
            Expiration = token.ValidTo
        };
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        string? secret;
        secret = isDevelopment ? _configuration[$"JWT:Secret"] : Environment.GetEnvironmentVariable("SECRET");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public bool IsValidToken(string token, bool temp)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSection = temp ? "TempJWT" : "JWT";

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[$"{jwtSection}:Secret"]));
        try
        {
            tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = authSigningKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                },
                out SecurityToken validatedToken);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public IdentityTokenDataResponseModel ReadToken(string bearerToken)
    {
        var token = bearerToken.Replace("Bearer ", string.Empty);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenData = tokenHandler.ReadJwtToken(token);
        var attrs = tokenData.Claims.FirstOrDefault(x => x.Type == "attrs")?.Value;
        var result = new IdentityTokenDataResponseModel()
        {
            sub = tokenData.Claims.FirstOrDefault(x => x.Type == "nameidentifier")?.Value,
            username = tokenData.Claims.FirstOrDefault(x => x.Type == "username")?.Value,
            tenantId = tokenData.Claims.FirstOrDefault(x => x.Type == "tenant_id")?.Value,
            branchId = tokenData.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value,
            roles = tokenData.Claims.FirstOrDefault(x => x.Type == "roles")?.Value,
            attrs = string.IsNullOrEmpty(attrs) ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(attrs),
        };
        return result;
    }

    public List<Claim> GenerateClaims(User user, List<string>? roles = null, List<string>? policies = null)
    {
        var authClaims = new List<Claim>
        {
            new("nameidentifier", user.Id.ToString()),
            new("username", user.UserName),
            new("phonenumber", user.PhoneNumber),
            new("firstname", user.FirstName),
            new("lastname", user.LastName),
            new("grade", user.Grade)
        };

        if (roles is not null)
            authClaims.Add(new Claim("roles", JsonSerializer.Serialize(roles)));

        if (policies is not null)
            authClaims.Add(new Claim("policies", JsonSerializer.Serialize(policies)));

        return authClaims;
    }
}