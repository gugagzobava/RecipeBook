namespace RecipeBook.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IChefProfileService _profiles;

    public UsersController(IChefProfileService profiles) => _profiles = profiles;

    [HttpGet("{id:int}/chef-profile")]
    public async Task<ActionResult<ChefProfileDto>> GetChefProfile(int id) =>
        Ok(await _profiles.GetByUserIdAsync(id));
}
