using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecipeBook.Api.Data.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> b)
    {
        b.HasKey(i => i.Id);
        b.Property(i => i.Name).HasMaxLength(100).IsRequired();
        b.HasIndex(i => i.Name).IsUnique();
    }
}
