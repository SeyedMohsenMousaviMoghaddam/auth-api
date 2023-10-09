using Service.Identity.Domain.Enums;
using System.Net;


namespace Service.Identity.Domain.Common;

public class BusinessRuleValidationException : BaseException
{
    public IBusinessRule BrokenRule { get; }

    public string Details { get; }

    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(ApiResultStatusCode.BadRequest, brokenRule.Message, HttpStatusCode.BadRequest)
    {
        BrokenRule = brokenRule;
        Details = brokenRule.Message;
    }

    public override string ToString()
    {
        return $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
    }
}
