using System.Net;
using System.Net.Http.Json;
using BookAPI.Contracts.Books;
using BookAPI.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookAPI.IntegrationTests;

public class BookControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly BookDbContext _dbContext;

    public BookControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
    }

    [Fact]
    public async Task GetBooksAsync_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/book");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetBookByIdAsync_ReturnsOk()
    {
        var book = await _dbContext.Books.FirstAsync();
        var response = await _client.GetAsync($"/api/book/{book.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateBookAsync_ReturnsCreated()
    {
        // Arrange
        var request = new CreateBookRequest("Integration Book", "Test Author");

        // Act
        var response = await _client.PostAsJsonAsync("/api/book", request);

        // If you didn’t get 201, dump the full ProblemDetails JSON
        if (response.StatusCode != HttpStatusCode.Created)
        {
            var body = await response.Content.ReadAsStringAsync();
            // Throw an exception so the test output shows the body
            throw new Xunit.Sdk.XunitException(
                $"Expected 201 Created but got {(int)response.StatusCode}.\n\nResponse body:\n{body}"
            );
        }

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task UpdateBookAsync_ReturnsNoContent()
    {
        var book = await _dbContext.Books.FirstAsync();

        var updateRequest = new UpdateBookRequest(book.Id, "Updated Title", "Updated Author");
        var response = await _client.PutAsJsonAsync("/api/book", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task DeleteBookAsync_ReturnsNoContent()
    {
        var book = await _dbContext.Books.FirstAsync();
        var response = await _client.DeleteAsync($"/api/book/{book.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    
}

