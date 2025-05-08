using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services
{
    public interface IBookingService
    {
        Task<T> GetAllAsync<T>(string token);
        Task<T> GetAsync<T>(int id, string token);
        Task<T> CreateAsync<T>(BookingDTO dto, string token);
        Task<T> UpdateAsync<T>(BookingUpdateDTO dto, string token);
        Task<T> DeleteAsync<T>(int id, string token);
       // Task<T> GetBookedDatesAsync<T>(int roomId);
        Task<List<string>> GetBookedDatesAsync(int roomId);
    }
}