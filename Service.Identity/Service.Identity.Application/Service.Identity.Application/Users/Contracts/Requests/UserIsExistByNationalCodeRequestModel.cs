using Service.Identity.Application.Common;

namespace Service.Identity.Application.Users.Contracts;

public class UserIsExistByNationalCodeRequestModel : IContract
{
    public string NationalCode { get; set; }
}