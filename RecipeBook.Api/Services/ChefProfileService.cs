namespace RecipeBook.Api.Services;

public interface IChefProfileService
{
    Task<ChefProfileDto> GetMineAsync();
    Task<ChefProfileDto> UpsertMineAsync(UpsertChefProfileRequest request);
    Task<ChefProfileDto> GetByUserIdAsync(int userId);
}

public class ChefProfileService : IChefProfileService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _current;

    public ChefProfileService(AppDbContext db, IMapper mapper, ICurrentUserService current)
    {
        _db = db;
        _mapper = mapper;
        _current = current;
    }

    public Task<ChefProfileDto> GetMineAsync() => GetByUserIdAsync(_current.UserId);

    public async Task<ChefProfileDto> GetByUserIdAsync(int userId)
    {
        return await _db.ChefProfiles.AsNoTracking()
                   .Where(p => p.UserId == userId)
                   .ProjectTo<ChefProfileDto>(_mapper.ConfigurationProvider)
                   .FirstOrDefaultAsync()
               ?? throw new NotFoundException("Chef profile not found.");
    }

    public async Task<ChefProfileDto> UpsertMineAsync(UpsertChefProfileRequest request)
    {
        var userId = _current.UserId;
        var profile = await _db.ChefProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile is null)
        {
            profile = _mapper.Map<ChefProfile>(request);
            profile.UserId = userId;
            _db.ChefProfiles.Add(profile);
        }
        else
        {
            _mapper.Map(request, profile);
        }

        await _db.SaveChangesAsync();
        return await GetByUserIdAsync(userId);
    }
}
