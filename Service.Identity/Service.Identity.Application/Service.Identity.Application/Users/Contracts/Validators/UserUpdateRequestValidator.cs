using Service.Identity.Application.Common;
using FluentValidation;

namespace Service.Identity.Application.Users.Contracts.Validators;

public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequestModel>
{
    public UserUpdateRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("FirstName"));
        RuleFor(x => x.LastName).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("LastName"));
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("PhoneNumber"));
    }
}