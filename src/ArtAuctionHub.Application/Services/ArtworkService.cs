using ArtAuctionHub.Application.DTOs.ArtWork;
using ArtAuctionHub.Application.Interfaces;

namespace ArtAuctionHub.Application.Services
{
    /// <summary>
    /// A mock implementation of IArtworkService.
    /// </summary>
    public class ArtworkService : IArtworkService
    {
        public ArtworkDto CreateArtwork(ArtworkDto dto)
        {
            Console.WriteLine($"[ArtworkService] Created artwork: {dto.Title}");
            return dto;
        }

        public ArtworkDto UpdateArtwork(int id, ArtworkDto dto)
        {
            Console.WriteLine($"[ArtworkService] Updated artwork {id}");
            return dto;
        }

        public void DeleteArtwork(int id)
        {
            Console.WriteLine($"[ArtworkService] Deleted artwork {id}");
        }

        public IEnumerable<ArtworkDto> GetAllArtworks()
        {
            return
            [
                new ArtworkDto
                {
                    Title = "Artwork1",
                    Description = "Description of Artwork1",
                    CategoryId = 1,
                    CodeName = "ART1",
                    ImageUrl = "http://example.com/artwork1.jpg",
                    IsAdultOnly = false
                },
                new ArtworkDto
                {
                    Title = "Artwork2",
                    Description = "Description of Artwork2",
                    CategoryId = 2,
                    CodeName = "ART2",
                    ImageUrl = "http://example.com/artwork2.jpg",
                    IsAdultOnly = true
                }
            ];
        }

        public IEnumerable<ArtworkDto> GetMyArtworks()
        {
            return
            [
                new ArtworkDto
                {
                    Title = "My Artwork1",
                    Description = "Description of My Artwork1",
                    CategoryId = 1,
                    CodeName = "MYART1",
                    ImageUrl = "http://example.com/myartwork1.jpg",
                    IsAdultOnly = false
                }
            ];
        }
    }
}