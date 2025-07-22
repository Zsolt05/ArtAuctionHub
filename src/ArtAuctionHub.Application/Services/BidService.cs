using ArtAuctionHub.Application.DTOs.Bid;
using ArtAuctionHub.Application.Interfaces;

namespace ArtAuctionHub.Application.Services
{
    /// <summary>
    /// A mock implementation of IBidService.
    /// </summary>
    public class BidService : IBidService
    {
        public void PlaceBid(BidDto dto)
        {
            Console.WriteLine($"[BidService] Placed bid of {dto.Amount} on auction {dto.AuctionId}");
        }

        public IEnumerable<BidDto> GetMyBids()
        {
            return new[] { new BidDto { AuctionId = 1, Amount = 100 } };
        }
    }
}