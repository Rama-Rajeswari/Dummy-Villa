using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services
{
    public interface IRoomService
    {
        Task<T>GetAllAsync<T>(string token);
        Task<T>GetAsync<T>(int id,string token);
        Task<T>CreateAsync<T>(RoomCreateDTO dto,string token);
        Task<T>UpdateAsync<T>(RoomUpdateDTO dto,string token);
        Task<T>DeleteAsync<T>(int id,string token);
        Task<T> GetRoomsByVillaAsync<T>(int villaId, string token);
       
    }
}