using BookAPI.Services.Validators.BookValidators;
using FluentValidation.TestHelper;

namespace BookAPI.Tests;

public class CreateBookRequestValidatorTests
{
    private readonly CreateBookRequestValidator _validator = new();
    private readonly Fixture _fixture = new();
    
    [Theory]
    [InlineData(null, "Author", "Title is required.")]
    [InlineData("", "Author", "Title is required.")]
    [InlineData("Book", null, "Author is required.")]
    [InlineData("Book", "", "Author is required.")]
    public void Should_Validate_Multiple_Cases(string title, string author, string expectedError)
    {
        var request = new CreateBookRequest(title ?? "", author ?? "");

        var result = _validator.TestValidate(request);

        if (string.IsNullOrWhiteSpace(title))
        {
            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage(expectedError);
        }
        else if (string.IsNullOrWhiteSpace(author))
        {
            result.ShouldHaveValidationErrorFor(x => x.Author)
                .WithErrorMessage(expectedError);
        }
    }
    
    [Fact]
    public void Should_Not_Have_Errors_When_Model_Is_Valid()
    {
        var request = _fixture.Create<CreateBookRequest>();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}