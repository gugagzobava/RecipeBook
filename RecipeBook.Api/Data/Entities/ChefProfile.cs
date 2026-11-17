namespace RecipeBook.Api.Data.Entities;

public class ChefProfile
{
    public int Id { get; set; }
    public string? Bio { get; set; }
    public string Speciality { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
