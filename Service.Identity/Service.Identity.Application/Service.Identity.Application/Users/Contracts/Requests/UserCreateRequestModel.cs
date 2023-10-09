using Service.Identity.Application.Common;

namespace Service.Identity.Application.Users.Contracts;

public class UserCreateRequestModel : IContract
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string NationalCode { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}