using Microsoft.AspNetCore.Mvc;
using Service.Identity.Application.Common;

namespace Service.Identity.Api.Validation;
public class BadRequestExceptionProblemDetails : ProblemDetails
{
    public BadRequestExceptionProblemDetails(BadRequestException exception)
    {
        Title = "BadRequest Exception";
        Status = StatusCodes.Status400BadRequest;
        Detail = exception.Message;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
        Errors = exception.Errors;
    }

    public Dictionary<string, string[]> Errors { get; set; }
}
