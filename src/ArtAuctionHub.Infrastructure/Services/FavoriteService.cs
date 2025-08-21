using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;
using ArtAuctionHub.Domain.Interfaces.Repositories;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// Default implementation of <see cref="IFavoriteService"/> that orchestrates
    /// domain repositories to manage a user's favorite artworks. The service is
    /// free of persistence concerns and depends exclusively on repository contracts
    /// and a <see cref="IUnitOfWork"/> for atomic commits.
    /// </summary>
    public sealed class FavoriteService : IFavoriteService
    {
        private readonly IUserRepository _users;
        private readonly IArtworkRepository _artworks;
        private readonly IFavoritesRepository _favorites;
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoriteService"/> class.
        /// </summary>
        /// <param name="users">User repository.</param>
        /// <param name="artworks">Artwork repository.</param>
        /// <param name="favorites">User favorites repository.</param>
        /// <param name="uow">Unit of work to coordinate persistence.</param>
        public FavoriteService(
            IUserRepository users,
            IArtworkRepository artworks,
            IFavoritesRepository favorites,
            IUnitOfWork uow)
        {
            _users = users;
            _artworks = artworks;
            _favorites = favorites;
            _uow = uow;
        }

        /// <summary>
        /// Adds the specified <paramref name="artworkId"/> to the given user's favorites.
        /// Validates existence, prevents duplicates, and persists changes atomically.
        /// </summary>
        /// <param name="artworkId">The artwork identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public async Task AddFavoriteAsync(int artworkId, int userId)
        {
            // Validate inputs against domain state
            if (!await _users.ExistsAsync(userId).ConfigureAwait(false))
                throw new ArgumentException($"User with ID {userId} does not exist.", nameof(userId));

            if (!await _artworks.ExistsAsync(artworkId).ConfigureAwait(false))
                throw new ArgumentException($"Artwork with ID {artworkId} does not exist.", nameof(artworkId));

            // Prevent duplicates
            var existing = await _favorites.GetByUserAndArtworkAsync(userId, artworkId).ConfigureAwait(false);
            if (existing is not null)
                throw new InvalidOperationException($"Artwork with ID {artworkId} is already in favorites for user with ID {userId}.");

            // Create and persist the favorite entry
            var favorite = new UserFavorites { UserId = userId, ArtworkId = artworkId };
            await _favorites.AddAsync(favorite).ConfigureAwait(false);
            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the specified <paramref name="artworkId"/> from the given user's favorites.
        /// Throws if the favorite entry does not exist.
        /// </summary>
        /// <param name="artworkId">The artwork identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public async Task RemoveFavoriteAsync(int artworkId, int userId)
        {
            if (!await _users.ExistsAsync(userId).ConfigureAwait(false))
                throw new ArgumentException($"User with ID {userId} does not exist.", nameof(userId));

            if (!await _artworks.ExistsAsync(artworkId).ConfigureAwait(false))
                throw new ArgumentException($"Artwork with ID {artworkId} does not exist.", nameof(artworkId));

            var existing = await _favorites.GetByUserAndArtworkAsync(userId, artworkId).ConfigureAwait(false)
                          ?? throw new InvalidOperationException($"Artwork with ID {artworkId} is not in favorites for user with ID {userId}.");

            _favorites.Remove(existing);
            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the list of favorite <see cref="Artwork"/> items for the given user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A read-only list of artworks favorited by the user.</returns>
        public async Task<List<Artwork>> GetFavoritesAsync(int userId)
        {
            if (!await _users.ExistsAsync(userId).ConfigureAwait(false))
                throw new ArgumentException($"User with ID {userId} does not exist.", nameof(userId));

            return await _favorites.GetFavoriteArtworksAsync(userId).ConfigureAwait(false);
        }
    }
}