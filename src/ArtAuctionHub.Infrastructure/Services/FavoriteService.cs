using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// A implementation of IFavoriteService interface to manage user favorites.
    /// </summary>
    public class FavoriteService(ArtAuctionHubDbContext dbContext) : IFavoriteService
    {
        public async Task AddFavoriteAsync(int artworkId, int userId)
        {
            // Validate the user and artwork exist in the database
            if (!await dbContext.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ArgumentException($"User with ID {userId} does not exist.");
            }

            if (!await dbContext.Artworks.AnyAsync(a => a.Id == artworkId))
            {
                throw new ArgumentException($"Artwork with ID {artworkId} does not exist.");
            }

            // Check if the artwork is already in the user's favorites
            var existingFavorite = await dbContext.UserFavorites
                .FirstOrDefaultAsync(f => f.ArtworkId == artworkId && f.UserId == userId);

            if (existingFavorite != null)
            {
                throw new InvalidOperationException($"Artwork with ID {artworkId} is already in favorites for user with ID {userId}.");
            }

            // Create a new UserFavorites entity and add it to the context
            var userFavorite = new UserFavorites
            {
                ArtworkId = artworkId,
                UserId = userId
            };

            dbContext.UserFavorites.Add(userFavorite);
            // Save changes to the database
            await dbContext.SaveChangesAsync();
        }

        public Task RemoveFavoriteAsync(int artworkId, int userId)
        {
            // Validate the user and artwork exist in the database
            if (!dbContext.Users.Any(u => u.Id == userId))
            {
                throw new ArgumentException($"User with ID {userId} does not exist.");
            }
            if (!dbContext.Artworks.Any(a => a.Id == artworkId))
            {
                throw new ArgumentException($"Artwork with ID {artworkId} does not exist.");
            }

            // Find the existing favorite entry
            var existingFavorite = dbContext.UserFavorites
                .FirstOrDefault(f => f.ArtworkId == artworkId && f.UserId == userId)
                ?? throw new InvalidOperationException($"Artwork with ID {artworkId} is not in favorites for user with ID {userId}.");

            // Remove the favorite entry from the context
            dbContext.UserFavorites.Remove(existingFavorite);
            // Save changes to the database
            return dbContext.SaveChangesAsync();
        }

        public async Task<List<Artwork>> GetFavoritesAsync(int userId)
        {
            // Validate the user exists in the database
            if (!await dbContext.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ArgumentException($"User with ID {userId} does not exist.");
            }

            // Retrieve the user's favorite artworks
            return await dbContext.UserFavorites
                            .AsNoTracking()
                            .Where(f => f.UserId == userId)
                            .OrderByDescending(f => f.AddedDate)
                            .Include(f => f.Artwork)
                            .Select(f => f.Artwork)
                            .ToListAsync();
        }
    }
}