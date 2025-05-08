using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Repository
{
    public class RoomRepository:Repository<Room>, IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Room> UpdateAsync(Room entity)
        {
           var existingRoom = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == entity.Id);
            if (existingRoom != null)
            {
                existingRoom.RoomType = entity.RoomType;
                existingRoom.MaxGuests = entity.MaxGuests;
                existingRoom.WeekdayPrice = entity.WeekdayPrice;
                existingRoom.WeekendPrice = entity.WeekendPrice;
                existingRoom.VillaId = entity.VillaId;
                _context.Rooms.Update(existingRoom);
                await _context.SaveChangesAsync();
                return existingRoom;
            }
            return null;
        }
        public async Task AddRangeAsync(IEnumerable<Room> entities)
        {
            await _context.Rooms.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
        public async Task<VillaDTO> GetRoomsByVilla(int villaId)
        {
           var villa = await _context.Villas
           .Include(v => v.Destination)
           .Include(v => v.Rooms)
             .ThenInclude(r => r.RoomGuestTypes)
                .ThenInclude(rgt => rgt.GuestType)
           .Include(v => v.VillaFacilities)
             .ThenInclude(vf => vf.Facility)
           .Where(v => v.Id == villaId) 
           .Select(v => new VillaDTO
            {
            Id = v.Id,
            Name = v.Name,
            Details = v.Details,
            Occupancy = v.Occupancy,
            Sqft = v.Sqft,
            ImageUrl = v.ImageUrl,
            DestinationName = v.Destination != null ? v.Destination.Name : null,
            Rooms = v.Rooms.Select(r => new RoomDTO
            {
                Id = r.Id,
                RoomType = r.RoomType,
                MaxGuests = r.MaxGuests,
                WeekdayPrice = r.WeekdayPrice,
                WeekendPrice = r.WeekendPrice,
                VillaId = v.Id,
                VillaName = v.Name,
                GuestTypes = r.RoomGuestTypes.Select(rgt => new GuestTypeDTO
                {
                    Id = rgt.GuestType.Id,
                    Name = rgt.GuestType.Name
                }).ToList()
            }).ToList(),
            Facilities = v.VillaFacilities
                .Where(vf => vf.Facility != null)
                .Select(vf => new FacilityDTO
                {
                    Id = vf.Facility.Id,
                    Name = vf.Facility.Name
                })
                .Distinct()
                .ToList()
        })
        .AsNoTracking()
        .FirstOrDefaultAsync(); 

    return villa;
}
















    //     public async Task<IEnumerable<Room>> GetRoomsByVilla(int villaId)
    //     {
    //                var rooms = await _context.Rooms
    //     .Where(r => r.VillaId == villaId)
    //     .Select(r => new Room
    //     {
    //         Id = r.Id,
    //         RoomType = r.RoomType,
    //         MaxGuests = r.MaxGuests,
    //         WeekdayPrice = r.WeekdayPrice,
    //         WeekendPrice = r.WeekendPrice,
    //         VillaId = r.VillaId,
    //         Villa = r.Villa,
    //         RoomGuestTypes = r.RoomGuestTypes.Select(rt => new RoomGuestType
    //         {
    //             GuestType = new GuestType { Id = rt.GuestType.Id, Name = rt.GuestType.Name }
    //         }).ToList()
    //     })
    //     .ToListAsync();

    // return rooms;
    //      }
    }
}