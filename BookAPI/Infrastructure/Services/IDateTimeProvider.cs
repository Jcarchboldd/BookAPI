namespace BookAPI.Infrastructure.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}