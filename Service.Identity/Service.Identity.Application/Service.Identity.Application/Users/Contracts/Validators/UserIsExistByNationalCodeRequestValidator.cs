using FluentValidation;
using Service.Identity.Application.Common;

namespace Service.Identity.Application.Users.Contracts.Validators;

public class UserIsExistByNationalCodeRequestValidator : AbstractValidator<UserIsExistByNationalCodeRequestModel>
{
    public UserIsExistByNationalCodeRequestValidator()
    {
        RuleFor(x => x.NationalCode).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("NationalCode"));
    }
}