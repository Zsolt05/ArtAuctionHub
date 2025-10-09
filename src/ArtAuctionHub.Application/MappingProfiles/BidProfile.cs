using AutoMapper;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Application.DTOs.Bid;

namespace ArtAuctionHub.Application.MappingProfiles
{
    public class BidProfile : Profile
    {
        public BidProfile()
        {
            CreateMap<BidDto, Bid>();
            CreateMap<Bid, ReadBidDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.BidDate, opt => opt.MapFrom(src => src.BidDate));
        }
    }
}