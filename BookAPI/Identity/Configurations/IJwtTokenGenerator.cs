using BookAPI.Identity.Models;

namespace BookAPI.Identity.Configurations;

public interface IJwtTokenGenerator
{
    string GenerateToken(AuthUser user);
}