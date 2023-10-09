using Service.Identity.Application.Common;
using FluentValidation;

namespace Service.Identity.Application.Roles.Contracts.Validators;

public class RoleGetAvailableRequestValidator : AbstractValidator<RoleGetAvailableRequestModel>
{
    public RoleGetAvailableRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage(FluentValiationMessage.CANNOT_BE_EMPTY("UserId"));
    }
}