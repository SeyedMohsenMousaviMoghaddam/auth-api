namespace Service.Identity.Infrastructure.Configuration;

public class DatabaseMigrationsConfiguration
{
    public bool ApplyDatabaseMigrations { get; set; } = false;

    public string IdentityDbMigrationsAssembly { get; set; }

    public string DataProtectionDbMigrationsAssembly { get; set; }

    public void SetMigrationsAssemblies(string commonMigrationsAssembly)
    {
        DataProtectionDbMigrationsAssembly = commonMigrationsAssembly;
        IdentityDbMigrationsAssembly = commonMigrationsAssembly;
    }
}