using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig:Profile
    {
       
        public MappingConfig()
        {
            CreateMap<Villa, VillaDTO>()
        //    .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => src.Rooms)) 
           .ReverseMap();

            CreateMap<Villa,VillaCreateDTO>().ReverseMap();
            CreateMap<Villa,VillaUpdateDTO>().ReverseMap();

            CreateMap<VillaNumber,VillaNumberDTO>().ReverseMap();
            CreateMap<VillaNumber,VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumber,VillaNumberUpdateDTO>().ReverseMap();
            CreateMap<ApplicationUser,UserDTO>().ReverseMap();

           CreateMap<Room, RoomDTO>()
          .ForMember(dest => dest.VillaId, opt => opt.Ignore()) 
          .ReverseMap();
            CreateMap<Room,RoomCreateDTO>().ReverseMap();
            CreateMap<Room,RoomUpdateDTO>().ReverseMap();

            CreateMap<Facility,FacilityDTO>().ReverseMap();
            CreateMap<Facility,FacilityCreateDTO>().ReverseMap();
            CreateMap<Facility,FacilityUpdateDTO>().ReverseMap();

            CreateMap<GuestType,GuestTypeDTO>().ReverseMap();
            CreateMap<GuestType,GuestTypeCreateDTO>().ReverseMap();
            CreateMap<GuestType,GuestTypeUpdateDTO>().ReverseMap();
            
            CreateMap<Destination,DestinationDTO>().ReverseMap();
            CreateMap<Destination,DestinationCreateDTO>().ReverseMap();
            CreateMap<Destination,DestinationUpdateDTO>().ReverseMap();

            CreateMap<RoomPricing, RoomPricingDTO>().ReverseMap(); 
            CreateMap<RoomPricingCreateDTO, RoomPricing>(); 
            CreateMap<RoomPricingUpdateDTO, RoomPricing>();

        }
    }
    
}