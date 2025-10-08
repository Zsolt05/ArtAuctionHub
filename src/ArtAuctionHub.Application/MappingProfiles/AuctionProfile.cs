using AutoMapper;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Application.DTOs.Auction;

namespace ArtAuctionHub.Application.MappingProfiles
{
    public class AuctionProfile : Profile
    {
        public AuctionProfile()
        {
            CreateMap<Auction, AuctionDto>().ReverseMap();
        }
    }
}