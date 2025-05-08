using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services
{
    public interface IVillaService
    {
        Task<T>GetAllAsync<T>(string token);
        Task<T>GetAsync<T>(int id,string token);
        Task<T>CreateAsync<T>(VillaCreateDTO dto,string token);
        Task<T>UpdateAsync<T>(VillaUpdateDTO dto,string token);
        Task<T>DeleteAsync<T>(int id,string token);
        Task<T> GetVillasByDestinationAsync<T>(string locationName, string token);
        Task<T> SearchVillasAsync<T>(string query, string token);

    }
}