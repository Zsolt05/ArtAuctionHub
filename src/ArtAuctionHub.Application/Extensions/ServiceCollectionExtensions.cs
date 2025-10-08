using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ArtAuctionHub.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers application services including validators and mappings.
        /// </summary>
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddAppValidators();
            services.AddAppMappings();
            return services;
        }

        /// <summary>
        /// Registers all FluentValidation validators from the executing assembly.
        /// </summary>
        public static IServiceCollection AddAppValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }

        /// <summary>
        /// Registers AutoMapper profiles from the executing assembly.
        /// </summary>
        public static IServiceCollection AddAppMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
