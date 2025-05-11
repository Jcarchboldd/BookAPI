using BookAPI.Identity.Configurations;
using BookAPI.Identity.Contracts;
using BookAPI.Identity.Models;
using BookAPI.Identity.Repositories;
using BookAPI.Identity.Services;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Tests;

public class AuthServiceTests
{
    private readonly IFixture                    _fixture;
    private readonly IUnitOfWork                 _uow;
    private readonly IAuthUserRepository         _userRepo;
    private readonly IPasswordHasher<AuthUser>   _hasher;
    private readonly IJwtTokenGenerator          _jwt;
    private readonly AuthService                 _sut;
    
    public AuthServiceTests()
    {
        _fixture = new Fixture();
        
        _userRepo = A.Fake<IAuthUserRepository>();
        _uow      = A.Fake<IUnitOfWork>();
        A.CallTo(() => _uow.AuthUserRepository).Returns(_userRepo);
        
        _hasher = A.Fake<IPasswordHasher<AuthUser>>();
        _jwt    = A.Fake<IJwtTokenGenerator>();
        
        _sut = new AuthService(_uow, _hasher, _jwt);
    }
    
    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_ShouldThrowAndNotPersist()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();
        var existingUser = _fixture
            .Build<AuthUser>()
            .With(u => u.Email, request.Email)
            .Create();

        // Fake the repo to return a user for that email
        A.CallTo(() => _userRepo.GetUserByEmailAsync(request.Email, A<CancellationToken>._))
            .Returns(Task.FromResult<AuthUser?>(existingUser));

        // Act
        Func<Task> act = () => _sut.RegisterAsync(request, CancellationToken.None);

        // Assert
        await act
            .Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Email is already registered.");

        // And ensure we never try to add or save
        A.CallTo(() => _userRepo.AddAsync(A<AuthUser>._, A<CancellationToken>._))
            .MustNotHaveHappened();

        A.CallTo(() => _uow.SaveAsync())
            .MustNotHaveHappened();
    }
    
    [Fact]
    public async Task RegisterAsync_ValidRequest_CreatesUserAndReturnsId()
    {
        // Arrange
        var req      = _fixture.Create<RegisterRequest>();
        var fakeHash = _fixture.Create<string>();

        A.CallTo(() => _userRepo.GetUserByEmailAsync(req.Email, A<CancellationToken>._))
            .Returns(Task.FromResult<AuthUser?>(null));
        A.CallTo(() => _hasher.HashPassword(A<AuthUser>._, req.Password))
            .Returns(fakeHash);

        AuthUser? added = null;
        A.CallTo(() => _userRepo.AddAsync(A<AuthUser>._, A<CancellationToken>._))
            .Invokes((AuthUser u, CancellationToken _) => added = u)
            .Returns(Task.CompletedTask);
        A.CallTo(() => _uow.SaveAsync())
            .Returns(Task.CompletedTask);

        // Act
        var newId = await _sut.RegisterAsync(req, CancellationToken.None);

        // Assert
        newId.Should().Be(added!.Id);
        added!.Email       .Should().Be(req.Email);
        added.FirstName    .Should().Be(req.FirstName);
        added.LastName     .Should().Be(req.LastName);
        added.PasswordHash .Should().Be(fakeHash);

        A.CallTo(() => _userRepo.AddAsync(A<AuthUser>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _uow.SaveAsync())
            .MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task AuthenticateAsync_InvalidPassword_ThrowsBadRequestException()
    {
        // Arrange
        var req  = _fixture.Create<LoginRequest>();
        var user = _fixture.Build<AuthUser>()
            .With(u => u.Email, req.Email)
            .With(u => u.PasswordHash, _fixture.Create<string>())
            .Create();

        A.CallTo(() => _userRepo.GetUserByEmailAsync(req.Email, A<CancellationToken>._))
            .Returns(Task.FromResult<AuthUser?>(user));
        A.CallTo(() => _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        Func<Task> act = () => _sut.AuthenticateAsync(req, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Invalid credentials.");
    }
    
    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsResponse()
    {
        // Arrange
        var req     = _fixture.Create<LoginRequest>();
        var fakeJwt = _fixture.Create<string>();
        var user    = _fixture.Build<AuthUser>()
            .With(u => u.Email,        req.Email)
            .With(u => u.PasswordHash, _fixture.Create<string>())
            .Create();

        A.CallTo(() => _userRepo.GetUserByEmailAsync(req.Email, A<CancellationToken>._))
            .Returns(Task.FromResult<AuthUser?>(user));
        A.CallTo(() => _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password))
            .Returns(PasswordVerificationResult.Success);
        A.CallTo(() => _jwt.GenerateToken(user)).Returns(fakeJwt);

        // Act
        var resp = await _sut.AuthenticateAsync(req, CancellationToken.None);

        // Assert
        resp.Should().NotBeNull();
        resp!.Id        .Should().Be(user.Id.ToString());
        resp.FirstName .Should().Be(user.FirstName);
        resp.LastName  .Should().Be(user.LastName);
        resp.Email     .Should().Be(user.Email);
        resp.Token     .Should().Be(fakeJwt);
    }
    
}

