namespace BookAPI.Infrastructure.Models;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public bool IsVerified { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}