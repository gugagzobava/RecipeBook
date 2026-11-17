namespace RecipeBook.Api.Controllers;

[ApiController]
[Authorize(Roles = "User")]
[Route("api/recipes/{recipeId:int}/favorite")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favorites;

    public FavoritesController(IFavoriteService favorites) => _favorites = favorites;

    [HttpPost]
    public async Task<ActionResult<FavoriteDto>> Add(int recipeId)
    {
        var created = await _favorites.AddAsync(recipeId);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(int recipeId)
    {
        await _favorites.RemoveAsync(recipeId);
        return NoContent();
    }
}
