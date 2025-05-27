namespace BookAPI.Contracts.Reviews;

public record UpdateReviewRequest(
    Guid Id,
    string Content,
    int Rating,
    Guid BookId,
    Guid UserId);