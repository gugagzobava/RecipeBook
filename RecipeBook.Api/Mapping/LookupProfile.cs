namespace RecipeBook.Api.Mapping;

public class LookupProfile : Profile
{
    public LookupProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryRequest, Category>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Recipes, o => o.Ignore());

        CreateMap<Ingredient, IngredientDto>();
        CreateMap<CreateIngredientRequest, Ingredient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.RecipeIngredients, o => o.Ignore());
    }
}
