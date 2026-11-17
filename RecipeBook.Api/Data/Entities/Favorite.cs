namespace RecipeBook.Api.Data.Entities;

/// <summary>Explicit join entity for User N:M Recipe (has AddedAt). Composite key (UserId, RecipeId).</summary>
public class Favorite
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
