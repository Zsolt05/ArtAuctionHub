using System.Text;
using ArtAuctionHub.Domain.Interfaces;
using ArtAuctionHub.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ArtAuctionHub.Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring JWT authentication
    /// in an ASP.NET Core dependency injection container.
    /// </summary>
    public static class JwtExtensions
    {
        /// <summary>
        /// Registers JWT bearer authentication using configuration values.
        /// </summary>
        /// <param name="services">The service collection to add authentication services to.</param>
        /// <param name="config">The application configuration, expected to contain a <c>Jwt</c> section.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance, for chaining calls.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <c>Jwt:Key</c> is missing in configuration.</exception>
        /// <remarks>
        /// Expects the following keys under <c>Jwt</c> in <c>appsettings.json</c>:
        /// <list type="bullet">
        ///   <item><description><c>Key</c> – symmetric signing key for token validation.</description></item>
        ///   <item><description><c>Issuer</c> – valid issuer name for tokens.</description></item>
        ///   <item><description><c>Audience</c> – valid audience name for tokens.</description></item>
        /// </list>
        /// Configures <see cref="JwtBearerDefaults.AuthenticationScheme"/> with zero clock skew to avoid
        /// tolerances in expiration validation.
        /// </remarks>
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
        {
            var key = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
                opts.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        Console.WriteLine($"JWT failed: {ctx.Exception.GetType().Name} - {ctx.Exception.Message}");
                        if (ctx.Exception is SecurityTokenInvalidIssuerException)
                            Console.WriteLine($"Expected issuer: {opts.TokenValidationParameters.ValidIssuer}");
                        if (ctx.Exception is SecurityTokenInvalidAudienceException)
                            Console.WriteLine($"Expected audience: {opts.TokenValidationParameters.ValidAudience}");
                        if (ctx.Exception is SecurityTokenInvalidSignatureException)
                            Console.WriteLine("Signature invalid (key mismatch?).");
                        if (ctx.Exception is SecurityTokenExpiredException ste)
                            Console.WriteLine($"Expired at: {ste.Expires:o}");
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddScoped<IJwtTokenFactory, JwtTokenFactory>();

            return services;
        }
    }
}