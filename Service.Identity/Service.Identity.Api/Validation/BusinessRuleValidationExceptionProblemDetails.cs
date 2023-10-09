using Microsoft.AspNetCore.Mvc;
using Service.Identity.Domain.Common;

namespace Service.Identity.Api.Validation;

public class BusinessRuleValidationExceptionProblemDetails : ProblemDetails
{
    public BusinessRuleValidationExceptionProblemDetails(BusinessRuleValidationException exception)
    {
        Title = "Business rule broken";
        Status = StatusCodes.Status409Conflict;
        Detail = exception.Message;
        Type = "https://somedomain/business-rule-validation-error";
    }
}
