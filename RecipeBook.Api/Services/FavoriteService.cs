namespace RecipeBook.Api.Services;

public interface IFavoriteService
{
    Task<FavoriteDto> AddAsync(int recipeId);
    Task RemoveAsync(int recipeId);
    Task<PagedResult<RecipeListDto>> GetMyFavoritesAsync(PaginationQuery paging);
}

public class FavoriteService : IFavoriteService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _current;

    public FavoriteService(AppDbContext db, IMapper mapper, ICurrentUserService current)
    {
        _db = db;
        _mapper = mapper;
        _current = current;
    }

    public async Task<FavoriteDto> AddAsync(int recipeId)
    {
        var me = _current.UserId;

        if (!await _db.Recipes.AnyAsync(r => r.Id == recipeId))
            throw new NotFoundException("Recipe not found.");

        if (await _db.Favorites.AnyAsync(f => f.UserId == me && f.RecipeId == recipeId))
            throw new ConflictException("Recipe is already in your favorites.");

        var favorite = new Favorite { UserId = me, RecipeId = recipeId, AddedAt = DateTime.UtcNow };
        _db.Favorites.Add(favorite);
        await _db.SaveChangesAsync();
        return _mapper.Map<FavoriteDto>(favorite);
    }

    public async Task RemoveAsync(int recipeId)
    {
        var me = _current.UserId;
        var favorite = await _db.Favorites.FirstOrDefaultAsync(f => f.UserId == me && f.RecipeId == recipeId)
                       ?? throw new NotFoundException("Recipe is not in your favorites.");

        _db.Favorites.Remove(favorite);
        await _db.SaveChangesAsync();
    }

    public async Task<PagedResult<RecipeListDto>> GetMyFavoritesAsync(PaginationQuery paging)
    {
        var me = _current.UserId;

        return await _db.Favorites.AsNoTracking()
            .Where(f => f.UserId == me)
            .OrderByDescending(f => f.AddedAt)
            .Select(f => f.Recipe)
            .ProjectTo<RecipeListDto>(_mapper.ConfigurationProvider)
            .ToPagedAsync(paging.Page, paging.PageSize);
    }
}
