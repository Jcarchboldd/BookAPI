using BookAPI.Services.Validators.BookValidators;

namespace BookAPI.Tests.ValidatorTests;

public class UpdateBookRequestValidatorTests
{
    private readonly UpdateBookRequestValidator _validator = new();
    private readonly Fixture _fixture = new();
    
    [Theory]
    [InlineData("", "Author", "00000000-0000-0000-0000-000000000000", "Title is required.")]
    [InlineData("Title", "", "00000000-0000-0000-0000-000000000000", "Author is required.")]
    [InlineData("Title", "Author", "", "Id is required.")]
    public void Should_Validate_Missing_Fields(string title, string author, string idString, string expectedError)
    {
        var id = Guid.TryParse(idString, out var parsedId) ? parsedId : Guid.Empty;

        var model = new UpdateBookRequest(id, title, author);

        var result = _validator.TestValidate(model);

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
        else if (id == Guid.Empty)
        {
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage(expectedError);
        }
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Model_Is_Valid()
    {
        var model = _fixture.Create<UpdateBookRequest>();
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}