using Service.Identity.Application.Common;

namespace Service.Identity.Application.Users.Contracts;

public class UserGetRequestModel : IContract
{
    public long Id { get; set; }
}