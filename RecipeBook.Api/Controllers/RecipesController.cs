namespace RecipeBook.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/recipes")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipes;

    public RecipesController(IRecipeService recipes) => _recipes = recipes;

    [HttpGet]
    public async Task<ActionResult<PagedResult<RecipeListDto>>> GetAll([FromQuery] RecipeQuery query) =>
        Ok(await _recipes.GetListAsync(query));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecipeDetailsDto>> GetById(int id) => Ok(await _recipes.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<RecipeDetailsDto>> Create(CreateRecipeRequest request)
    {
        var created = await _recipes.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RecipeDetailsDto>> Update(int id, UpdateRecipeRequest request) =>
        Ok(await _recipes.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _recipes.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>Bonus: "What can I cook?"</summary>
    [HttpPost("search-by-ingredients")]
    public async Task<ActionResult<IngredientSearchResultDto>> SearchByIngredients(SearchByIngredientsRequest request) =>
        Ok(await _recipes.SearchByIngredientsAsync(request));
}
