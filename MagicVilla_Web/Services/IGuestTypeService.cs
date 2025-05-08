using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services
{
    public interface IGuestTypeService
    {
        Task<T>GetAllAsync<T>(string token);
        Task<T>GetAsync<T>(int id,string token);
        Task<T>CreateAsync<T>(GuestTypeCreateDTO dto,string token);
        Task<T>UpdateAsync<T>(GuestTypeUpdateDTO dto,string token);
        Task<T>DeleteAsync<T>(int id,string token);
    }
}