using BookAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Tests;

public class BookControllerTests
{
    private readonly IFixture _fixture;
    private readonly IBookService _bookService;
    private readonly BookController _controller;

    public BookControllerTests()
    {
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });

        _bookService = A.Fake<IBookService>();
        _controller = new BookController(_bookService);
    }

    [Fact]
    public async Task GetBooksAsync_ReturnsOkWithBooks()
    {
        var books = _fixture.CreateMany<BookResponse>(3).ToList();
        A.CallTo(() => _bookService.GetAllBooksAsync()).Returns(books);

        var result = await _controller.GetBooksAsync();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(books);
    }

    [Fact]
    public async Task GetBookByIdAsync_BookExists_ReturnsOk()
    {
        var book = _fixture.Create<BookResponse>();
        A.CallTo(() => _bookService.GetBookByIdAsync(book.Id)).Returns(book);

        var result = await _controller.GetBookByIdAsync(book.Id);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(book);
    }

    [Fact]
    public async Task GetBookByIdAsync_BookNotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _bookService.GetBookByIdAsync(id)).Returns((BookResponse?)null);

        var result = await _controller.GetBookByIdAsync(id);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateBookAsync_ReturnsCreatedAtAction()
    {
        var request = _fixture.Create<CreateBookRequest>();
        var newId = Guid.NewGuid();
        A.CallTo(() => _bookService.CreateBookAsync(request)).Returns(newId);

        var result = await _controller.CreateBookAsync(request);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.RouteValues?["id"].Should().Be(newId);
    }

    [Fact]
    public async Task UpdateBookAsync_ReturnsNoContent()
    {
        var updateRequest = _fixture.Create<UpdateBookRequest>();

        var result = await _controller.UpdateBookAsync(updateRequest);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteBookAsync_ReturnsNoContent()
    {
        var id = Guid.NewGuid();

        var result = await _controller.DeleteBookAsync(id);

        result.Should().BeOfType<NoContentResult>();
    }
}