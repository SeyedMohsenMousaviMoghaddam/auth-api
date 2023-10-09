using AutoMapper;
using Service.Identity.Application.Common;
using Service.Identity.Application.Roles.Contracts;
using Service.Identity.Domain;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Users;

namespace Service.Identity.Application.Users.Contracts;

public class UserReadResponseModel : IMapping<User>, IContract
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string FullName { get; set; }
    public string CreatedUserFullName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string ModifiedUserFullName { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string Grade { get; set; }
    public List<RoleReadResponseModel> Roles { get; set; }

    public void Mapping(Profile profile, IUserInfo userInfo)
    {
        profile.CreateMap<User, UserReadResponseModel>()
            .ForMember(x => x.CreatedUserFullName,
                opt => opt.MapFrom(c => c.CreatedUser.FirstName + " " + c.CreatedUser.LastName))
            .ForMember(x => x.ModifiedUserFullName,
                opt => opt.MapFrom(c => c.ModifiedUser.FirstName + " " + c.ModifiedUser.LastName));
    }
}