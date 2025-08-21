using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence
{
    /// <summary>
    /// The ArtAuctionHubDbContext class manages all database access for the application.
    /// It defines DbSet properties that represent tables in the SQL Server database.
    /// </summary>
    public class ArtAuctionHubDbContext(DbContextOptions<ArtAuctionHubDbContext> options) : DbContext(options)
    {
        /// <summary>
        /// Represents the Users table in the database.
        /// </summary>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// Represents the Roles table in the database.
        /// </summary>
        public DbSet<Role> Roles { get; set; }
        /// <summary>
        /// Represents the UserRoles table in the database, which is a join table for many-to-many relationships between Users and Roles.
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }
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
                entity.HasMany(u => u.Roles)
                      .WithOne(ur => ur.User)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
                entity.HasMany(r => r.Users)
                      .WithOne(ur => ur.Role)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure UserRole entity
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.Roles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
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
                entity.HasMany(a => a.Bids)
                      .WithOne(b => b.Auction)
                      .HasForeignKey(b => b.AuctionId)
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
                      .OnDelete(DeleteBehavior.Cascade);
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
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(uf => uf.Artwork)
                      .WithMany()
                      .HasForeignKey(uf => uf.ArtworkId)
                      .OnDelete(DeleteBehavior.Cascade);
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

            // Seed initial data for Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = RoleNames.Buyer,
                    Email = $"{RoleNames.Buyer}@test.com",
                    PasswordHash = "$2a$12$UmdjxzKaFe4DF.74Y7P/8ug8bie1bSTFC4UHtC7ZW/g3vxFLyc1OS" // Example hash for "Buyer123!"
                },
                new User
                {
                    Id = 2,
                    Username = RoleNames.Artist,
                    Email = $"{RoleNames.Artist}@test.com",
                    PasswordHash = "$2a$12$8ZeSHcGMpER.zMQXp7BFWuMC7AMDIyFGIKCETTFYUFveb5Qz0Jeqq" // Example hash for "Artist123!"
                }
            );

            // Seed initial data for UserRoles
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }, // Buyer
                new UserRole { UserId = 2, RoleId = 2 } // Artist
            );
        }
    }
}