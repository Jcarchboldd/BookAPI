namespace BookAPI.Contracts.Reviews;

public record CreateReviewRequest(
    string Content,
    int Rating,
    Guid BookId,
    Guid UserId);