using Microsoft.AspNetCore.Mvc;
using Service.Identity.Application.Common;

namespace Service.Identity.Api.Validation;

public class ForbiddenExceptionProblemDetails : ProblemDetails
{
    public ForbiddenExceptionProblemDetails(ForbiddenException exception)
    {
        Title = "Forbidden Exception";
        Status = StatusCodes.Status403Forbidden;
        Detail = exception.Message;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
        Errors = exception.Errors;
    }

    public Dictionary<string, string[]> Errors { get; set; }
}
