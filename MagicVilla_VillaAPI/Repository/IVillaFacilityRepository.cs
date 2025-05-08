using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository
{
    public interface IVillaFacilityRepository:IRepository<VillaFacility>
    {
        Task AddRangeAsync(IEnumerable<VillaFacility> entities);
    }
}