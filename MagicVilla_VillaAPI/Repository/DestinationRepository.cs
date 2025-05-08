using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository
{
    public class DestinationRepository:Repository<Destination>, IDestinationRepository
    {
         private readonly ApplicationDbContext _context;

        public DestinationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Destination> UpdateAsync(Destination entity)
        {
            _context.Destinations.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}