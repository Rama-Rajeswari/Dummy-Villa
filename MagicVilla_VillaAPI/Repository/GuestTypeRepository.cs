using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository
{
    public class GuestTypeRepository:Repository<GuestType>, IGuestTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public GuestTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<GuestType> UpdateAsync(GuestType entity)
        {
            _context.GuestTypes.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}