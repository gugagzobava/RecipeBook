using AutoMapper;
using RecipeBook.Api.DTOs;
using RecipeBook.Api.Mapping;
using RecipeBook.Api.Validators;
using Xunit;

namespace RecipeBook.Tests;

public class MappingTests
{
    [Fact]
    public void AutoMapper_Configuration_Is_Valid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(RecipeProfile).Assembly));
        config.AssertConfigurationIsValid();
    }
}

public class ValidatorTests
{
    [Fact]
    public void Step_Instruction_Too_Short_Fails()
    {
        var result = new StepValidator().Validate(new StepRequest { Order = 1, Instruction = "ab" });
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(10001)]
    public void Ingredient_Quantity_Out_Of_Range_Fails(double quantity)
    {
        var result = new IngredientLineValidator().Validate(
            new IngredientLineRequest { IngredientId = 1, Quantity = (decimal)quantity });
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Comment_Empty_Text_Fails()
    {
        Assert.False(new CreateCommentRequestValidator().Validate(new CreateCommentRequest { Text = "" }).IsValid);
    }
}
