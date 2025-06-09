using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using BookAPI.Contracts.Reviews;
using BookAPI.Identity.Contracts;
using BookAPI.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookAPI.IntegrationTests;

public class ReviewControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly BookDbContext _dbContext;
    private readonly IFixture _fixture;

    public ReviewControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
    }

    [Fact]
    public async Task GetBookReviewsAsync_ReturnsOk()
    {
        var book = await _dbContext.Books.FirstAsync();
        var response = await _client.GetAsync($"/api/review/bookReviews/{book.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reviews = await response.Content.ReadFromJsonAsync<List<ReviewResponse>>();
        reviews.Should().NotBeNull();
    }

    [Fact]
    public async Task GetReviewByIdAsync_ReturnsOk()
    {
        var review = await _dbContext.Reviews.FirstAsync();
        var response = await _client.GetAsync($"/api/review/{review.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateReviewAsync_ReturnsCreated()
    {
        await AuthenticateAsync();

        var content = _fixture.Create<string>();
        var book = await _dbContext.Books.FirstAsync();
        var user = await _dbContext.Users.FirstAsync();
        var request = _fixture.Build<CreateReviewRequest>()
            .With(r => r.BookId, book.Id)
            .With(r => r.UserId, user.Id)
            .With(r => r.Rating, 5)
            .With(r => r.Content, content)
            .Create();

        var response = await _client.PostAsJsonAsync("/api/review", request);

        if (response.StatusCode != HttpStatusCode.Created)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Xunit.Sdk.XunitException(
                $"Expected 201 Created but got {(int)response.StatusCode}.\n\nResponse body:\n{body}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateReviewAsync_ReturnsNoContent()
    {
        await AuthenticateAsync();

        var review = await _dbContext.Reviews.FirstAsync();
        var updateRequest = _fixture.Build<UpdateReviewRequest>()
            .With(r => r.Id, review.Id)
            .With(r => r.BookId, review.BookId)
            .With(r => r.UserId, review.UserId)
            .With(r => r.Rating, review.Rating)
            .Create();

        var response = await _client.PutAsJsonAsync("/api/review", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteReviewAsync_ReturnsNoContent()
    {
        await AuthenticateAsync();

        var review = await _dbContext.Reviews.FirstAsync();
        var response = await _client.DeleteAsync($"/api/review/{review.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task AuthenticateAsync()
    {
        const string password = "MyTestP@ssword!";
        var registerRequest = _fixture.Build<RegisterRequest>()
            .With(r => r.Password, password)
            .Create();

        var regResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        regResponse.EnsureSuccessStatusCode();

        var loginRequest = new LoginRequest(registerRequest.Email, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);
    }
}
