using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Service.Identity.Domain.Users;

namespace Service.Identity.Infrastructure.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User", "Auth");

        builder.HasKey(x => x.Id);


        builder.HasIndex(x => x.UserName).HasFilter("[IsDeleted] = 0").IsUnique();
        builder.HasIndex(x => x.PhoneNumber).HasFilter("[IsDeleted] = 0").IsUnique();

        // important !
        // reasons for removing this index : we can create duplicate username when that was deleted by isDeleted property.
        builder.Metadata.RemoveIndex(new[] { builder.Property(u => u.NormalizedUserName).Metadata });
        builder.HasIndex(x => x.NormalizedUserName).HasName("UserNameIndex").IsUnique().HasFilter("[NormalizedUserName] IS NOT NULL AND [IsDeleted] = 0");

        builder.Property(x => x.FirstName).IsRequired(true);
        builder.Property(x => x.LastName).IsRequired(true);
        builder.Property(x => x.Grade).IsRequired(true).HasDefaultValue("0");
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.IsDeleted).IsRequired(false).HasDefaultValue(false);
        builder.Property(x => x.FullName).HasComputedColumnSql("concat([FirstName],' ',[LastName])", stored: true);

        #region Relationship
        builder.HasOne(x => x.CreatedUser)
               .WithMany()
               .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.ModifiedUser)
               .WithMany()
               .HasForeignKey(x => x.ModifiedById);

        builder.HasMany(x => x.UserPolicies);
        builder.HasMany(x => x.UserRoles);
        builder.HasMany(x => x.CreatedRoles);
        builder.HasMany(x => x.ModifiedRoles);
        builder.HasMany(x => x.CreatedRolePolicies);
        builder.HasMany(x => x.ModifiedRolePolicies);
        builder.HasMany(x => x.CreatedUserPolicies);
        builder.HasMany(x => x.ModifiedUserPolicies);
        builder.HasMany(x => x.CreatedUserRoles);
        builder.HasMany(x => x.ModifiedUserRoles);
        #endregion
    }
}