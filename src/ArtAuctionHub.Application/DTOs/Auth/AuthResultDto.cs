namespace ArtAuctionHub.Application.DTOs.Auth
{
    public sealed record AuthResultDto(string AccessToken, DateTime Expires);
}
