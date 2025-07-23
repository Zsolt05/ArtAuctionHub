using ArtAuctionHub.Application.DTOs.ArtWork;
using Microsoft.AspNetCore.Http;

namespace ArtAuctionHub.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for artwork-related operations
    /// </summary>
    public interface IArtworkService
    {
        #region EF Core Methods

        /// <summary>
        /// Creates a new artwork and saves the uploaded image to the file system.
        /// </summary>
        Task<ReadArtworkDto> CreateArtworkAsync(ArtworkDto dto, IFormFile imageFile);

        /// <summary>
        /// Updates an artwork in the database (excluding image update).
        /// </summary>
        Task<ReadArtworkDto> UpdateArtworkAsync(int id, ArtworkDto dto);

        /// <summary>
        /// Deletes an artwork and its image from the database and file system.
        /// </summary>
        Task DeleteArtworkAsync(int id);

        /// <summary>
        /// Retrieves all artworks from the database.
        /// </summary>
        Task<IEnumerable<ReadArtworkDto>> GetAllArtworksAsync();

        /// <summary>
        /// Retrieves artworks created by the currently authenticated user from the database.
        /// </summary>
        Task<IEnumerable<ReadArtworkDto>> GetMyArtworksAsync(int userId);

        #endregion
    }
}