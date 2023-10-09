using Service.Identity.Domain.Roles;
using Service.Identity.Domain.Users;

namespace Service.Identity.Infrastructure.Configuration;

public class IdentityData
{
    public List<SeedRole> Roles { get; set; }
    public List<SeedUser> Users { get; set; }
}

public class SeedUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<string> Roles { get; set; } = new ();
}

public class SeedRole
{
    public string Name { get; set; }
}