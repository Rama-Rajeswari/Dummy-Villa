using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Repository
{
    public class FacilityRepository:Repository<Facility>, IFacilityRepository
    {
        private readonly ApplicationDbContext _context;

        public FacilityRepository(ApplicationDbContext context) : base(context)
        {
            _context = context ;
        }

        public async Task<Facility> UpdateAsync(Facility entity)
        {
            var existingFacility = await _context.Facilities.FirstOrDefaultAsync(f => f.Id == entity.Id);
            if (existingFacility != null)
            {
            existingFacility.Name = entity.Name;
            _context.Facilities.Update(existingFacility);
            await _context.SaveChangesAsync();
            return existingFacility;
            }
            return null;
        }
    }
}