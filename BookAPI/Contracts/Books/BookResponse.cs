namespace BookAPI.Contracts.Books;

public record BookResponse(
    Guid Id,
    string Title,
    string Author,
    IEnumerable<ReviewResponse> Reviews);