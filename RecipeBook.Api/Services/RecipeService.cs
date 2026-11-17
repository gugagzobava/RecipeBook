namespace RecipeBook.Api.Services;

public interface IRecipeService
{
    Task<PagedResult<RecipeListDto>> GetListAsync(RecipeQuery query);
    Task<PagedResult<RecipeListDto>> GetMyRecipesAsync(PaginationQuery paging);
    Task<RecipeDetailsDto> GetByIdAsync(int id);
    Task<RecipeDetailsDto> CreateAsync(CreateRecipeRequest request);
    Task<RecipeDetailsDto> UpdateAsync(int id, UpdateRecipeRequest request);
    Task DeleteAsync(int id);
    Task<IngredientSearchResultDto> SearchByIngredientsAsync(SearchByIngredientsRequest request);
}

public class RecipeService : IRecipeService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _current;

    public RecipeService(AppDbContext db, IMapper mapper, ICurrentUserService current)
    {
        _db = db;
        _mapper = mapper;
        _current = current;
    }

    // ---------------- Queries ----------------

    public Task<PagedResult<RecipeListDto>> GetMyRecipesAsync(PaginationQuery paging) =>
        GetListAsync(new RecipeQuery { AuthorId = _current.UserId, Page = paging.Page, PageSize = paging.PageSize });

    public async Task<PagedResult<RecipeListDto>> GetListAsync(RecipeQuery query)
    {
        var recipes = _db.Recipes.AsNoTracking().AsQueryable();

        if (query.CategoryId.HasValue)
            recipes = recipes.Where(r => r.CategoryId == query.CategoryId.Value);
        if (query.Difficulty.HasValue)
            recipes = recipes.Where(r => r.Difficulty == query.Difficulty.Value);
        if (query.MaxMinutes.HasValue)
            recipes = recipes.Where(r => r.CookingMinutes <= query.MaxMinutes.Value);
        if (query.AuthorId.HasValue)
            recipes = recipes.Where(r => r.AuthorId == query.AuthorId.Value);
        if (query.IngredientId.HasValue)
            recipes = recipes.Where(r => r.RecipeIngredients.Any(ri => ri.IngredientId == query.IngredientId.Value));
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim();
            recipes = recipes.Where(r => r.Title.Contains(s) || (r.Description != null && r.Description.Contains(s)));
        }

        return await recipes
            .OrderByDescending(r => r.CreatedAt).ThenByDescending(r => r.Id)
            .ProjectTo<RecipeListDto>(_mapper.ConfigurationProvider)
            .ToPagedAsync(query.Page, query.PageSize);
    }

    public async Task<RecipeDetailsDto> GetByIdAsync(int id)
    {
        var recipe = await _db.Recipes.AsNoTracking().AsSplitQuery()
            .Include(r => r.Category)
            .Include(r => r.Author).ThenInclude(a => a.ChefProfile)
            .Include(r => r.Steps)
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Comments).ThenInclude(c => c.User)
            .Include(r => r.Favorites)
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new NotFoundException("Recipe not found.");

        var userId = _current.UserId;
        var isFavorite = recipe.Favorites.Any(f => f.UserId == userId);

        return _mapper.Map<RecipeDetailsDto>(recipe, opt => opt.AfterMap((_, dest) => dest.IsFavorite = isFavorite));
    }

    // ---------------- Commands ----------------

    public async Task<RecipeDetailsDto> CreateAsync(CreateRecipeRequest request)
    {
        // One mapping call builds the recipe together with its nested Steps and RecipeIngredients.
        var recipe = _mapper.Map<Recipe>(request);
        recipe.AuthorId = _current.UserId;
        recipe.CreatedAt = DateTime.UtcNow;

        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(recipe.Id);
    }

    public async Task<RecipeDetailsDto> UpdateAsync(int id, UpdateRecipeRequest request)
    {
        var recipe = await _db.Recipes
            .Include(r => r.Steps)
            .Include(r => r.RecipeIngredients)
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new NotFoundException("Recipe not found.");

        EnsureCanModify(recipe.AuthorId);

        await using var tx = await _db.Database.BeginTransactionAsync();

        // Phase 1: remove the old children completely.
        _db.Steps.RemoveRange(recipe.Steps);
        _db.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
        await _db.SaveChangesAsync();

        // Phase 2: update the recipe and add the new children.
        recipe.Title = request.Title;
        recipe.Description = request.Description;
        recipe.CookingMinutes = request.CookingMinutes;
        recipe.Servings = request.Servings;
        recipe.Difficulty = request.Difficulty;
        recipe.CategoryId = request.CategoryId;
        recipe.UpdatedAt = DateTime.UtcNow;

        foreach (var step in _mapper.Map<List<Step>>(request.Steps))
            recipe.Steps.Add(step);
        foreach (var line in _mapper.Map<List<RecipeIngredient>>(request.Ingredients))
            recipe.RecipeIngredients.Add(line);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.Id == id)
                     ?? throw new NotFoundException("Recipe not found.");

        EnsureCanModify(recipe.AuthorId);

        // Steps, ingredient links, comments and favorites are removed by cascade delete.
        _db.Recipes.Remove(recipe);
        await _db.SaveChangesAsync();
    }

    // ---------------- Bonus: "What can I cook?" ----------------

    public async Task<IngredientSearchResultDto> SearchByIngredientsAsync(SearchByIngredientsRequest request)
    {
        var ids = request.IngredientIds.Distinct().ToList();

        var candidates = await _db.Recipes.AsNoTracking()
            .Where(r => r.RecipeIngredients.Count(ri => !ids.Contains(ri.IngredientId)) <= 2)
            .Select(r => new
            {
                r.Id,
                Missing = r.RecipeIngredients
                    .Where(ri => !ids.Contains(ri.IngredientId))
                    .Select(ri => ri.Ingredient.Name)
                    .ToList()
            })
            .ToListAsync();

        var recipeIds = candidates.Select(c => c.Id).ToList();
        var dtos = await _db.Recipes.AsNoTracking()
            .Where(r => recipeIds.Contains(r.Id))
            .ProjectTo<RecipeListDto>(_mapper.ConfigurationProvider)
            .ToDictionaryAsync(d => d.Id);

        var result = new IngredientSearchResultDto();
        foreach (var c in candidates)
        {
            if (c.Missing.Count == 0)
                result.CanCook.Add(dtos[c.Id]);
            else
                result.AlmostCanCook.Add(new RecipeSuggestionDto { Recipe = dtos[c.Id], MissingIngredients = c.Missing });
        }

        return result;
    }

    // ---------------- Ownership ----------------

    private void EnsureCanModify(int authorId)
    {
        if (authorId != _current.UserId && !_current.IsAdmin)
            throw new ForbiddenException("Only the author or an admin can modify this recipe.");
    }
}
