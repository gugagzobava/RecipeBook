using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecipeBook.Api.Data.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> b)
    {
        b.HasKey(r => r.Id);
        b.Property(r => r.Title).HasMaxLength(150).IsRequired();
        b.Property(r => r.Description).HasMaxLength(2000);
        b.Property(r => r.Difficulty).HasConversion<string>().HasMaxLength(10);

        // 1:N Category -> Recipe. A category that still has recipes cannot be deleted.
        b.HasOne(r => r.Category)
            .WithMany(c => c.Recipes)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1:N User -> Recipe (author). Restrict: deleting a user must not silently wipe recipes.
        b.HasOne(r => r.Author)
            .WithMany(u => u.Recipes)
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
