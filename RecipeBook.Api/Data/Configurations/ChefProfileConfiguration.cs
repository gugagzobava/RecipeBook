using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecipeBook.Api.Data.Configurations;

public class ChefProfileConfiguration : IEntityTypeConfiguration<ChefProfile>
{
    public void Configure(EntityTypeBuilder<ChefProfile> b)
    {
        b.HasKey(c => c.Id);
        b.Property(c => c.Bio).HasMaxLength(1000);
        b.Property(c => c.Speciality).HasMaxLength(80).IsRequired();

        // 1:1 User <-> ChefProfile. FK lives on the dependent side (ChefProfile).
        b.HasOne(c => c.User)
            .WithOne(u => u.ChefProfile)
            .HasForeignKey<ChefProfile>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // The unique index is what really enforces 1:1 in the database.
        b.HasIndex(c => c.UserId).IsUnique();
    }
}
