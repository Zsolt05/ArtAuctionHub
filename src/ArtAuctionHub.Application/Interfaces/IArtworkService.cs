using ArtAuctionHub.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
        Task<Artwork> CreateArtworkAsync(Artwork createArtwork, IFormFile imageFile, ClaimsPrincipal user);

        /// <summary>
        /// Updates an artwork in the database (excluding image update).
        /// </summary>
        Task<Artwork> UpdateArtworkAsync(int id, Artwork dto, ClaimsPrincipal user);

        /// <summary>
        /// Deletes an artwork and its image from the database and file system.
        /// </summary>
        Task DeleteArtworkAsync(int id, ClaimsPrincipal user);

        /// <summary>
        /// Retrieves all artworks from the database.
        /// </summary>
        Task<IEnumerable<Artwork>> GetAllArtworksAsync();

        /// <summary>
        /// Retrieves artworks created by the currently authenticated user from the database.
        /// </summary>
        Task<IEnumerable<Artwork>> GetMyArtworksAsync(int userId);

        #endregion
    }
}