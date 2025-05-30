namespace BookAPI.Services.Validators;

public static class ValidationHandler
{
    public static async Task ValidateAsync<T>(T request, IServiceProvider serviceProvider)
    {
        var validator = serviceProvider.GetService<IValidator<T>>();
        if (validator is null)
        {
            throw new InternalServerException(
                $"Missing validator for type {typeof(T).Name}",
                $"Validator of type IValidator<{typeof(T).Name}> was not registered in the DI container.");
        }

        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}