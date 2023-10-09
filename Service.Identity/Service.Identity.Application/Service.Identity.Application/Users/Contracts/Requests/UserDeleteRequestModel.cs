using Service.Identity.Application.Common;

namespace Service.Identity.Application.Users.Contracts;

public class UserDeleteRequestModel : IContract
{
    public long UserId { get; set; }
}