

namespace BookAPI.Tests.Utilities;

public static class FakeValidator
{
    public static IValidator<T> GetFakeValidator<T>(T request)
    {
        var fakeValidator = A.Fake<IValidator<T>>();
        A.CallTo(() => fakeValidator.ValidateAsync(request, A<CancellationToken>._))
            .Returns(new ValidationResult());
        return fakeValidator;
    }
    
    public static IServiceProvider GetServiceProviderWithValidator<T>(IValidator<T> validator)
    {
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(typeof(IValidator<T>)))
            .Returns(validator);
        return serviceProvider;
    }
}