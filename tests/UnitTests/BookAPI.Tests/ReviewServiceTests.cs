using BookAPI.Contracts.Reviews;
using BookAPI.Tests.Utilities;

namespace BookAPI.Tests;

public class ReviewServiceTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReviewRepository _reviewRepository;
    private readonly ReviewService _sut;

    public ReviewServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization {ConfigureMembers = true});
        
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWork = A.Fake<IUnitOfWork>();
        _reviewRepository = A.Fake<IReviewRepository>();
        var serviceProvider = A.Fake<IServiceProvider>();

        A.CallTo(() => _unitOfWork.ReviewRepository).Returns(_reviewRepository);

        _sut = new ReviewService(_unitOfWork, serviceProvider);
    }
    
    [Fact]
    public async Task GetAllReviewsAsync_ReturnsMappedReviews()
    {
        // Arrange
        var reviews = _fixture.CreateMany<Review>(3).ToList();
        A.CallTo(() => _reviewRepository.GetAllReviewsAsync()).Returns(reviews);

        // Act
        var result = (await _sut.GetAllReviewsAsync()).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveSameCount(reviews);
        result.Should().AllBeOfType<ReviewResponse>();
    }
    
    [Fact]
    public async Task GetBookByIdAsync_WithValidId_ReturnsBook()
    {
        // Arrange
        var review = _fixture.Create<Review>();
        A.CallTo(() => _reviewRepository.GetReviewByIdAsync(review.Id)).Returns(review);

        // Act
        var result = await _sut.GetReviewByIdAsync(review.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(review.Id);
    }
    
    [Fact]
    public async Task CreateReviewAsync_CreatesReviewAndReturnsId()
    {
        // Arrange
        var createRequest = _fixture.Create<CreateReviewRequest>();

        // Properly typed fake validator
        var fakeValidator = FakeValidator.GetFakeValidator(createRequest);
        
        // Return the correct type
        var serviceProvider = FakeValidator.GetServiceProviderWithValidator(fakeValidator);
        
        var sut = new ReviewService(_unitOfWork, serviceProvider);

        A.CallTo(() => _reviewRepository.CreateReviewAsync(A<Review>._))
            .Invokes((Review review) => review.Id = Guid.NewGuid())
            .Returns(Task.CompletedTask);

        // Act
        var result = await sut.CreateReviewAsync(createRequest);

        // Assert
        result.Should().NotBeEmpty();
        A.CallTo(() => _reviewRepository.CreateReviewAsync(A<Review>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitOfWork.SaveAsync()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task UpdateReviewAsync_UpdatesReviewAndSaves()
    {
        // Arrange
        var updateRequest = _fixture.Create<UpdateReviewRequest>();

        // Fake validator for UpdateReviewRequest
        var fakeValidator = FakeValidator.GetFakeValidator(updateRequest);
        
        // Fake service provider that returns the fake validator
        var serviceProvider = FakeValidator.GetServiceProviderWithValidator(fakeValidator);
        
        var sut = new ReviewService(_unitOfWork, serviceProvider);

        // Act
        await sut.UpdateReviewAsync(updateRequest);

        // Assert
        A.CallTo(() => _reviewRepository.UpdateReviewAsync(A<Review>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitOfWork.SaveAsync()).MustHaveHappenedOnceExactly();
    }
}