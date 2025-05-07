namespace BookAPI.Identity.Contracts;

public record LoginRequest(
    string Email,
    string Password);