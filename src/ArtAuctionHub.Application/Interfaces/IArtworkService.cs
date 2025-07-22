using ArtAuctionHub.Application.DTOs.ArtWork;

namespace ArtAuctionHub.Application.Interfaces
{
    /// <summary>
    /// Provides the contract for artwork-related business logic,
    /// including CRUD operations and queries.
    /// </summary>
    public interface IArtworkService
    {
        /// <summary>
        /// Creates a new artwork with the provided details.
        /// </summary>
        /// <param name="dto">Artwork data transfer object containing the artwork details.</param>
        /// <returns>ArtworkDto representing the created artwork.</returns>
        ArtworkDto CreateArtwork(ArtworkDto dto);

        /// <summary>
        /// Updates an existing artwork identified by its ID with the provided details.
        /// </summary>
        /// <param name="id">Artwork ID to identify the artwork to be updated.</param>
        /// <param name="dto">Artwork data transfer object containing the updated artwork details.</param>
        /// <returns>ArtworkDto representing the updated artwork.</returns>
        ArtworkDto UpdateArtwork(int id, ArtworkDto dto);

        /// <summary>
        /// Deletes an artwork identified by its ID.
        /// </summary>
        /// <param name="id">Artwork ID to identify the artwork to be deleted.</param>
        void DeleteArtwork(int id);

        /// <summary>
        /// Retrieves a list of all artworks.
        /// </summary>
        /// <returns>A collection of ArtworkDto representing all artworks.</returns>
        IEnumerable<ArtworkDto> GetAllArtworks();

        /// <summary>
        /// Retrieves an artwork by its ID.
        /// </summary>
        /// <returns>ArtworkDto representing the artwork with the specified ID.</returns>
        IEnumerable<ArtworkDto> GetMyArtworks();
    }
}