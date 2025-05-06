namespace BookAPI.Identity.Models;

public class AuthUser
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }

    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }
}