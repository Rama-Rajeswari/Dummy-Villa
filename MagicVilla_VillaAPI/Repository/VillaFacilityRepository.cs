using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaFacilityRepository:Repository<VillaFacility>, IVillaFacilityRepository
    {
        private readonly ApplicationDbContext _context;

    public VillaFacilityRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<VillaFacility> entities)
    {
        await _context.VillaFacilities.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
    }
}