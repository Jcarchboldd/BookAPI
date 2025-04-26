namespace BookAPI.Contracts.Books;

public record CreateBookRequest(
    string Title,
    string Author);