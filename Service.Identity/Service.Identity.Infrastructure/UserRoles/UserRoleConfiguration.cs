using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Service.Identity.Domain.UserRoles;

namespace Service.Identity.Infrastructure.UserRoles;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRole", "Auth");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UserId, x.RoleId, x.TenantId });

        builder.Property(x => x.UserId).IsRequired(true);
        builder.Property(x => x.RoleId).IsRequired(true);
        builder.Property(x => x.TenantId).IsRequired(false);

        #region Relationship
        builder.HasOne(x => x.Role)
               .WithMany(x => x.UserRoles)
               .OnDelete(DeleteBehavior.Restrict)
               .HasForeignKey(x => x.RoleId);

        builder.HasOne(x => x.User)
               .WithMany(x => x.UserRoles)
               .OnDelete(DeleteBehavior.Restrict)
               .HasForeignKey(x => x.UserId);


        builder.HasOne(x => x.CreatedUser)
               .WithMany(x => x.CreatedUserRoles)
               .HasForeignKey(x => x.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ModifiedUser)
               .WithMany(x => x.ModifiedUserRoles)
               .HasForeignKey(x => x.ModifiedById)
               .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }
}