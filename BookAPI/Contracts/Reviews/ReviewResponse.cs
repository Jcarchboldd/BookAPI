namespace BookAPI.Contracts.Reviews;

public record ReviewResponse(
    string Content,
    int Rating,
    string BookTitle,
    string UserName);