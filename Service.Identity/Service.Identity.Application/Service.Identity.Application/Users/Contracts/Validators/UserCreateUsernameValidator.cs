using Microsoft.AspNetCore.Identity;
using Service.Identity.Domain.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Service.Identity.Application.Users.Contracts.Validators;

public class UserCreateUsernameValidator<TUser> : IUserValidator<TUser> where TUser : class
{
    private readonly IUnitOfWork _unitOfWork;
    public IdentityErrorDescriber Describer { get; set; }

    public UserCreateUsernameValidator(IUnitOfWork unitOfWork, IdentityErrorDescriber? describer = null)
    {
        _unitOfWork = unitOfWork;
        Describer = describer ?? new IdentityErrorDescriber();
    }

    public virtual async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    {
        var result = new List<IdentityResult>();

        if (manager == null)
            throw new ArgumentNullException(nameof(manager));

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var errors = await ValidateUserName(manager, user).ConfigureAwait(false);

        if (manager.Options.User.RequireUniqueEmail)
        {
            errors = await ValidateEmail(manager, user, errors).ConfigureAwait(false);
        }

        if (errors is not null)
        {
            for (int i = 0; i < errors.Count; i++)
            {
                result.Add(IdentityResult.Failed(errors[i]));
            }
        }

        return result.Any() ? result.FirstOrDefault() : IdentityResult.Success;
    }

    private async Task<List<IdentityError>?> ValidateUserName(UserManager<TUser> manager, TUser user)
    {
        var errors = new List<IdentityError>();

        var userName = await manager.GetUserNameAsync(user).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(userName))
        {
            errors.Add(Describer.InvalidUserName(userName));
        }
        else if (!string.IsNullOrEmpty(manager.Options.User.AllowedUserNameCharacters) &&
                 userName.Any(c => !manager.Options.User.AllowedUserNameCharacters.Contains(c)))
        {
            errors.Add(Describer.InvalidUserName(userName));
        }

        return errors;
    }

    private async Task<List<IdentityError>?> ValidateEmail(UserManager<TUser> manager, TUser user,
        List<IdentityError>? errors)
    {
        var email = await manager.GetEmailAsync(user).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(email))
        {
            errors ??= new List<IdentityError>();
            errors.Add(Describer.InvalidEmail(email));
            return errors;
        }
        if (!new EmailAddressAttribute().IsValid(email))
        {
            errors ??= new List<IdentityError>();
            errors.Add(Describer.InvalidEmail(email));
            return errors;
        }

        var owner = await manager.FindByEmailAsync(email).ConfigureAwait(false);
        if (owner != null && !string.Equals(await manager.GetUserIdAsync(owner).ConfigureAwait(false),
                await manager.GetUserIdAsync(user).ConfigureAwait(false)))
        {
            errors ??= new List<IdentityError>();
            errors.Add(Describer.DuplicateEmail(email));
        }

        return errors;
    }
}