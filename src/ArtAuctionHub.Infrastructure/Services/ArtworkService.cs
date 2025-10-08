using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using ArtAuctionHub.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// Application service that orchestrates creation, update and deletion of <see cref="Artwork"/>s
    /// while delegating persistence to repositories and committing using <see cref="IUnitOfWork"/>.
    /// <para>
    /// File I/O (image upload/delete) is handled here because it is infrastructure-related but
    /// not part of the domain model itself.
    /// </para>
    /// </summary>
    public class ArtworkService : IArtworkService
    {
        private readonly IArtworkRepository _artworks;
        private readonly IUnitOfWork _uow;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<ArtworkService> _logger;

        /// <summary>
        /// Creates the service with the required dependencies.
        /// </summary>
        /// <param name="artworks">Artwork repository used for data access.</param>
        /// <param name="uow">Unit of work used to commit changes atomically.</param>
        /// <param name="environment">Host environment used to resolve the upload path.</param>
        /// <param name="logger">Logger for artwork actions.</param>
        public ArtworkService(IArtworkRepository artworks, IUnitOfWork uow, IHostEnvironment environment, ILogger<ArtworkService> logger)
        {
            _artworks = artworks;
            _uow = uow;
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new <see cref="Artwork"/> and saves the uploaded image to the file system.
        /// The entity is persisted through the repository and committed via the unit of work.
        /// </summary>
        /// <param name="dto">Incoming data used to populate the artwork.</param>
        /// <param name="imageFile">Uploaded file to be stored under the <c>uploads</c> folder.</param>
        /// <returns>A read model for the created artwork.</returns>
        /// <exception cref="ArgumentException">Thrown when the file is missing or has an invalid extension.</exception>
        public async Task<Artwork> CreateArtworkAsync(Artwork createArtwork, IFormFile imageFile, ClaimsPrincipal user)
        {
            _logger.LogInformation("User {User} is creating a new artwork: {Title}", user.GetUserName(), createArtwork.Title);
            if (imageFile is null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required.", nameof(imageFile));

            var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!validExtensions.Contains(ext))
                throw new ArgumentException("Invalid image format. Allowed: jpg, jpeg, png.", nameof(imageFile));

            createArtwork.ArtistId = user.GetUserId();
            createArtwork.ArtistName = user.GetUserName();


            // Ensure folder exists
            var uploadPath = Path.Combine(_environment.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploadPath);

            // Persist file
            var fileName = createArtwork.CodeName + ext; // assuming CodeName is generated in ctor or by EF
            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = File.Create(filePath))
            {
                await imageFile.CopyToAsync(stream);
            }
            createArtwork.ImageUrl = $"/uploads/{fileName}";

            // Save via repo + commit via UoW
            await _artworks.AddAsync(createArtwork);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Artwork created successfully: {ArtworkId} by user {User}", createArtwork.Id, user.GetUserName());

            return createArtwork;
        }

        /// <summary>
        /// Updates mutable properties of an existing <see cref="Artwork"/>.
        /// Image replacement is not performed by this method.
        /// </summary>
        /// <param name="id">Artwork identifier.</param>
        /// <param name="updateArtwork">Incoming changes.</param>
        /// <returns>The updated read model.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the artwork does not exist.</exception>
        public async Task<Artwork> UpdateArtworkAsync(int id, Artwork updateArtwork, ClaimsPrincipal user)
        {
            _logger.LogInformation("User {User} is updating artwork {ArtworkId}", user.GetUserName(), id);
            var artwork = await _artworks.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Artwork not found.");

            if (artwork.ArtistId != user.GetUserId())
            {
                _logger.LogWarning("User {User} is not authorized to update artwork {ArtworkId}", user.GetUserName(), id);
                throw new UnauthorizedAccessException("You are not authorized to update this artwork.");
            }

            artwork.Title = updateArtwork.Title;
            artwork.Description = updateArtwork.Description;
            artwork.IsAdultOnly = updateArtwork.IsAdultOnly;
            artwork.CategoryId = updateArtwork.CategoryId;

            _artworks.Update(artwork);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Artwork {ArtworkId} updated successfully by user {User}", id, user.GetUserName());

            return artwork;
        }

        /// <summary>
        /// Removes an artwork and its associated image file (if present).
        /// The entity deletion and file I/O are not transactional; consider Outbox/compensation if needed.
        /// </summary>
        /// <param name="id">Artwork identifier.</param>
        public async Task DeleteArtworkAsync(int id, ClaimsPrincipal user)
        {
            _logger.LogInformation("User {User} is deleting artwork {ArtworkId}", user.GetUserName(), id);
            var artwork = await _artworks.GetByIdAsync(id);
            if (artwork is null) return;

            if (artwork.ArtistId != user.GetUserId())
            {
                _logger.LogWarning("User {User} is not authorized to delete artwork {ArtworkId}", user.GetUserName(), id);
                throw new UnauthorizedAccessException("You are not authorized to delete this artwork.");
            }

            var imagePath = Path.Combine(_environment.ContentRootPath, "uploads",
                                         Path.GetFileName(artwork.ImageUrl ?? string.Empty));
            if (File.Exists(imagePath))
                File.Delete(imagePath);

            _artworks.Remove(artwork);
            await _uow.SaveChangesAsync();
            _logger.LogInformation("Artwork {ArtworkId} deleted successfully by user {User}", id, user.GetUserName());
        }

        /// <summary>
        /// Retrieves all artworks as read models. Uses a read-optimized, non-tracked query in the repository.
        /// </summary>
        public async Task<IEnumerable<Artwork>> GetAllArtworksAsync()
        {
            _logger.LogInformation("Retrieving all artworks");
            var entities = await _artworks.GetAllAsync();
            _logger.LogInformation("Returned {Count} artworks", entities.Count);
            return entities;
        }

        /// <summary>
        /// Retrieves artworks for the current user.
        /// </summary>
        /// <param name="userId">The current user's id (to be wired from auth later).</param>
        public async Task<IEnumerable<Artwork>> GetMyArtworksAsync(int userId)
        {
            _logger.LogInformation("Retrieving artworks for user {UserId}", userId);
            var entities = await _artworks.GetByArtistIdAsync(userId);
            _logger.LogInformation("Returned {Count} artworks for user {UserId}", entities.Count, userId);
            return entities;
        }
    }
}