using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArtAuctionHub.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// Default implementation of <see cref="IJwtTokenFactory"/> that uses
    /// configuration values to generate and sign JSON Web Tokens (JWT).
    /// </summary>
    /// <remarks>
    /// Reads the following configuration keys from the <c>Jwt</c> section:
    /// <list type="bullet">
    ///   <item><description><c>Key</c> – symmetric signing key (must be long and strong).</description></item>
    ///   <item><description><c>Issuer</c> – the token issuer string.</description></item>
    ///   <item><description><c>Audience</c> – the intended audience for the token.</description></item>
    ///   <item><description><c>AccessTokenMinutes</c> – token lifetime in minutes (default: 60).</description></item>
    /// </list>
    /// </remarks>
    public sealed class JwtTokenFactory(IConfiguration config) : IJwtTokenFactory
    {
        /// <inheritdoc />
        public (string Token, DateTime Expires) CreateToken(int userId, string username, string email, IEnumerable<string> roles)
        {
            var section = config.GetSection("Jwt");
            var key = section["Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
            var issuer = section["Issuer"];
            var audience = section["Audience"];
            var minutes = int.TryParse(section["AccessTokenMinutes"], out var m) ? m : 60;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Email, email)
            };

            // Attach all role claims
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // Prepare signing credentials
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(minutes);

            // Build token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expires);
        }
    }
}