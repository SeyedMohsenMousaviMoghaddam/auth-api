using Service.Identity.Application.Common;

namespace Service.Identity.Application.Users.Contracts;

public class UserCheckWhiteResponse : IContract
{
    public bool IsWhite { get; set; }
}