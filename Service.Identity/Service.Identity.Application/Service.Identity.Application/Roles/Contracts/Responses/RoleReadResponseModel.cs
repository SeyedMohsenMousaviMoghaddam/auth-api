using AutoMapper;
using Service.Identity.Application.Common;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Roles;

namespace Service.Identity.Application.Roles.Contracts;

public class RoleReadResponseModel : IMapping<Role>, IContract, ILocked
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool? Locked { get; set; }
    public string CreatedUserFullName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string ModifiedUserFullName { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public void Mapping(Profile profile, IUserInfo userInfo)
    {
        profile.CreateMap<Role, RoleReadResponseModel>()
               .ForMember(x => x.CreatedUserFullName, opt => opt.MapFrom(c => c.CreatedUser.FirstName + " " + c.CreatedUser.LastName))
               .ForMember(x => x.ModifiedUserFullName, opt => opt.MapFrom(c => c.ModifiedUser.FirstName + " " + c.ModifiedUser.LastName));
    }
}