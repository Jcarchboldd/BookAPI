using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using BookAPI.Identity.Contracts;
using BookAPI.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BookAPI.IntegrationTests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient      _client;
    private readonly BookDbContext   _dbContext;
    private readonly IFixture        _fixture;
    
    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client   = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider
            .GetRequiredService<BookDbContext>();

        _fixture = new Fixture()
            .Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
    }
    
    [Fact]
    public async Task RegisterAsync_NewUser_ReturnsCreatedAndPersists()
    {
        // Arrange
        var registerRequest = _fixture.Create<RegisterRequest>();

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert: Created + valid GUID in body
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var newUserId = await response.Content.ReadFromJsonAsync<Guid>();
        newUserId.Should().NotBe(Guid.Empty);

        // Assert: user persisted in DB
        var persisted = await _dbContext.AuthUsers.FindAsync(newUserId);
        persisted.Should().NotBeNull();
        persisted!.Email     .Should().Be(registerRequest.Email);
        persisted.FirstName  .Should().Be(registerRequest.FirstName);
        persisted.LastName   .Should().Be(registerRequest.LastName);
        persisted.PasswordHash.Should().NotBeNullOrWhiteSpace();
    }
}