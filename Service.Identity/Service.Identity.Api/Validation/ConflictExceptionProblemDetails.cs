using Microsoft.AspNetCore.Mvc;
using Service.Identity.Domain.Common;

namespace Service.Identity.Api.Validation;

public class ConflictExceptionProblemDetails : ProblemDetails
{
    public ConflictExceptionProblemDetails(ConflictException exception)
    {
        Title = "One or more validation errors occurred.";
        Status = StatusCodes.Status409Conflict;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8";
        Errors = exception.Errors;
    }

    public Dictionary<string, string[]> Errors { get; set; }
}
