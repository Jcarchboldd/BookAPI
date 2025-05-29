namespace BookAPI.Contracts.Reviews;

public record ReviewResponse(
    Guid Id,
    string Content,
    int Rating,
    string BookTitle,
    string UserName);