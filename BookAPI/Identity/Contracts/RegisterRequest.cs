namespace BookAPI.Identity.Contracts;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password);