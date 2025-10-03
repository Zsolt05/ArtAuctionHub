using ArtAuctionHub.Application.DTOs.Auction;
using FluentValidation;

namespace ArtAuctionHub.Application.Validation
{
    public class AuctionDtoValidator : AbstractValidator<AuctionDto>
    {
        public AuctionDtoValidator()
        {
            RuleFor(x => x.ArtworkId)
                .GreaterThan(0).WithMessage("ArtworkId must be greater than 0.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .LessThan(x => x.EndDate).WithMessage("Start date must be earlier than end date.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.");

            RuleFor(x => x.StartingPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Starting price cannot be negative.");
        }
    }
}