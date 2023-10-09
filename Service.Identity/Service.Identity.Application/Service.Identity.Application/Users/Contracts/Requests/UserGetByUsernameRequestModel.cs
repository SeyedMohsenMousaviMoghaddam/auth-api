using Service.Identity.Application.Common;

namespace Service.Identity.Application.Users.Contracts;

public class UserGetByUsernameRequestModel : IContract
{
    public string Username { get; set; }
}