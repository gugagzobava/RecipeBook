namespace RecipeBook.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/ingredients")]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientService _ingredients;

    public IngredientsController(IIngredientService ingredients) => _ingredients = ingredients;

    [HttpGet]
    public async Task<ActionResult<List<IngredientDto>>> GetAll([FromQuery] string? search) =>
        Ok(await _ingredients.GetAllAsync(search));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IngredientDto>> Create(CreateIngredientRequest request)
    {
        var created = await _ingredients.CreateAsync(request);
        return Created($"/api/ingredients/{created.Id}", created);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _ingredients.DeleteAsync(id);
        return NoContent();
    }
}
