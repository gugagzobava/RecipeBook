using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecipeBook.Api.Data.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> b)
    {
        // N:M Recipe <-> Ingredient with extra fields => explicit join entity + composite key.
        b.HasKey(x => new { x.RecipeId, x.IngredientId });

        b.Property(x => x.Quantity).HasPrecision(18, 3);
        b.Property(x => x.Unit).HasConversion<string>().HasMaxLength(10);

        b.HasOne(x => x.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(x => x.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // An ingredient that is used in recipes cannot be deleted.
        b.HasOne(x => x.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(x => x.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
