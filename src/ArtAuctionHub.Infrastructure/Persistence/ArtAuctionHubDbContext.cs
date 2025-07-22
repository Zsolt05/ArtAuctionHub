using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence
{
    /// <summary>
    /// The ArtAuctionHubDbContext class manages all database access for the application.
    /// It defines DbSet properties that represent tables in the SQL Server database.
    /// </summary>
    public class ArtAuctionHubDbContext : DbContext
    {
        public ArtAuctionHubDbContext(DbContextOptions<ArtAuctionHubDbContext> options)
            : base(options) { }

        /// <summary>
        /// Represents the Users table in the database.
        /// </summary>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// Represents the Roles table in the database.
        /// </summary>
        public DbSet<Role> Roles { get; set; }
        /// <summary>
        /// Represents the Artworks table in the database.
        /// </summary>
        public DbSet<Artwork> Artworks { get; set; }
        /// <summary>
        /// Represents the Auctions table in the database.
        /// </summary>
        public DbSet<Auction> Auctions { get; set; }
        /// <summary>
        /// Represents the Bids table in the database.
        /// </summary>
        public DbSet<Bid> Bids { get; set; }
        /// <summary>
        /// Represents the Categories table in the database.
        /// </summary>
        public DbSet<Category> Categories { get; set; }
        /// <summary>
        /// Represents the UserFavorites table in the database.
        /// </summary>
        public DbSet<UserFavorites> UserFavorites { get; set; }

        /// <summary>
        /// Configures entity mappings and relationships.
        /// This method is called automatically by EF Core.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.HasMany(u => u.Roles).WithMany(r => r.Users);
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
            });

            // Configure Artwork entity
            modelBuilder.Entity<Artwork>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Description).HasMaxLength(500);
                entity.HasOne(a => a.Category)
                      .WithMany(c => c.Artworks)
                      .HasForeignKey(a => a.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(a => a.Artist)
                      .WithMany(u => u.Artworks)
                      .HasForeignKey(a => a.ArtistId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure Auction entity
            modelBuilder.Entity<Auction>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.StartDate).IsRequired();
                entity.Property(a => a.EndDate).IsRequired();
                entity.Property(a => a.StartingPrice).HasPrecision(18, 2).IsRequired();
                entity.HasOne(a => a.Artwork)
                      .WithMany()
                      .HasForeignKey(a => a.ArtworkId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Bid entity
            modelBuilder.Entity<Bid>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Amount).HasPrecision(18, 2).IsRequired();
                entity.Property(b => b.BidDate).IsRequired();
                entity.HasOne(b => b.Auction)
                      .WithMany(a => a.Bids)
                      .HasForeignKey(b => b.AuctionId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(b => b.User)
                      .WithMany()
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
                entity.HasMany(c => c.Artworks)
                      .WithOne(a => a.Category)
                      .HasForeignKey(a => a.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure UserFavorites entity
            modelBuilder.Entity<UserFavorites>(entity =>
            {
                entity.HasKey(uf => new { uf.UserId, uf.ArtworkId });
                entity.HasOne(uf => uf.User)
                      .WithMany(u => u.Favorites)
                      .HasForeignKey(uf => uf.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(uf => uf.Artwork)
                      .WithMany()
                      .HasForeignKey(uf => uf.ArtworkId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Seed initial data for Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = RoleNames.Buyer },
                new Role { Id = 2, Name = RoleNames.Artist }
            );

            // Seed initial data for Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Painting" },
                new Category { Id = 2, Name = "Photography" },
                new Category { Id = 3, Name = "Digital Art" }
            );
        }
    }
}
