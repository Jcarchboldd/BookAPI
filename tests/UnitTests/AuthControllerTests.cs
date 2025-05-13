namespace Xunit.Sdk;

public class AuthControllerTests
{
    private readonly IFixture       _fixture;
    private readonly IAuthService   _fakeAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _fixture        = new Fixture();
        _fakeAuthService = A.Fake<IAuthService>();
        _controller      = new AuthController(_fakeAuthService);
    }
    
    [Fact]
    public async Task RegisterAsync_OnSuccess_Returns201CreatedWithId()
    {
        // Arrange
        var request   = _fixture.Create<RegisterRequest>();
        var newUserId = _fixture.Create<Guid>();

        A.CallTo(() => _fakeAuthService.RegisterAsync(request, A<CancellationToken>._))
            .Returns(Task.FromResult(newUserId));

        // Act
        var result = await _controller.RegisterAsync(request);

        // Assert
        var created = result.Should().BeOfType<CreatedResult>().Subject;
        created.Value.Should().Be(newUserId);
        created.StatusCode.Should().Be(201);

        A.CallTo(() => _fakeAuthService.RegisterAsync(request, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task RegisterAsync_OnDuplicateEmail_ThrowsBadRequestException()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();

        A.CallTo(() => _fakeAuthService.RegisterAsync(request, A<CancellationToken>._))
            .Throws(new BadRequestException("Email is already registered."));

        // Act
        Func<Task> act = () => _controller.RegisterAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Email is already registered.");

        A.CallTo(() => _fakeAuthService.RegisterAsync(request, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task LoginAsync_OnSuccess_Returns200OkWithResponse()
    {
        // Arrange
        var request  = _fixture.Create<LoginRequest>();
        var response = _fixture.Build<AuthenticationResponse>()
            .With(x => x.Email, request.Email)
            .Create();

        A.CallTo(() => _fakeAuthService.AuthenticateAsync(request, A<CancellationToken>._))
            .Returns(Task.FromResult<AuthenticationResponse?>(response));

        // Act
        var result = await _controller.LoginAsync(request);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(response);
        ok.StatusCode.Should().Be(200);

        A.CallTo(() => _fakeAuthService.AuthenticateAsync(request, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task LoginAsync_OnInvalidCredentials_ThrowsBadRequestException()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();

        A.CallTo(() => _fakeAuthService.AuthenticateAsync(request, A<CancellationToken>._))
            .Throws(new BadRequestException("Invalid credentials."));

        // Act
        Func<Task> act = () => _controller.LoginAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Invalid credentials.");

        A.CallTo(() => _fakeAuthService.AuthenticateAsync(request, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}