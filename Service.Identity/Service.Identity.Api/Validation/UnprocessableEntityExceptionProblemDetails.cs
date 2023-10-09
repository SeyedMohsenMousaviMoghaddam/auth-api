using Microsoft.AspNetCore.Mvc;
using Service.Identity.Application.Common;

namespace Service.Identity.Api.Validation;

public class UnprocessableEntityExceptionProblemDetails : ProblemDetails
{
    public UnprocessableEntityExceptionProblemDetails(UnprocessableEntityException exception)
    {
        Title = "UnprocessableEntity Exception";
        Status = StatusCodes.Status422UnprocessableEntity;
        Detail = exception.Message;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
        Errors = exception.Errors;
    }

    public Dictionary<string, string[]> Errors { get; set; }
}
