using ArtAuctionHub.Application.DTOs.Bid;
using FluentValidation;

namespace ArtAuctionHub.Application.Validation
{
    public class BidDtoValidator : AbstractValidator<BidDto>
    {
        public BidDtoValidator()
        {
            RuleFor(x => x.AuctionId)
                .GreaterThan(0).WithMessage("AuctionId must be greater than 0.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Bid amount must be greater than 0.");
        }
    }
}