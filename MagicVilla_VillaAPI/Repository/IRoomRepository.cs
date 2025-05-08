using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Repository
{
    public interface IRoomRepository:IRepository<Room>
    {
        Task<Room> UpdateAsync(Room entity);
        Task AddRangeAsync(IEnumerable<Room> entities);
         Task<VillaDTO> GetRoomsByVilla(int villaId);
    }
}