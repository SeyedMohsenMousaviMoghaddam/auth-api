using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Service.Identity.Domain.RolePolicies;

namespace Service.Identity.Infrastructure.RolePolicies;

public class RolePolicyConfiguration : IEntityTypeConfiguration<RolePolicy>
{
    public void Configure(EntityTypeBuilder<RolePolicy> builder)
    {
        builder.ToTable("RolePolicy", "Auth");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.RoleId, x.PolicyId }).IsUnique();

        builder.Property(x => x.PolicyId).IsRequired(true);

        #region Relationship
        builder.HasOne(x => x.Role)
               .WithMany(x => x.RolePolicies)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Policy)
               .WithMany(x => x.RolePolicies)
               .HasForeignKey(x => x.PolicyId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CreatedUser)
               .WithMany(x => x.CreatedRolePolicies)
               .HasForeignKey(x => x.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ModifiedUser)
               .WithMany(x => x.ModifiedRolePolicies)
               .HasForeignKey(x => x.ModifiedById)
               .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }
}