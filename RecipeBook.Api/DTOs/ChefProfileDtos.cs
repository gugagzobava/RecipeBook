namespace RecipeBook.Api.DTOs;

public class UpsertChefProfileRequest
{
    public string? Bio { get; set; }
    public string Speciality { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
}

public class ChefProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string Speciality { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public int RecipesCount { get; set; }
}
