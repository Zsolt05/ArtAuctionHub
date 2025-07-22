using ArtAuctionHub.Application.DTOs.Auction;
using ArtAuctionHub.Application.Interfaces;

namespace ArtAuctionHub.Application.Services
{
    /// <summary>
    /// A mock implementation of IAuctionService.
    /// </summary>
    public class AuctionService : IAuctionService
    {
        public AuctionDto CreateAuction(AuctionDto dto)
        {
            Console.WriteLine($"[AuctionService] Created auction for artwork {dto.ArtworkId}");
            return dto;
        }

        public AuctionDto UpdateAuction(int id, AuctionDto dto)
        {
            Console.WriteLine($"[AuctionService] Updated auction {id}");
            return dto;
        }

        public void DeleteAuction(int id)
        {
            Console.WriteLine($"[AuctionService] Deleted auction {id}");
        }

        public IEnumerable<AuctionDto> GetActiveAuctions()
        {
            return 
                [
                    new AuctionDto
                    {
                        ArtworkId = 1,
                        StartDate = DateTime.Now.AddDays(-1),
                        EndDate = DateTime.Now.AddDays(5),
                    },
                    new AuctionDto
                    {
                        ArtworkId = 2,
                        StartDate = DateTime.Now.AddDays(-2),
                        EndDate = DateTime.Now.AddDays(3),
                    },
                ];
        }

        public IEnumerable<AuctionDto> GetUserAuctions()
        {
            return 
                [
                    new AuctionDto
                    {
                        ArtworkId = 1,
                        StartDate = DateTime.Now.AddDays(-1),
                        EndDate = DateTime.Now.AddDays(5),
                    },
                    new AuctionDto
                    {
                        ArtworkId = 2,
                        StartDate = DateTime.Now.AddDays(-2),
                        EndDate = DateTime.Now.AddDays(3),
                    },
                ];
        }

        public AuctionDto GetAuctionById(int id)
        {
            Console.WriteLine($"[AuctionService] Retrieved auction {id}");
            return new AuctionDto
            {
                ArtworkId = id,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(5),
            };
        }
    }
}
