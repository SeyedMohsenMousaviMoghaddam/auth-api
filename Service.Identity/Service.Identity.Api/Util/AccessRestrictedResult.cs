using Microsoft.AspNetCore.Mvc;

namespace Service.Identity.Api.Util;

public class AccessRestrictedResult : StatusCodeResult
{
    private const int DefaultStatusCode = StatusCodes.Status403Forbidden;

    public AccessRestrictedResult() : base(DefaultStatusCode)
    {
    }
}