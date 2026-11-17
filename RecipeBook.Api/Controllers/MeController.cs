namespace RecipeBook.Api.Controllers;

[ApiController]
[Authorize(Roles = "User")]
[Route("api/me")]
public class MeController : ControllerBase
{
    private readonly IChefProfileService _profiles;
    private readonly IRecipeService _recipes;
    private readonly IFavoriteService _favorites;

    public MeController(IChefProfileService profiles, IRecipeService recipes, IFavoriteService favorites)
    {
        _profiles = profiles;
        _recipes = recipes;
        _favorites = favorites;
    }

    [HttpGet("chef-profile")]
    public async Task<ActionResult<ChefProfileDto>> GetChefProfile() => Ok(await _profiles.GetMineAsync());

    [HttpPut("chef-profile")]
    public async Task<ActionResult<ChefProfileDto>> UpsertChefProfile(UpsertChefProfileRequest request) =>
        Ok(await _profiles.UpsertMineAsync(request));

    [HttpGet("favorites")]
    public async Task<ActionResult<PagedResult<RecipeListDto>>> GetFavorites([FromQuery] PaginationQuery paging) =>
        Ok(await _favorites.GetMyFavoritesAsync(paging));

    [HttpGet("recipes")]
    public async Task<ActionResult<PagedResult<RecipeListDto>>> GetRecipes([FromQuery] PaginationQuery paging) =>
        Ok(await _recipes.GetMyRecipesAsync(paging));
}
