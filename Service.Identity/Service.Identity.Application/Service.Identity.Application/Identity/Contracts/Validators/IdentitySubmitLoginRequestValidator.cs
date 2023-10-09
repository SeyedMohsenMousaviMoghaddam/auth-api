using FluentValidation;

namespace Service.Identity.Application.Identity.Contracts.Validators;

public class IdentitySubmitLoginRequestValidator : AbstractValidator<IdentitySubmitLoginRequestModel>
{
    public IdentitySubmitLoginRequestValidator()
    {
    }
}