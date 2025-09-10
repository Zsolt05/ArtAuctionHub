using System.Security.Claims;

namespace ArtAuctionHub.Shared.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return id != null ? int.Parse(id) : throw new UnauthorizedAccessException("User id not found in token.");
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            var name = user.FindFirst(ClaimTypes.Name)?.Value;
            return name ?? throw new UnauthorizedAccessException("User name not found in token.");
        }
    }
}