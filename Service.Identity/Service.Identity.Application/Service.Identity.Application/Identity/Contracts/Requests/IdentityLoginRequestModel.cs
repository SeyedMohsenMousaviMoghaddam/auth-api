using Service.Identity.Application.Common;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityLoginRequestModel : IContract
{
    public string Username { get; set; }
    public string Password { get; set; }
}