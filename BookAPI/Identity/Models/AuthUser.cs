namespace BookAPI.Identity.Models;

public class AuthUser
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}