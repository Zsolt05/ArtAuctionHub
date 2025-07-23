namespace ArtAuctionHub.Application.DTOs.ArtWork
{
    public sealed record ReadArtworkDto : ArtworkDto
    {
        /// <summary>
        /// The unique identifier of the artwork.
        /// </summary>
        public int Id { get; init; }
        /// <summary>
        /// The name of the artist who created the artwork.
        /// </summary>
        public string ArtistName { get; init; } = default!;
        /// <summary>
        /// The date and time when the artwork was created.
        /// </summary>
        public DateTime CreatedDate { get; init; }
    }
}
