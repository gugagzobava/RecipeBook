using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecipeBook.Api.Data.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> b)
    {
        // N:M User <-> Recipe with AddedAt => explicit join entity, composite key (UserId, RecipeId).
        b.HasKey(f => new { f.UserId, f.RecipeId });

        b.HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(f => f.Recipe)
            .WithMany(r => r.Favorites)
            .HasForeignKey(f => f.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
