namespace RecipeBook.Api.Services;

public interface ICommentService
{
    Task<CommentDto> AddAsync(int recipeId, CreateCommentRequest request);
    Task DeleteAsync(int recipeId, int commentId);
}

public class CommentService : ICommentService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _current;

    public CommentService(AppDbContext db, IMapper mapper, ICurrentUserService current)
    {
        _db = db;
        _mapper = mapper;
        _current = current;
    }

    public async Task<CommentDto> AddAsync(int recipeId, CreateCommentRequest request)
    {
        if (!await _db.Recipes.AnyAsync(r => r.Id == recipeId))
            throw new NotFoundException("Recipe not found.");

        var comment = new Comment
        {
            RecipeId = recipeId,
            UserId = _current.UserId,
            Text = request.Text.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        return await _db.Comments.AsNoTracking()
            .Where(c => c.Id == comment.Id)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
            .FirstAsync();
    }

    public async Task DeleteAsync(int recipeId, int commentId)
    {
        var comment = await _db.Comments.Include(c => c.Recipe)
                          .FirstOrDefaultAsync(c => c.Id == commentId && c.RecipeId == recipeId)
                      ?? throw new NotFoundException("Comment not found.");

        var me = _current.UserId;
        var allowed = comment.UserId == me || comment.Recipe.AuthorId == me || _current.IsAdmin;
        if (!allowed)
            throw new ForbiddenException("Only the comment author, the recipe author or an admin can delete this comment.");

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
    }
}
