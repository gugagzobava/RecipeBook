using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RecipeBook.Api.Common;

/// <summary>
/// Runs the FluentValidation validator (sync + async rules) for every action argument that has one,
/// and returns ALL errors at once in the unified error format.
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var failures = new List<ValidationFailure>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator) continue;

            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument), context.HttpContext.RequestAborted);
            failures.AddRange(result.Errors);
        }

        if (failures.Count > 0)
        {
            var errors = failures
                .GroupBy(f => ErrorKeys.Normalize(f.PropertyName))
                .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).Distinct().ToArray());

            context.Result = new BadRequestObjectResult(new ApiError(400, "Validation failed", errors));
            return;
        }

        await next();
    }
}
