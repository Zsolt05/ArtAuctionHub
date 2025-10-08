using AutoMapper;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Application.DTOs.ArtWork;

namespace ArtAuctionHub.Application.MappingProfiles
{
    public class ArtworkProfile : Profile
    {
        public ArtworkProfile()
        {
            CreateMap<Artwork, ArtworkDto>().ReverseMap();
            CreateMap<Artwork, ReadArtworkDto>();
            CreateMap<CreateArtworkForm, Artwork>();
        }
    }
}