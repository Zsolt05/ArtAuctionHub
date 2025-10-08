namespace ArtAuctionHub.Application.DTOs.ArtWork
{
    /// <summary>
    /// Data Transfer Object (DTO) representing the data of an artwork.
    /// </summary>
    public record ArtworkDto
    {
        /// <summary>
        /// The official title of the artwork.
        /// </summary>
        public string Title { get; set; } = default!;

        /// <summary>
        /// A unique code name or identifier for the artwork.
        /// </summary>
        public string CodeName { get; set; } = default!;

        /// <summary>
        /// A descriptive text providing details about the artwork.
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// A flag indicating if the artwork is restricted to adult audiences (18+ content).
        /// This is used to prevent minors from viewing explicit content.
        /// </summary>
        public bool IsAdultOnly { get; set; }

        /// <summary>
        /// The ID of the category to which this artwork belongs.
        /// </summary>
        public int CategoryId { get; set; }
    }
}