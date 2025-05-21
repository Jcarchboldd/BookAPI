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
        persisted.Email     .Should().Be(registerRequest.Email);
        persisted.FirstName  .Should().Be(registerRequest.FirstName);
        persisted.LastName   .Should().Be(registerRequest.LastName);
        persisted.PasswordHash.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();

        // First call succeeds
        (await _client.PostAsJsonAsync("/api/auth/register", request))
            .StatusCode.Should().Be(HttpStatusCode.Created);

        // Act: second call with same email
        var dup = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        dup.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        dup.Content.Headers.ContentType!.MediaType
            .Should().Be("application/problem+json");
    }
    
    
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsOkAndJwt()
    {
        // Arrange: register a user first
        var password = _fixture.Create<string>();

        var registerRequest = _fixture.Build<RegisterRequest>()
            .With(x => x.Password, password)
            .Create();

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var loginRequest = new LoginRequest(
            Email: registerRequest.Email,
            Password: password
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        result.Should().NotBeNull();
        result.Email.Should().Be(loginRequest.Email);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsBadRequest()
    {
        // Arrange: register successfully
        var registerReq = _fixture.Create<RegisterRequest>();
        
        (await _client.PostAsJsonAsync("/api/auth/register", registerReq))
            .StatusCode.Should().Be(HttpStatusCode.Created);

        var badLogin = new LoginRequest(
            Email:    registerReq.Email,
            Password: registerReq.Password + "_wrong"
        );

        // Act
        var resp = await _client.PostAsJsonAsync("/api/auth/login", badLogin);

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        resp.Content.Headers.ContentType!.MediaType
            .Should().Be("application/problem+json");
    }
    
}