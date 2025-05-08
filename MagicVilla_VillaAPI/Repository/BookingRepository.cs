using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Repository
{
    public class BookingRepository:IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsRoomAvailable(int villaId, int? roomId, DateTime checkIn, DateTime checkOut)
        {
             return !await _context.Bookings.AnyAsync(b =>
             b.VillaId == villaId &&
             (roomId == null || b.RoomId == roomId) &&
             ((b.CheckIn < checkOut && b.CheckOut > checkIn) || 
             (b.CheckIn == checkIn) || (b.CheckOut == checkOut))); 
        }
        public async Task<DateTime?> GetNextAvailableDate(int villaId, int? roomId, DateTime checkOut)
        {
            var nextAvailableDate = checkOut;
            var existingBookings = await _context.Bookings
            .Where(b => b.VillaId == villaId && (roomId == null || b.RoomId == roomId))
            .OrderBy(b => b.CheckIn)
            .ToListAsync();
            foreach (var booking in existingBookings)
            {
                if (booking.CheckIn <= nextAvailableDate && booking.CheckOut >= nextAvailableDate)
                {
                     nextAvailableDate = booking.CheckOut.AddDays(1); 
                }
            }

            return nextAvailableDate;
        }
        public async Task<double> GetTotalCost(int villaId, int? roomId, DateTime checkIn, DateTime checkOut)
        {
            double totalCost = 0;

            for (DateTime date = checkIn; date < checkOut; date = date.AddDays(1))
            {
                double dailyRate = 0;

                if (roomId.HasValue)
                {
                    var room = await _context.Rooms.FindAsync(roomId.Value);
                    if (room == null) throw new Exception($"Room with ID {roomId.Value} not found.");

                    dailyRate = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        ? room.WeekendPrice
                        : room.WeekdayPrice;
                }
                else
                {
                    var rooms = await _context.Rooms.Where(r => r.VillaId == villaId).ToListAsync();
                    if (!rooms.Any()) throw new Exception($"No rooms found for Villa ID {villaId}.");

                    foreach (var room in rooms)
                    {
                        dailyRate += (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                            ? room.WeekendPrice
                            : room.WeekdayPrice;
                    }
                }

                totalCost += dailyRate;
            }

            return totalCost;
        }
        public async Task<BookingResponseDTO> BookVillaOrRoom(BookingDTO request)
        {
            if (request.RoomId.HasValue)
            {
               var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId.Value && r.VillaId == request.VillaId);
               if (room == null)
               {
                  return new BookingResponseDTO
                  {
                      Message = "The selected room does not belong to the specified villa."
                  };
               }
            }
            if (!await IsRoomAvailable(request.VillaId, request.RoomId, request.CheckIn, request.CheckOut))
             {
                 DateTime? nextAvailableDate = await GetNextAvailableDate(request.VillaId, request.RoomId, request.CheckOut);
                 var selectedVilla = await _context.Villas.FirstOrDefaultAsync(v => v.Id == request.VillaId);
                 var selectedRoom = request.RoomId.HasValue
                 ? await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId.Value)
                 : null;
                  return new BookingResponseDTO
                  {
                      Message = nextAvailableDate.HasValue
                      ? $"Room/Villa is not available. Next available date is {nextAvailableDate.Value:yyyy-MM-dd}"
                      : "Room/Villa is fully booked for the foreseeable future.",
                      CheckIn = nextAvailableDate ?? DateTime.MinValue,
                      VillaId = selectedVilla?.Id ?? 0,
                      VillaName = selectedVilla?.Name,
                      RoomId = selectedRoom?.Id,
                      RoomType = selectedRoom?.RoomType
                   };
            }
            double totalCost = await GetTotalCost(request.VillaId, request.RoomId, request.CheckIn, request.CheckOut);
            var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == request.VillaId);
            var roomDetails = request.RoomId.HasValue ? await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId.Value) : null;
            if (villa == null)
            {
                return new BookingResponseDTO { Message = "Invalid Villa ID provided." };
            }
             Booking newBooking = new Booking
             {
                Name = request.Name,
                MobileNumber = request.MobileNumber,
                Email = request.Email,
                VillaId = request.VillaId,
                RoomId = request.RoomId,
                Adults = request.Adults,
                Children = request.Children,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                TotalCost = totalCost,
                CreatedAt = DateTime.UtcNow
             };
              _context.Bookings.Add(newBooking);
               await _context.SaveChangesAsync();
                return new BookingResponseDTO
                {
                    Message = "Booking confirmed successfully!",
                    TotalCost = totalCost,
                    VillaId = request.VillaId,
                    VillaName = villa?.Name,
                    RoomId = request.RoomId,
                    RoomType = roomDetails?.RoomType,
                    CheckIn = request.CheckIn,
                    CheckOut = request.CheckOut
                };
        }
        public async Task<List<Booking>> GetAllBookings()
        {
            return await _context.Bookings.Include(b => b.Villa).Include(b => b.Room).ToListAsync();
        }

        public async Task<Booking> GetBookingById(int id)
        {
            return await _context.Bookings.Include(b => b.Villa).Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<string> UpdateBooking(int id, BookingDTO updatedBooking)
        {
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null) return "Booking not found";

            bool isAvailable = await IsRoomAvailable(updatedBooking.VillaId, updatedBooking.RoomId, updatedBooking.CheckIn, updatedBooking.CheckOut);
            if (!isAvailable) return "Selected dates are unavailable.";

            existingBooking.VillaId = updatedBooking.VillaId;
            existingBooking.RoomId = updatedBooking.RoomId;
            existingBooking.CheckIn = updatedBooking.CheckIn;
            existingBooking.CheckOut = updatedBooking.CheckOut;
            existingBooking.Adults = updatedBooking.Adults;
            existingBooking.Children = updatedBooking.Children;
            existingBooking.TotalCost = await GetTotalCost(updatedBooking.VillaId, updatedBooking.RoomId, updatedBooking.CheckIn, updatedBooking.CheckOut);

            _context.Bookings.Update(existingBooking);
            await _context.SaveChangesAsync();
            return "Booking updated successfully!";
        }

        public async Task<string> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return "Booking not found";

            double refundAmount = CalculateRefund(booking);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return $"Booking cancelled. Refund Amount: {refundAmount}";
        }
        private double CalculateRefund(Booking booking)
        {
            int daysBeforeCheckIn = (booking.CheckIn - DateTime.UtcNow).Days;
            double refundPercentage = daysBeforeCheckIn switch
            {
                >= 7 => 0.9,
                >= 3 => 0.5,
                _ => 0.0
            };
            return booking.TotalCost * refundPercentage;
        }
        public async Task<List<DateTime>> GetBookedDatesByRoomIdAsync(int roomId)
        {
            var bookings = await _context.Bookings
        .Where(b => b.RoomId == roomId && b.CheckOut >= DateTime.UtcNow.Date)
        .ToListAsync(); // Fetch bookings first

        var bookedDates = bookings
        .Where(b => b.CheckOut > b.CheckIn) // Ensure valid date range
        .SelectMany(b => Enumerable.Range(0, (b.CheckOut - b.CheckIn).Days + 1)
            .Select(offset => b.CheckIn.AddDays(offset).Date))
        .ToList(); // Now process the dates in-memory

         return bookedDates;

    
        // var bookings = await _context.Bookings
        //      .Where(b => b.RoomId == roomId && b.CheckIn.Date <= DateTime.UtcNow.Date)  // Assuming you have CheckInDate
        //     .Select(b => b.CheckIn.Date)  // or any other date you want to check
        //     .ToListAsync();

        // return bookings;
        }
        public async Task<bool> CheckAvailabilityAsync(int villaId, int? roomId, DateTime checkIn, DateTime checkOut)
        {
        return !await _context.Bookings.AnyAsync(b =>
            b.VillaId == villaId &&
            b.RoomId == roomId &&
            ((checkIn >= b.CheckIn && checkIn < b.CheckOut) || 
             (checkOut > b.CheckIn && checkOut <= b.CheckOut)));
       }
    }
}
    


















       