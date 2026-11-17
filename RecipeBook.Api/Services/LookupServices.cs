namespace RecipeBook.Api.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
    Task DeleteAsync(int id);
}

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public Task<List<CategoryDto>> GetAllAsync() =>
        _db.Categories.AsNoTracking().OrderBy(c => c.Name)
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider).ToListAsync();

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        var name = request.Name.Trim();
        if (await _db.Categories.AnyAsync(c => c.Name == name))
            throw new ConflictException("A category with this name already exists.");

        var category = _mapper.Map<Category>(request);
        category.Name = name;
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id) ?? throw new NotFoundException("Category not found.");

        if (await _db.Recipes.AnyAsync(r => r.CategoryId == id))
            throw new BadRequestException("Category has recipes and cannot be deleted.");

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
    }
}

public interface IIngredientService
{
    Task<List<IngredientDto>> GetAllAsync(string? search);
    Task<IngredientDto> CreateAsync(CreateIngredientRequest request);
    Task DeleteAsync(int id);
}

public class IngredientService : IIngredientService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public IngredientService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public Task<List<IngredientDto>> GetAllAsync(string? search)
    {
        var query = _db.Ingredients.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(i => i.Name.Contains(s));
        }

        return query.OrderBy(i => i.Name).ProjectTo<IngredientDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<IngredientDto> CreateAsync(CreateIngredientRequest request)
    {
        var name = request.Name.Trim();
        if (await _db.Ingredients.AnyAsync(i => i.Name == name))
            throw new ConflictException("An ingredient with this name already exists.");

        var ingredient = _mapper.Map<Ingredient>(request);
        ingredient.Name = name;
        _db.Ingredients.Add(ingredient);
        await _db.SaveChangesAsync();
        return _mapper.Map<IngredientDto>(ingredient);
    }

    public async Task DeleteAsync(int id)
    {
        var ingredient = await _db.Ingredients.FindAsync(id) ?? throw new NotFoundException("Ingredient not found.");

        if (await _db.RecipeIngredients.AnyAsync(ri => ri.IngredientId == id))
            throw new BadRequestException("Ingredient is used in recipes and cannot be deleted.");

        _db.Ingredients.Remove(ingredient);
        await _db.SaveChangesAsync();
    }
}
