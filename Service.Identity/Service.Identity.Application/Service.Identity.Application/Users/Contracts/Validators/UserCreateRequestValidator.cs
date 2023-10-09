using FluentValidation;
using Service.Identity.Application.Common;

namespace Service.Identity.Application.Users.Contracts.Validators;

public class UserCreateRequestValidator : AbstractValidator<UserCreateRequestModel>
{
    public UserCreateRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("FirstName"));
        RuleFor(x => x.LastName).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("LastName"));
        RuleFor(x => x.Password).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("Password"));
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("ConfirmPassword"));
        RuleFor(x => x).Must(x => x.Password.Equals(x.ConfirmPassword)).WithMessage("password_not_match_with_confirm_password");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("PhoneNumber"));
        RuleFor(x => x.NationalCode).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("NationalCode"));
    }
}