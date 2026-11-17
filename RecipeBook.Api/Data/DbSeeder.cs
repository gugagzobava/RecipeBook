namespace RecipeBook.Api.Data;

public static class DbSeeder
{
    public const string AdminEmail = "admin@recipebook.com";
    public const string AdminPassword = "Admin123!";
    public const string UserEmail = "user@recipebook.com";
    public const string UserPassword = "User123!";

    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(new[] { "სუპები", "დესერტები", "ცომეული", "ძირითადი კერძები", "სალათები", "საუზმე" }
                .Select(n => new Category { Name = n }));
        }

        if (!await db.Ingredients.AnyAsync())
        {
            db.Ingredients.AddRange(new[]
            {
                "ფქვილი", "შაქარი", "მარილი", "რძე", "კარაქი", "საფუარი", "კვერცხი",
                "სულგუნი", "პომიდორი", "ხახვი", "ნიორი", "კარტოფილი", "საქონლის ხორცი", "მცენარეული ზეთი"
            }.Select(n => new Ingredient { Name = n }));
        }

        if (!await db.Users.AnyAsync())
        {
            var admin = new User
            {
                Email = AdminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(AdminPassword),
                DisplayName = "Admin",
                Role = UserRole.Admin
            };
            var chef = new User
            {
                Email = UserEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(UserPassword),
                DisplayName = "Nino Chef",
                Role = UserRole.User,
                ChefProfile = new ChefProfile
                {
                    Bio = "ქართული სამზარეულოს მოყვარული.",
                    Speciality = "ქართული ცომეული",
                    YearsOfExperience = 8
                }
            };
            db.Users.AddRange(admin, chef);
        }

        await db.SaveChangesAsync();

        if (!await db.Recipes.AnyAsync())
        {
            var author = await db.Users.FirstAsync(u => u.Email == UserEmail);
            var category = await db.Categories.FirstAsync(c => c.Name == "ცომეული");
            var ing = await db.Ingredients.ToDictionaryAsync(i => i.Name);

            db.Recipes.Add(new Recipe
            {
                Title = "ხაჭაპური აჭარული",
                Description = "კლასიკური აჭარული ხაჭაპური ნავის ფორმით.",
                CookingMinutes = 45,
                Servings = 2,
                Difficulty = Difficulty.Medium,
                CategoryId = category.Id,
                AuthorId = author.Id,
                Steps = new List<Step>
                {
                    new() { Order = 1, Instruction = "მოზილეთ ცომი და დააყოვნეთ 30 წუთი." },
                    new() { Order = 2, Instruction = "გააბრტყელეთ ნავის ფორმით და შიგნით ჩაყარეთ ყველი." },
                    new() { Order = 3, Instruction = "გამოაცხვეთ 220 გრადუსზე და ბოლოს დაამატეთ კვერცხი." }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new() { IngredientId = ing["ფქვილი"].Id, Quantity = 500, Unit = Unit.Gram },
                    new() { IngredientId = ing["სულგუნი"].Id, Quantity = 300, Unit = Unit.Gram },
                    new() { IngredientId = ing["კვერცხი"].Id, Quantity = 2, Unit = Unit.Piece }
                }
            });
            await db.SaveChangesAsync();
        }
    }
}
