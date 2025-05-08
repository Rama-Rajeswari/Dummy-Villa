using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository
{
    public class RoomPricingRepository:Repository<RoomPricing>, IRoomPricingRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomPricingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RoomPricing> UpdateAsync(RoomPricing entity)
        {
            _context.RoomPricings.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}