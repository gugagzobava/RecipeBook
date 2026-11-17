namespace RecipeBook.Api.Mapping;

public class RecipeProfile : Profile
{
    public RecipeProfile()
    {
        // ---- Entity -> DTO ----
        CreateMap<Recipe, RecipeListDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.DisplayName))
            .ForMember(d => d.FavoritesCount, o => o.MapFrom(s => s.Favorites.Count()))
            .ForMember(d => d.CommentsCount, o => o.MapFrom(s => s.Comments.Count()));

        CreateMap<Step, StepDto>();

        CreateMap<RecipeIngredient, IngredientLineDto>()
            .ForMember(d => d.IngredientName, o => o.MapFrom(s => s.Ingredient.Name));

        CreateMap<Recipe, RecipeDetailsDto>()
            .IncludeBase<Recipe, RecipeListDto>()
            .ForMember(d => d.Steps, o => o.MapFrom(s => s.Steps.OrderBy(x => x.Order)))
            .ForMember(d => d.Ingredients, o => o.MapFrom(s => s.RecipeIngredients))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments.OrderBy(c => c.CreatedAt)))
            .ForMember(d => d.AuthorSpeciality,
                o => o.MapFrom(s => s.Author.ChefProfile != null ? s.Author.ChefProfile.Speciality : null))
            .ForMember(d => d.IsFavorite, o => o.Ignore()); // filled in the service (depends on current user)

        // ---- Request -> Entity (nested collections in one go) ----
        CreateMap<StepRequest, Step>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.RecipeId, o => o.Ignore())
            .ForMember(d => d.Recipe, o => o.Ignore());

        CreateMap<IngredientLineRequest, RecipeIngredient>()
            .ForMember(d => d.RecipeId, o => o.Ignore())
            .ForMember(d => d.Recipe, o => o.Ignore())
            .ForMember(d => d.Ingredient, o => o.Ignore());

        CreateMap<CreateRecipeRequest, Recipe>()
            .ForMember(d => d.RecipeIngredients, o => o.MapFrom(s => s.Ingredients))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.AuthorId, o => o.Ignore())
            .ForMember(d => d.Author, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.Comments, o => o.Ignore())
            .ForMember(d => d.Favorites, o => o.Ignore());
    }
}
