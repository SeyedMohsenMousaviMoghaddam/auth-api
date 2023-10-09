using Service.Identity.Application.Common;
using FluentValidation;

namespace Service.Identity.Application.Identity.Contracts.Validators;

public class IdentityRefreshTokenRequestValidator : AbstractValidator<IdentityRefreshTokenRequest>
{
    public IdentityRefreshTokenRequestValidator()
    {
        RuleFor(c => c.Token).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("Token"));
        RuleFor(c => c.RefreshToken).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("Refresh token"));
    }
}