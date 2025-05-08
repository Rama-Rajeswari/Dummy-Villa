using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Repository
{
     public interface IVillaRepository : IRepository<Villa>
    {
        Task<Villa> UpdateAsync(Villa entity);
         Task<IEnumerable<Villa>> GetVillasByDestinationAsync(string destination);
         Task< Villa> GetVillaByName(string name);
         IQueryable<Villa> GetAll();
         Task<List<VillaDTO>> GetAllVillasWithDetailsAsync();
         Task<List<Villa>> SearchVillasByQueryAsync(string? query);
         
    }
    
}