using Service.Identity.Application.Common;
using FluentValidation;

namespace Service.Identity.Application.Identity.Contracts.Validators;

public class IdentityChangePasswordRequestValidator : AbstractValidator<IdentityChangePasswordRequestModel>
{
    public IdentityChangePasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("OldPassword"));
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("NewPassword"));
        RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("ConfirmNewPassword"));
        RuleFor(x => x).Must(x => x.NewPassword.Equals(x.ConfirmNewPassword)).WithMessage("new_password_not_match_with_confirm_password");
    }
}