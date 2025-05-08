using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Controller
{
    
    [Route("api/v{version:apiVersion}/BookingAPI")]
    [ApiController]
    public class BookingAPIController:ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingAPIController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingRepository.GetAllBookings();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingRepository.GetBookingById(id);
            return booking == null ? NotFound("Booking not found") : Ok(booking);
        }

        [HttpPost("book")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDTO request)
        {
            var response = await _bookingRepository.BookVillaOrRoom(request);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDTO updatedBooking)
        {
            var response = await _bookingRepository.UpdateBooking(id, updatedBooking);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var response = await _bookingRepository.CancelBooking(id);
            return Ok(response);
        }   
        [HttpGet("GetBookedDates/{roomId}")]
    public async Task<IActionResult> GetBookedDates(int roomId)
    {
        try
        {
            // Call service to get the booked dates for the given room
            var bookedDates = await _bookingRepository.GetBookedDatesByRoomIdAsync(roomId);

            if (bookedDates == null || !bookedDates.Any())
            {
                return Ok(new List<string>()); // No dates booked
            }

            // Return dates in "YYYY-MM-DD" format
            var formattedDates = bookedDates.Select(d => d.ToString("yyyy-MM-dd")).ToList();
            return Ok(formattedDates);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error fetching booked dates: {ex.Message}");
        }
    }
    [HttpGet("check-availability")]
public async Task<IActionResult> CheckAvailability(int villaId, int? roomId, DateTime checkIn, DateTime checkOut)
{
    try
    {
        // Ensure dates are in UTC before passing to repository
        checkIn = DateTime.SpecifyKind(checkIn, DateTimeKind.Utc);
        checkOut = DateTime.SpecifyKind(checkOut, DateTimeKind.Utc);

        // Call the repository method
        bool isAvailable = await _bookingRepository.IsRoomAvailable(villaId, roomId, checkIn, checkOut);

        // Return the response
        return Ok(new AvailabilityResponseDTO 
        { 
            IsAvailable = isAvailable,
            CheckIn = checkIn.ToString("yyyy-MM-dd"),
            CheckOut = checkOut.ToString("yyyy-MM-dd")
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, new { Message = "An error occurred while checking availability." });
    }
    }

    }
}



