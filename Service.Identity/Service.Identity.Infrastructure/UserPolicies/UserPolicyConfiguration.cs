using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Service.Identity.Domain.UserPolcies;

namespace Service.Identity.Infrastructure.UserPolicies;

public class UserPolicyConfiguration : IEntityTypeConfiguration<UserPolicy>
{
    public void Configure(EntityTypeBuilder<UserPolicy> builder)
    {
        builder.ToTable("UserPolicy", "Auth");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UserId, x.PolicyId }).IsUnique();

        builder.Property(x => x.UserId).IsRequired(true);
        builder.Property(x => x.PolicyId).IsRequired(true);
        builder.Property(x => x.TenantId).IsRequired(true);

        #region Relationship
        builder.HasOne(x => x.User)
               .WithMany(x => x.UserPolicies)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Policy)
               .WithMany(x => x.UserPolicies)
               .OnDelete(DeleteBehavior.Cascade)
               .HasForeignKey(x => x.PolicyId);

        builder.HasOne(x => x.CreatedUser)
               .WithMany(x => x.CreatedUserPolicies)
               .HasForeignKey(x => x.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ModifiedUser)
               .WithMany(x => x.ModifiedUserPolicies)
               .HasForeignKey(x => x.ModifiedById)
               .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }
}