using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository
{
    public class RoomGuestTypeRepository:Repository<RoomGuestType>, IRoomGuestTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public RoomGuestTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<RoomGuestType> UpdateAsync(RoomGuestType entity)
        {
            _db.RoomGuestTypes.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}