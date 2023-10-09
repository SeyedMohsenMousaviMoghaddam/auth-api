using FluentValidation;
using Service.Identity.Application.Common;

namespace Service.Identity.Application.UserRoles.Contracts.Validators;

public class UserRoleCreateRequestValidator : AbstractValidator<UserRoleCreateRequestModel>
{
    public UserRoleCreateRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("UserId"));
    }
}