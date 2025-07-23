using ArtAuctionHub.Application.DTOs.ArtWork;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ArtAuctionHub.Infrastructure.Services
{
    public class ArtworkService : IArtworkService
    {
        private readonly ArtAuctionHubDbContext _context;
        private readonly IHostEnvironment _environment;

        public ArtworkService(ArtAuctionHubDbContext context, IHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /// <summary>
        /// Creates a new artwork and saves the uploaded image to the file system.
        /// </summary>
        public async Task<ReadArtworkDto> CreateArtworkAsync(ArtworkDto dto, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required.");

            // Check valid file extension
            var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(imageFile.FileName).ToLower();
            if (!validExtensions.Contains(ext))
                throw new ArgumentException("Invalid image format. Allowed: jpg, jpeg, png.");

            // Create entity
            var artwork = new Artwork
            {
                Title = dto.Title,
                Description = dto.Description,
                IsAdultOnly = dto.IsAdultOnly,
                CategoryId = dto.CategoryId,
                ArtistId = 2, // Future implementation will replace this with actual artist ID
                ArtistName = "Unknown Artist", // Future implementation will replace this with actual artist data
            };

            // Ensure directory exists
            var uploadPath = Path.Combine(_environment.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploadPath);

            // Save file
            var fileName = artwork.CodeName + ext;
            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            artwork.ImageUrl = $"/uploads/{fileName}";

            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();

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
                CreatedDate = DateTime.UtcNow,
            };
        }

        /// <summary>
        /// Updates an artwork (without image).
        /// </summary>
        public async Task<ReadArtworkDto> UpdateArtworkAsync(int id, ArtworkDto dto)
        {
            var artwork = await _context.Artworks.FindAsync(id)
                ?? throw new KeyNotFoundException("Artwork not found.");
            artwork.Title = dto.Title;
            artwork.Description = dto.Description;
            artwork.IsAdultOnly = dto.IsAdultOnly;
            artwork.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

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
        /// Deletes an artwork and its image.
        /// </summary>
        public async Task DeleteArtworkAsync(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null) return;

            // Delete image file
            var imagePath = Path.Combine(_environment.ContentRootPath, "uploads", Path.GetFileName(artwork.ImageUrl));
            if (File.Exists(imagePath))
                File.Delete(imagePath);

            _context.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all artworks from DB.
        /// </summary>
        public async Task<IEnumerable<ReadArtworkDto>> GetAllArtworksAsync()
        {
            return await _context.Artworks
                .AsNoTracking()
                .Select(a => new ReadArtworkDto
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
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves artworks created by the current user.
        /// </summary>
        public async Task<IEnumerable<ReadArtworkDto>> GetMyArtworksAsync(int userId)
        {
            // Future implementation:
            // 1. Replace with actual user ID logic.
            // 2. Filter artworks by the current user's ID.

            return await GetAllArtworksAsync();
        }
    }
}