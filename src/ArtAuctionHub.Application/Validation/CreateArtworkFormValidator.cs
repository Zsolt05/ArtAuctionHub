using ArtAuctionHub.Application.DTOs.ArtWork;
using FluentValidation;

namespace ArtAuctionHub.Application.Validation
{
    public class CreateArtworkFormValidator : AbstractValidator<CreateArtworkForm>
    {
        private static readonly string[] AllowedContentTypes =
            { "image/jpeg", "image/png", "image/jpg" };

        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public CreateArtworkFormValidator()
        {
            Include(new ArtworkDtoValidator()); // Reuse ArtworkDto validation rules

            RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("Image file is required.")
                .Must(f => f.Length > 0).WithMessage("Image file cannot be empty.")
                .Must(f => f.Length <= MaxFileSize).WithMessage("Image file size must not exceed 5 MB.")
                .Must(f => AllowedContentTypes.Contains(f.ContentType))
                .WithMessage("Only JPEG, PNG, WEBP, or GIF files are allowed.");
        }
    }
}
