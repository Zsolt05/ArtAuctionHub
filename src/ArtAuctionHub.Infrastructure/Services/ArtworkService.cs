using ArtAuctionHub.Application.DTOs.ArtWork;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

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

        /// <summary>
        /// Creates the service with the required dependencies.
        /// </summary>
        /// <param name="artworks">Artwork repository used for data access.</param>
        /// <param name="uow">Unit of work used to commit changes atomically.</param>
        /// <param name="environment">Host environment used to resolve the upload path.</param>
        public ArtworkService(IArtworkRepository artworks, IUnitOfWork uow, IHostEnvironment environment)
        {
            _artworks = artworks;
            _uow = uow;
            _environment = environment;
        }

        /// <summary>
        /// Creates a new <see cref="Artwork"/> and saves the uploaded image to the file system.
        /// The entity is persisted through the repository and committed via the unit of work.
        /// </summary>
        /// <param name="dto">Incoming data used to populate the artwork.</param>
        /// <param name="imageFile">Uploaded file to be stored under the <c>uploads</c> folder.</param>
        /// <returns>A read model for the created artwork.</returns>
        /// <exception cref="ArgumentException">Thrown when the file is missing or has an invalid extension.</exception>
        public async Task<ReadArtworkDto> CreateArtworkAsync(ArtworkDto dto, IFormFile imageFile)
        {
            if (imageFile is null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required.", nameof(imageFile));

            var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!validExtensions.Contains(ext))
                throw new ArgumentException("Invalid image format. Allowed: jpg, jpeg, png.", nameof(imageFile));

            var artwork = new Artwork
            {
                Title = dto.Title,
                Description = dto.Description,
                IsAdultOnly = dto.IsAdultOnly,
                CategoryId = dto.CategoryId,
                ArtistId = 2,                 // TODO: replace with the actual logged-in artist id
                ArtistName = "Unknown Artist" // TODO: replace with real artist data
            };

            // Ensure folder exists
            var uploadPath = Path.Combine(_environment.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploadPath);

            // Persist file
            var fileName = artwork.CodeName + ext; // assuming CodeName is generated in ctor or by EF
            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = File.Create(filePath))
            {
                await imageFile.CopyToAsync(stream);
            }
            artwork.ImageUrl = $"/uploads/{fileName}";

            // Save via repo + commit via UoW
            await _artworks.AddAsync(artwork);
            await _uow.SaveChangesAsync();

            return new ReadArtworkDto
            {
                Id = artwork.Id,
                Title = artwork.Title,
                Description = artwork.Description,
                CodeName = artwork.CodeName,
                ImageUrl = artwork.ImageUrl,
                IsAdultOnly = artwork.IsAdultOnly,
                CategoryId = artwork.CategoryId,
                ArtistName = artwork.ArtistName,
                CreatedDate = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Updates mutable properties of an existing <see cref="Artwork"/>.
        /// Image replacement is not performed by this method.
        /// </summary>
        /// <param name="id">Artwork identifier.</param>
        /// <param name="dto">Incoming changes.</param>
        /// <returns>The updated read model.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the artwork does not exist.</exception>
        public async Task<ReadArtworkDto> UpdateArtworkAsync(int id, ArtworkDto dto)
        {
            var artwork = await _artworks.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Artwork not found.");

            artwork.Title = dto.Title;
            artwork.Description = dto.Description;
            artwork.IsAdultOnly = dto.IsAdultOnly;
            artwork.CategoryId = dto.CategoryId;

            _artworks.Update(artwork);
            await _uow.SaveChangesAsync();

            return new ReadArtworkDto
            {
                Id = artwork.Id,
                Title = artwork.Title,
                Description = artwork.Description,
                CodeName = artwork.CodeName,
                ImageUrl = artwork.ImageUrl,
                IsAdultOnly = artwork.IsAdultOnly,
                CategoryId = artwork.CategoryId,
                ArtistName = artwork.ArtistName,
                CreatedDate = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Removes an artwork and its associated image file (if present).
        /// The entity deletion and file I/O are not transactional; consider Outbox/compensation if needed.
        /// </summary>
        /// <param name="id">Artwork identifier.</param>
        public async Task DeleteArtworkAsync(int id)
        {
            var artwork = await _artworks.GetByIdAsync(id);
            if (artwork is null) return;

            var imagePath = Path.Combine(_environment.ContentRootPath, "uploads",
                                         Path.GetFileName(artwork.ImageUrl ?? string.Empty));
            if (File.Exists(imagePath))
                File.Delete(imagePath);

            _artworks.Remove(artwork);
            await _uow.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all artworks as read models. Uses a read-optimized, non-tracked query in the repository.
        /// </summary>
        public async Task<IEnumerable<ReadArtworkDto>> GetAllArtworksAsync()
        {
            var entities = await _artworks.GetAllAsync();
            return entities.Select(a => new ReadArtworkDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                CodeName = a.CodeName,
                ImageUrl = a.ImageUrl,
                IsAdultOnly = a.IsAdultOnly,
                CategoryId = a.CategoryId,
                ArtistName = a.ArtistName,
                CreatedDate = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Retrieves artworks for the current user.
        /// </summary>
        /// <param name="userId">The current user's id (to be wired from auth later).</param>
        public async Task<IEnumerable<ReadArtworkDto>> GetMyArtworksAsync(int userId)
        {
            // TODO: replace with the real artist ↔ user mapping when auth is wired.
            var entities = await _artworks.GetByArtistIdAsync(userId);
            return entities.Select(a => new ReadArtworkDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                CodeName = a.CodeName,
                ImageUrl = a.ImageUrl,
                IsAdultOnly = a.IsAdultOnly,
                CategoryId = a.CategoryId,
                ArtistName = a.ArtistName,
                CreatedDate = DateTime.UtcNow
            });
        }
    }
}