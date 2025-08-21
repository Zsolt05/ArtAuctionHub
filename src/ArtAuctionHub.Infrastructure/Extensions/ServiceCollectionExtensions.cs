using ArtAuctionHub.Domain.Interfaces;
using ArtAuctionHub.Infrastructure.Persistence;
using ArtAuctionHub.Infrastructure.Persistence.Repositories;
using ArtAuctionHub.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArtAuctionHub.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // Adds Entity Framework Core services to the dependency injection container.
            // This allows us to use EF Core to interact with the database.
            // The DbContext is the main class that interacts with the database.
            // It is configured to use SQL Server with a connection string from the appsettings.json file.
            services.AddDbContext<ArtAuctionHubDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Unit of Work
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            // Open generic repository registration – covers IRepository<T> for ANY T
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            services.Scan(scan => scan
                .FromAssemblies(typeof(ArtAuctionHubDbContext).Assembly)

                // Every class that implements IRepository<T>
                .AddClasses(c => c.AssignableTo(typeof(IRepository<>)), publicOnly: false)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                // Every class whose name ends with "Repository"
                .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")), publicOnly: false)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
            );

            return services;
        }
    }
}