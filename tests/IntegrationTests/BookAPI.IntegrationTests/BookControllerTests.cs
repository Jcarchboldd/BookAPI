using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using BookAPI.Contracts.Books;
using BookAPI.Identity.Contracts;
using BookAPI.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookAPI.IntegrationTests;

public class BookControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly BookDbContext _dbContext;
    private readonly IFixture _fixture;

    public BookControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
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
        await AuthenticateAsync();
        
        // Arrange
        var request = _fixture.Create<CreateBookRequest>();

        // Act
        var response = await _client.PostAsJsonAsync("/api/book", request);
        
        if (response.StatusCode != HttpStatusCode.Created)
        {
            var body = await response.Content.ReadAsStringAsync();
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
        await AuthenticateAsync();
        
        var book = await _dbContext.Books.FirstAsync();

        var updateRequest = _fixture
            .Build<UpdateBookRequest>()
            .With(x => x.Id, book.Id)
            .Create();
        
        var response = await _client.PutAsJsonAsync("/api/book", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task DeleteBookAsync_ReturnsNoContent()
    {
        await AuthenticateAsync();
        
        var book = await _dbContext.Books.FirstAsync();
        var response = await _client.DeleteAsync($"/api/book/{book.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    private async Task AuthenticateAsync()
    {
        const string password = "MyTestP@ssword!";
        var registerRequest = _fixture.Build<RegisterRequest>()
            .With(r => r.Password, password)
            .Create();

        // Register
        var regResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        regResponse.EnsureSuccessStatusCode();

        // Login
        var loginRequest = new LoginRequest(registerRequest.Email, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);
    }
    
    
}

