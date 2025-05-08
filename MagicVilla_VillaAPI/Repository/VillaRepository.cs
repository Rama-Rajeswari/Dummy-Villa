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
    public class VillaRepository :Repository<Villa>,IVillaRepository
    {
        
        private readonly ApplicationDbContext _context;
        public VillaRepository(ApplicationDbContext context):base(context)
        {
            _context=context;
        }       
        public async Task<Villa> UpdateAsync(Villa entity)
        {
            entity.UpdatedDate=DateTime.UtcNow;
           _context.Villas.Update(entity);
           await _context.SaveChangesAsync();
           return entity;
        }
        public async Task<IEnumerable<Villa>> GetVillasByDestinationAsync(string destination)
        { 
             var villas = await _context.Villas
        .Where(v => v.Destination.Name.ToLower() == destination.ToLower())  
        .ToListAsync();

         return villas;
        }
        public async Task< Villa> GetVillaByName(string name)
       {
        return _context.Villas
        .Include(v => v.Rooms) 
        .Include(v => v.VillaFacilities) 
        .FirstOrDefault(v => v.Name.ToLower() == name.ToLower()); 
       }
       public IQueryable<Villa> GetAll()
       {
         return _context.Villas.Include(v => v.Rooms);
       }
     public async Task<List<VillaDTO>> GetAllVillasWithDetailsAsync()
     {
         return await _context.Villas
        .Include(v => v.Destination)
        .Include(v => v.Rooms)
            .ThenInclude(r => r.RoomGuestTypes)
                .ThenInclude(rgt => rgt.GuestType)
        .Include(v => v.VillaFacilities)
            .ThenInclude(vf => vf.Facility)
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
        .ToListAsync();
       }
      public async Task<List<Villa>> SearchVillasByQueryAsync(string? query)
{
    query = query?.ToLower();

    var villas = await _context.Villas
        .Include(v => v.Destination)
        .Include(v => v.Rooms)
        .Where(v =>
            string.IsNullOrEmpty(query) ||
            v.Name.ToLower().Contains(query) ||
            (v.Destination != null && v.Destination.Name.ToLower().Contains(query)) ||
            v.Rooms.Any(r => r.RoomType.ToLower().Contains(query))
        )
        .ToListAsync();

    return villas;
}




       
       
    }
    
}