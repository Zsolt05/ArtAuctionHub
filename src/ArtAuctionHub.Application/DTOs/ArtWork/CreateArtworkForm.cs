using Microsoft.AspNetCore.Http;

namespace ArtAuctionHub.Application.DTOs.ArtWork
{
    /// <summary>
    /// Request model for creating artwork with image upload.
    /// </summary>
    public record CreateArtworkForm : ArtworkDto
    {
        public IFormFile ImageFile { get; set; } = default!;
    }
}