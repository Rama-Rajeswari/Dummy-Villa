using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_Web.Models.Dto;


namespace MagicVilla_Web
{
    public class MappingConfig:Profile
    {
       
        public MappingConfig()
        {
            CreateMap<VillaDTO,VillaCreateDTO>().ReverseMap();
            CreateMap<VillaDTO,VillaUpdateDTO>().ReverseMap();
            
            
            CreateMap<VillaNumberDTO,VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumberDTO,VillaNumberUpdateDTO>().ReverseMap();

            CreateMap<DestinationDTO,DestinationCreateDTO>().ReverseMap();
            CreateMap<DestinationDTO,DestinationUpdateDTO>().ReverseMap();

            CreateMap<RoomDTO,RoomCreateDTO>().ReverseMap();
            CreateMap<RoomDTO,RoomUpdateDTO>().ReverseMap();

            CreateMap<FacilityDTO,FacilityCreateDTO>().ReverseMap();
            CreateMap<FacilityDTO,FacilityUpdateDTO>().ReverseMap();

            CreateMap<GuestTypeDTO,GuestTypeCreateDTO>().ReverseMap();
            CreateMap<GuestTypeDTO,GuestTypeUpdateDTO>().ReverseMap();
        }
    }
    
}