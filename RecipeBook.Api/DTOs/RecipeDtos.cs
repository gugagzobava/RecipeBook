namespace RecipeBook.Api.DTOs;

// ---------- Requests ----------
public class StepRequest
{
    public int Order { get; set; }
    public string Instruction { get; set; } = string.Empty;
}

public class IngredientLineRequest
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public Unit Unit { get; set; }
}

public class CreateRecipeRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CookingMinutes { get; set; }
    public int Servings { get; set; }
    public Difficulty Difficulty { get; set; }
    public int CategoryId { get; set; }
    public List<StepRequest> Steps { get; set; } = new();
    public List<IngredientLineRequest> Ingredients { get; set; } = new();
}

/// <summary>PUT is a full replacement, so it has the same shape (and rules) as create.</summary>
public class UpdateRecipeRequest : CreateRecipeRequest { }

public class RecipeQuery : PaginationQuery
{
    public int? CategoryId { get; set; }
    public Difficulty? Difficulty { get; set; }
    public int? MaxMinutes { get; set; }
    public int? IngredientId { get; set; }
    public int? AuthorId { get; set; }
    public string? Search { get; set; }
}

public class SearchByIngredientsRequest
{
    public List<int> IngredientIds { get; set; } = new();
}

// ---------- Responses ----------
public class RecipeListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public int CookingMinutes { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int FavoritesCount { get; set; }
    public int CommentsCount { get; set; }
}

public class StepDto
{
    public int Order { get; set; }
    public string Instruction { get; set; } = string.Empty;
}

public class IngredientLineDto
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public Unit Unit { get; set; }
}

public class CommentDto
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public int UserId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RecipeDetailsDto : RecipeListDto
{
    public string? Description { get; set; }
    public int Servings { get; set; }
    public int CategoryId { get; set; }
    public int AuthorId { get; set; }
    public string? AuthorSpeciality { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsFavorite { get; set; }
    public List<StepDto> Steps { get; set; } = new();
    public List<IngredientLineDto> Ingredients { get; set; } = new();
    public List<CommentDto> Comments { get; set; } = new();
}

public class RecipeSuggestionDto
{
    public RecipeListDto Recipe { get; set; } = null!;
    public List<string> MissingIngredients { get; set; } = new();
}

public class IngredientSearchResultDto
{
    public List<RecipeListDto> CanCook { get; set; } = new();
    public List<RecipeSuggestionDto> AlmostCanCook { get; set; } = new();
}

public class CreateCommentRequest
{
    public string Text { get; set; } = string.Empty;
}

public class FavoriteDto
{
    public int RecipeId { get; set; }
    public DateTime AddedAt { get; set; }
}
