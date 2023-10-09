using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Service.Identity.Domain.Roles;

namespace Service.Identity.Infrastructure.Roles;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role", "Auth");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Grade).IsRequired(true).HasDefaultValue(0);
        builder.Property(x => x.TenantId);

        #region Relationship
        builder.HasMany(x => x.RolePolicies);
        


        builder.HasOne(x => x.CreatedUser)
               .WithMany(x => x.CreatedRoles)
               .HasForeignKey(x => x.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ModifiedUser)
               .WithMany(x => x.ModifiedRoles)
               .HasForeignKey(x => x.ModifiedById)
               .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }
}