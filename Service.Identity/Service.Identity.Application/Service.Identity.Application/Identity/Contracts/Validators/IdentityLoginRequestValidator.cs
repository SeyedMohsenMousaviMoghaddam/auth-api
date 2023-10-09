using FluentValidation;

namespace Service.Identity.Application.Identity.Contracts.Validators;

public class IdentityLoginRequestValidator : AbstractValidator<IdentityLoginRequestModel>
{
    public IdentityLoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}