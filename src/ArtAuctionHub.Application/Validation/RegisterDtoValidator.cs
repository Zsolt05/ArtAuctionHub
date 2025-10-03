using ArtAuctionHub.Application.DTOs.Auth;
using FluentValidation;

namespace ArtAuctionHub.Application.Validation
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        private static readonly string[] AllowedRoles = { "Buyer", "Artist" };

        public RegisterDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date is required.")
                .LessThan(DateTime.UtcNow.AddYears(-18))
                .WithMessage("You must be at least 18 years old to register.");

            RuleFor(x => x.Role)
                .Must(r => AllowedRoles.Contains(r))
                .WithMessage("Role must be either 'Buyer' or 'Artist'.");
        }
    }
}