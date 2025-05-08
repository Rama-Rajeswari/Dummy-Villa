using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services
{
    public interface IFacilityService
    {
        Task<T>GetAllAsync<T>(string token);
        Task<T>GetAsync<T>(int id,string token);
        Task<T>CreateAsync<T>(FacilityCreateDTO dto,string token);
        Task<T>UpdateAsync<T>(FacilityUpdateDTO dto,string token);
        Task<T>DeleteAsync<T>(int id,string token);
    }
}