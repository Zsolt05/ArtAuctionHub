using System.Security.Claims;

namespace ArtAuctionHub.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return id != null ? int.Parse(id) : throw new UnauthorizedAccessException("User id not found in token.");
        }
    }
}