using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Service.Identity.Application.Common;

namespace Service.Identity.Api.Validation;

public class InvalidCommandProblemDetails : ProblemDetails
{
    public InvalidCommandProblemDetails(InvalidCommandException exception, bool activeExceptionDetails)
    {
        if (activeExceptionDetails)
        {
            Title = "One or more validation errors occurred";
            Status = StatusCodes.Status400BadRequest;
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
            Errors = exception.Errors;
            //Errors=exception.
        }
        else
        {
            Title = "Unprocessable Entity";
            Status = StatusCodes.Status422UnprocessableEntity;
            Type = "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2";
        }
    }

    public InvalidCommandProblemDetails(RequestException exception, bool activeExceptionDetails)
    {
        if (exception.InnerException is InvalidCommandException)
        {
            if (activeExceptionDetails)
            {
                Title = "One or more validation errors occurred";
                Status = StatusCodes.Status400BadRequest;
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
                Errors = (exception.InnerException as InvalidCommandException).Errors;
            }
        }
        else
        {
            Title = "Unprocessable Entity";
            Status = StatusCodes.Status422UnprocessableEntity;
            Type = "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2";
        }
    }

    public InvalidCommandProblemDetails()
    {
    }

    public Dictionary<string, string[]> Errors { get; set; }
}