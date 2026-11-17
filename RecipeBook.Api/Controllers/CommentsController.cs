namespace RecipeBook.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/recipes/{recipeId:int}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _comments;

    public CommentsController(ICommentService comments) => _comments = comments;

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<CommentDto>> Add(int recipeId, CreateCommentRequest request)
    {
        var created = await _comments.AddAsync(recipeId, request);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpDelete("{commentId:int}")]
    public async Task<IActionResult> Delete(int recipeId, int commentId)
    {
        await _comments.DeleteAsync(recipeId, commentId);
        return NoContent();
    }
}
