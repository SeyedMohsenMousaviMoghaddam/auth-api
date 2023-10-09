using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Service.Identity.Domain.Enums;

namespace Service.Identity.Api.Filters;

public class AuthorizationAttribute : TypeFilterAttribute
{
    private string[] _policies;
    private readonly AuthOperator _authOperator;

    public AuthorizationAttribute(string policies, AuthOperator authOperator = AuthOperator.AND) : base(typeof(OnAuthorization))
    {
        _policies = policies.Split(',');
        _authOperator = authOperator;
    }

    private class OnAuthorization : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                // var data = token.ReadJwt();
                //
                // var userClaims = new List<Claim>
                // {
                //     new Claim("TenantId", "3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                //     new Claim("BranchId", "070DD839-0DB7-4AF9-AB3B-1445E02F68D8"),
                //     new Claim("UserId", "333c0137-6eaa-44d3-ab46-6c845d3487bd"),
                // };
                // context.HttpContext.User.AddIdentity(new ClaimsIdentity(userClaims));
            }
            else
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}