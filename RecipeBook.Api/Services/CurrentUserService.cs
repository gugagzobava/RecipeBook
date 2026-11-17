using System.Security.Claims;

namespace RecipeBook.Api.Services;

public interface ICurrentUserService
{
    int UserId { get; }
    bool IsAdmin { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor http) => _http = http;

    private ClaimsPrincipal? Principal => _http.HttpContext?.User;

    public int UserId
    {
        get
        {
            var sub = Principal?.FindFirstValue("sub") ?? Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(sub, out var id) ? id : throw new UnauthorizedException();
        }
    }

    public bool IsAdmin => Principal?.IsInRole(nameof(UserRole.Admin)) ?? false;
}
