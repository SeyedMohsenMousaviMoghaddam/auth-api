using Microsoft.AspNetCore.Mvc;
using Service.Identity.Application.Common;

namespace Service.Identity.Api.Validation;

public class NotFoundExceptionProblemDetails : ProblemDetails
{
    public NotFoundExceptionProblemDetails(NotFoundException exception)
    {
        Title = "NotFound Exception";
        Status = StatusCodes.Status404NotFound;
        Detail = exception.Message;
        Type = "https://somedomain/business-rule-validation-error";
        Errors = exception.Errors;
    }

    public Dictionary<string, string[]> Errors { get; set; }
}
