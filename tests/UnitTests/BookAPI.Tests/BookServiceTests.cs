using BookAPI.Tests.Utilities;

namespace BookAPI.Tests;

public class BookServiceTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookRepository _bookRepository;
    private readonly BookService _sut;

    public BookServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWork = A.Fake<IUnitOfWork>();
        _bookRepository = A.Fake<IBookRepository>();
        var serviceProvider = A.Fake<IServiceProvider>();

        A.CallTo(() => _unitOfWork.BookRepository).Returns(_bookRepository);

        _sut = new BookService(_unitOfWork, serviceProvider);
    }
    
    [Fact]
    public async Task GetAllBooksAsync_ReturnsMappedBooks()
    {
        // Arrange
        var books = _fixture.CreateMany<Book>(3).ToList();
        A.CallTo(() => _bookRepository.GetAllBooksAsync()).Returns(books);

        // Act
        var result = (await _sut.GetAllBooksAsync()).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveSameCount(books);
        result.Should().AllBeOfType<BookResponse>();
    }
    
    [Fact]
    public async Task GetBookByIdAsync_WithValidId_ReturnsBook()
    {
        // Arrange
        var book = _fixture.Create<Book>();
        A.CallTo(() => _bookRepository.GetBookByIdAsync(book.Id)).Returns(book);

        // Act
        var result = await _sut.GetBookByIdAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(book.Id);
    }
    
    [Fact]
    public async Task GetBookByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var randomId = Guid.NewGuid();
        A.CallTo(() => _bookRepository.GetBookByIdAsync(randomId)).Returns<Book?>(null);

        // Act
        var result = await _sut.GetBookByIdAsync(randomId);

        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task CreateBookAsync_CreatesBookAndReturnsId()
    {
        // Arrange
        var createRequest = _fixture.Create<CreateBookRequest>();

        // Properly typed fake validator
        var fakeValidator = FakeValidator.GetFakeValidator(createRequest);
        
        // Return the correct type
        var serviceProvider = FakeValidator.GetServiceProviderWithValidator(fakeValidator);
        
        var sut = new BookService(_unitOfWork, serviceProvider);

        A.CallTo(() => _bookRepository.CreateBookAsync(A<Book>._))
            .Invokes((Book book) => book.Id = Guid.NewGuid())
            .Returns(Task.CompletedTask);

        // Act
        var result = await sut.CreateBookAsync(createRequest);

        // Assert
        result.Should().NotBeEmpty();
        A.CallTo(() => _bookRepository.CreateBookAsync(A<Book>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitOfWork.SaveAsync()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task UpdateBookAsync_UpdatesBookAndSaves()
    {
        // Arrange
        var updateRequest = _fixture.Create<UpdateBookRequest>();

        // Fake validator for UpdateBookRequest
        var fakeValidator = FakeValidator.GetFakeValidator(updateRequest);
        
        // Fake service provider that returns the fake validator
        var serviceProvider = FakeValidator.GetServiceProviderWithValidator(fakeValidator);
        
        var sut = new BookService(_unitOfWork, serviceProvider);

        // Act
        await sut.UpdateBookAsync(updateRequest);

        // Assert
        A.CallTo(() => _bookRepository.UpdateBookAsync(A<Book>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitOfWork.SaveAsync()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task UpdateBookAsync_BookNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var updateRequest = _fixture.Create<UpdateBookRequest>();

        // Set up a fake validator to pass validation
        var fakeValidator = FakeValidator.GetFakeValidator(updateRequest);

        // Fake the service provider to return the fake validator
        var serviceProvider = FakeValidator.GetServiceProviderWithValidator(fakeValidator);
        
        var sut = new BookService(_unitOfWork, serviceProvider);

        // Simulate repository throwing NotFoundException
        A.CallTo(() => _bookRepository.UpdateBookAsync(A<Book>._))
            .Throws(new NotFoundException(nameof(Book), updateRequest.Id));

        // Act
        var act = async () => await sut.UpdateBookAsync(updateRequest);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task DeleteBookAsync_DeletesBookAndSaves()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        await _sut.DeleteBookAsync(id);

        // Assert
        A.CallTo(() => _bookRepository.DeleteBookAsync(id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitOfWork.SaveAsync()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task DeleteBookAsync_BookNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        A.CallTo(() => _bookRepository.DeleteBookAsync(id))
            .Throws(new NotFoundException(nameof(Book), id));

        // Act
        var act = async () => await _sut.DeleteBookAsync(id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
    
}

