using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Service.Identity.Domain.Policies;

namespace Service.Identity.Infrastructure.Policies;

public class RolePolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policy", "Auth");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.Value);
        builder.HasIndex(x => new { x.ApiResource, x.ApiScope, x.Value }).IsUnique();

        builder.Property(x => x.Grade).IsRequired(true).HasDefaultValue(0);
        builder.Property(x => x.ApiResource).HasColumnName("ApiResource").IsRequired(true);
        builder.Property(x => x.ApiScope).HasColumnName("ApiScope").IsRequired(true);

        #region Relationship
        builder.HasMany(x => x.RolePolicies);
        builder.HasMany(x => x.UserPolicies);
        #endregion
    }
}