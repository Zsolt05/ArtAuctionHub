namespace ArtAuctionHub.Domain.Options
{
    public class AdultContentOptions
    {
        public int MinimumAge { get; set; } = 18;
        public string ForbiddenMessage { get; set; } = "You must be at least 18 years old to favorite adult content.";
    }
}