namespace RecipeBook.Api.Data.Entities;

/// <summary>Explicit join entity for Recipe N:M Ingredient (has Quantity and Unit).</summary>
public class RecipeIngredient
{
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public decimal Quantity { get; set; }
    public Unit Unit { get; set; }
}
