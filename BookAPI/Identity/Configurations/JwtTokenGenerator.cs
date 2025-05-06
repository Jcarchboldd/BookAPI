using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookAPI.Identity.Models;
using BookAPI.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookAPI.Identity.Configurations;

public class JwtTokenGenerator(IDateTimeProvider dateTimeProvider, IOptions<JwtSettings> jwtOptions)
    : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public string GenerateToken(AuthUser user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()!),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}