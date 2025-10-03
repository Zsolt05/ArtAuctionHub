using ArtAuctionHub.Application.DTOs.ArtWork;
using FluentValidation;

namespace ArtAuctionHub.Application.Validation
{
    public class ArtworkDtoValidator : AbstractValidator<ArtworkDto>
    {
        public ArtworkDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

            RuleFor(x => x.CodeName)
                .NotEmpty().WithMessage("CodeName is required.")
                .Matches("^[a-zA-Z0-9_-]{3,64}$")
                .WithMessage("CodeName must be 3–64 characters and contain only letters, numbers, hyphens, or underscores.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(4000).WithMessage("Description cannot exceed 4000 characters.");

            RuleFor(x => x.ImageUrl)
                .Must(url => string.IsNullOrWhiteSpace(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("ImageUrl must be a valid URL.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
        }
    }
}