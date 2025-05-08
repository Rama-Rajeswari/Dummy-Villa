using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Repository
{
    public interface IBookingRepository
    {
        Task<bool> IsRoomAvailable(int villaId, int? roomId, DateTime checkIn, DateTime checkOut);
        Task<DateTime?> GetNextAvailableDate(int villaId, int? roomId, DateTime requestedDate);
        Task<double> GetTotalCost(int villaId, int? roomId, DateTime checkIn, DateTime checkOut);
        Task<BookingResponseDTO> BookVillaOrRoom(BookingDTO request);
        Task<List<Booking>> GetAllBookings();
        Task<Booking> GetBookingById(int id);
        Task<string> UpdateBooking(int id, BookingDTO updatedBooking);
        Task<string> CancelBooking(int id);
        Task<List<DateTime>> GetBookedDatesByRoomIdAsync(int roomId);
        Task<bool> CheckAvailabilityAsync(int villaId, int? roomId, DateTime checkIn, DateTime checkOut);
        
    }
}