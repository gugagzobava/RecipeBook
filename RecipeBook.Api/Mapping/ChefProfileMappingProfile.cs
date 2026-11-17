namespace RecipeBook.Api.Mapping;

public class ChefProfileMappingProfile : Profile
{
    public ChefProfileMappingProfile()
    {
        CreateMap<ChefProfile, ChefProfileDto>()
            .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.User.DisplayName))
            .ForMember(d => d.RecipesCount, o => o.MapFrom(s => s.User.Recipes.Count()));

        CreateMap<UpsertChefProfileRequest, ChefProfile>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());
    }
}
