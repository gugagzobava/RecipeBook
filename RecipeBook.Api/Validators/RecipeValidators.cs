namespace RecipeBook.Api.Validators;

public class StepValidator : AbstractValidator<StepRequest>
{
    public StepValidator()
    {
        RuleFor(x => x.Instruction)
            .NotEmpty().WithMessage("Instruction is required.")
            .Length(3, 500).WithMessage("Instruction must be between 3 and 500 characters.");
    }
}

public class IngredientLineValidator : AbstractValidator<IngredientLineRequest>
{
    public IngredientLineValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Quantity must not exceed 10000.");
        RuleFor(x => x.Unit).IsInEnum().WithMessage("Unit is not a valid value.");
    }
}

/// <summary>Shared rules for create and update (PUT is a full replacement).</summary>
public abstract class RecipeRequestValidator<T> : AbstractValidator<T> where T : CreateRecipeRequest
{
    protected RecipeRequestValidator(AppDbContext db)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(3, 150).WithMessage("Title must be between 3 and 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must be at most 2000 characters.");

        RuleFor(x => x.CookingMinutes)
            .InclusiveBetween(1, 1440).WithMessage("Cooking minutes must be between 1 and 1440.");

        RuleFor(x => x.Servings)
            .InclusiveBetween(1, 50).WithMessage("Servings must be between 1 and 50.");

        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("Difficulty must be Easy, Medium or Hard.");

        RuleFor(x => x.CategoryId)
            .MustAsync((id, ct) => db.Categories.AnyAsync(c => c.Id == id, ct))
            .WithMessage("Category does not exist.");

        // ---- Steps ----
        RuleFor(x => x.Steps)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("At least one step is required.")
            .Must(steps => steps.All(s => s != null)).WithMessage("Steps must not contain empty items.")
            .Must(steps => steps.Select(s => s.Order).Distinct().Count() == steps.Count)
                .WithMessage("Step orders must be unique.")
            .Must(steps => steps.Select(s => s.Order).OrderBy(o => o).SequenceEqual(Enumerable.Range(1, steps.Count)))
                .WithMessage("Step orders must be sequential starting from 1 (1, 2, 3...).");

        RuleForEach(x => x.Steps).SetValidator(new StepValidator());

        // ---- Ingredients ----
        RuleFor(x => x.Ingredients)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("At least one ingredient is required.")
            .Must(list => list.All(i => i != null)).WithMessage("Ingredients must not contain empty items.")
            .Must(list => list.Select(i => i.IngredientId).Distinct().Count() == list.Count)
                .WithMessage("The same ingredient cannot be listed twice.")
            .MustAsync(async (list, ct) =>
            {
                var ids = list.Select(i => i.IngredientId).Distinct().ToList();
                var existing = await db.Ingredients.CountAsync(i => ids.Contains(i.Id), ct);
                return existing == ids.Count;
            }).WithMessage("One or more ingredients do not exist.");

        RuleForEach(x => x.Ingredients).SetValidator(new IngredientLineValidator());
    }
}

public class CreateRecipeRequestValidator : RecipeRequestValidator<CreateRecipeRequest>
{
    public CreateRecipeRequestValidator(AppDbContext db) : base(db) { }
}

public class UpdateRecipeRequestValidator : RecipeRequestValidator<UpdateRecipeRequest>
{
    public UpdateRecipeRequestValidator(AppDbContext db) : base(db) { }
}

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Text is required.")
            .Length(1, 500).WithMessage("Text must be between 1 and 500 characters.");
    }
}

public class SearchByIngredientsRequestValidator : AbstractValidator<SearchByIngredientsRequest>
{
    public SearchByIngredientsRequestValidator()
    {
        RuleFor(x => x.IngredientIds)
            .NotEmpty().WithMessage("Provide at least one ingredient id.");
    }
}
