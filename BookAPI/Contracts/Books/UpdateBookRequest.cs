namespace BookAPI.Contracts.Books;

public record UpdateBookRequest(
    Guid Id,
    string Title,
    string Author);