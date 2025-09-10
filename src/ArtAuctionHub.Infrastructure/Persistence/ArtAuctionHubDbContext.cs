using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence
{
    /// <summary>
    /// The ArtAuctionHubDbContext class manages all database access for the application.
    /// It defines DbSet properties that represent tables in the SQL Server database.
    /// </summary>
    public class ArtAuctionHubDbContext(DbContextOptions<ArtAuctionHubDbContext> options)
        : IdentityDbContext<User, Role, int,
                            IdentityUserClaim<int>, UserRole,
                            IdentityUserLogin<int>, IdentityRoleClaim<int>,
                            IdentityUserToken<int>>(options)
    {
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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Map identity tables to custom table names
            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<UserRole>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            // Configure User entity
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.HasMany(u => u.Roles)
                      .WithOne(ur => ur.User)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Role entity
            builder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
                entity.HasMany(r => r.Users)
                      .WithOne(ur => ur.Role)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure UserRole entity
            builder.Entity<UserRole>(entity =>
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
            builder.Entity<Artwork>(entity =>
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
            builder.Entity<Auction>(entity =>
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
            builder.Entity<Bid>(entity =>
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
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
                entity.HasMany(c => c.Artworks)
                      .WithOne(a => a.Category)
                      .HasForeignKey(a => a.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure UserFavorites entity
            builder.Entity<UserFavorites>(entity =>
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
            builder.Entity<Role>().HasData(
                new Role { Id = 1, Name = RoleNames.Buyer, NormalizedName = RoleNames.Buyer.ToUpper() },
                new Role { Id = 2, Name = RoleNames.Artist, NormalizedName = RoleNames.Artist.ToUpper() }
            );

            // Seed initial data for Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Painting" },
                new Category { Id = 2, Name = "Photography" },
                new Category { Id = 3, Name = "Digital Art" }
            );

            // Seed initial data for Users
            builder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserName = RoleNames.Buyer,
                    Email = RoleNames.Buyer + "@test.com",
                    NormalizedEmail = (RoleNames.Buyer + "@test.com").ToUpper(),
                    PasswordHash = "AQAAAAIAAYagAAAAEKVY84EDLu9Rdv6SuRUOWr20KTY8CBrZvdKOCRUBc7FI3pPGcFneUYf/TvVv2GeT0g==", // Example hash for "Buyer123!"
                    NormalizedUserName = RoleNames.Buyer.ToUpper(),
                    EmailConfirmed = true,
                    ConcurrencyStamp = null,
                    SecurityStamp = null

                },
                new User
                {
                    Id = 2,
                    UserName = RoleNames.Artist,
                    Email = RoleNames.Artist + "@test.com",
                    NormalizedEmail = (RoleNames.Artist + "@test.com").ToUpper(),
                    PasswordHash = "AQAAAAIAAYagAAAAEFMIke77qIxGeBw2N2D15Ab/KXWFfiF+ygvFY96hVYkw//cO0oCwKviR1GAl0Q9CTw==", // Example hash for "Artist123!"
                    NormalizedUserName = RoleNames.Artist.ToUpper(),
                    EmailConfirmed = true,
                    ConcurrencyStamp = null,
                    SecurityStamp = null
                }
            );

            // Seed initial data for UserRoles
            builder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }, // Buyer
                new UserRole { UserId = 2, RoleId = 2 } // Artist
            );
        }
    }
}