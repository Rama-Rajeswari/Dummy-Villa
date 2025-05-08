using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class BookingController:Controller
    {
         private readonly IBookingService _bookingService;
         private readonly IVillaService _villaService;
         private readonly IRoomService _roomService;

        public BookingController(IBookingService bookingService,IVillaService villaService,IRoomService roomService)
        {
            _bookingService = bookingService;
            _villaService=villaService;
            _roomService=roomService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _bookingService.GetAllAsync<APIResponse>("");
            if (response != null && response.IsSuccess)
            {
                var bookings = JsonConvert.DeserializeObject<List<BookingDTO>>(response.Result.ToString());
                return View(bookings);
            }
            return View(new List<BookingDTO>());
        }
       [HttpGet]
[Authorize(Roles = "customer")]
public async Task<IActionResult> Create(int villaId, int roomId)
{
    Console.WriteLine($"Creating booking form for VillaId: {villaId}, RoomId: {roomId}");

    var villaResponse = await _villaService.GetAsync<APIResponse>(villaId, "");
    var villaName = villaResponse != null && villaResponse.IsSuccess
        ? JsonConvert.DeserializeObject<VillaDTO>(villaResponse.Result.ToString()).Name
        : "Unknown Villa";

    var roomResponse = await _roomService.GetAsync<APIResponse>(roomId, "");
    var roomData = roomResponse != null && roomResponse.IsSuccess
        ? JsonConvert.DeserializeObject<RoomDTO>(roomResponse.Result.ToString())
        : null;

    var maxGuests = roomData?.MaxGuests ?? 0;
    var roomName = roomData?.RoomType ?? "Unknown Room";

    // Fetch booked dates for the room
    var bookedDates = await _bookingService.GetBookedDatesAsync(roomId);

    // Store in ViewBag
    ViewBag.VillaName = villaName;
    ViewBag.RoomName = roomName;
    ViewBag.MaxGuests = maxGuests;
    ViewBag.BookedDates = bookedDates;

    var booking = new BookingDTO
    {
        VillaId = villaId,
        RoomId = roomId,
        VillaName = villaName,
        RoomName = roomName,
        MaxGuests = maxGuests
    };

    return View(booking);
}









        // public async Task<IActionResult> Create(int villaId, int roomId)
        // {
        //     Console.WriteLine($"Creating booking form for VillaId: {villaId}, RoomId: {roomId}");
        //     ViewBag.VillaId = villaId;
        //     ViewBag.RoomId = roomId;

        //     var villaResponse = await _villaService.GetAsync<APIResponse>(villaId, "");
        //     var villaName = "Unknown Villa";

        //     if (villaResponse != null && villaResponse.IsSuccess)
        //     {
        //         villaName = JsonConvert.DeserializeObject<VillaDTO>(villaResponse.Result.ToString()).Name;
        //     }
        //     else
        //     {
        //         Console.WriteLine("Failed to fetch villa details.");
        //     }

        //     var roomResponse = await _roomService.GetAsync<APIResponse>(roomId, "");
        //     var roomName = "Unknown Room";
        //      int maxGuests = 0;
            

        //     if (roomResponse != null && roomResponse.IsSuccess)
        //     {
        //         Console.WriteLine($"Room API Response: {JsonConvert.SerializeObject(roomResponse)}");
        //          var roomData = JsonConvert.DeserializeObject<RoomDTO>(roomResponse.Result.ToString());
        //          roomName = roomData.RoomType;
        //          maxGuests = roomData.MaxGuests; 
                
        //          Console.WriteLine($"ViewBag.RoomName Set: {ViewBag.RoomName}");
        //          Console.WriteLine($"ViewBag.RoomName: {ViewBag.roomName}");
        //          Console.WriteLine($"Deserialized RoomDTO: {JsonConvert.SerializeObject(roomName)}");
        //          Console.WriteLine($"Room Name Fetched: {roomName}");
        //     }
        //     else
        //     {
        //         Console.WriteLine("Room API returned null.");
        //         Console.WriteLine("Failed to fetch room details.");
        //     }

        //     var booking = new BookingDTO
        //     {
        //         VillaId = villaId,
        //         RoomId = roomId
        //     };

        //     ViewBag.VillaName = villaName;
        //     ViewBag.RoomName = roomName;
        //     ViewBag.MaxGuests = maxGuests;

        //     return View(booking);
            
        // }
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(BookingDTO model)
{
    // Fetch already booked dates for the room
    var bookedDates = await _bookingService.GetBookedDatesAsync(model.RoomId??0);
    Console.WriteLine($"Fetching room details for RoomId: {model.RoomId}");

    if (bookedDates != null && bookedDates.Any())
    {
        // Check if the selected check-in and check-out dates overlap with booked dates
        var selectedDates = Enumerable.Range(0, (model.CheckOut - model.CheckIn).Days + 1)
                                      .Select(offset => model.CheckIn.AddDays(offset).ToString("yyyy-MM-dd"))
                                      .ToList();

        if (selectedDates.Any(date => bookedDates.Contains(date)))
        {
            ModelState.AddModelError("CheckIn", "The selected dates are already booked. Please choose different dates.");
            return View(model);
        }
    }

    try
    {
        var response = await _bookingService.CreateAsync<APIResponse>(model, "");

        if (response != null && response.IsSuccess && response.Result != null)
        {
            var bookingResponse = JsonConvert.DeserializeObject<BookingResponseDTO>(response.Result.ToString());
            return View("BookingConfirmation", bookingResponse);
        }
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", "An internal error occurred while processing the booking.");
    }

    return View(model);
}





















        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create(BookingDTO model)
        // {
        //     Console.WriteLine($"Booking Request - VillaId: {model.VillaId}, RoomId: {model.RoomId}");
        //     if (!ModelState.IsValid)
        //     {
        //         Console.WriteLine("Model validation failed.");
        //         return View(model);
        //     }

        //     Console.WriteLine("Sending booking request: " + JsonConvert.SerializeObject(model));

        //     try
        //     {
        //         var response = await _bookingService.CreateAsync<APIResponse>(model, "");

        //         if (response != null && response.IsSuccess && response.Result != null)
        //         {
        //             Console.WriteLine("Booking successful! Response: " + JsonConvert.SerializeObject(response));
        //             var bookingResponse = JsonConvert.DeserializeObject<BookingResponseDTO>(response.Result.ToString());
        //             return View("BookingConfirmation", bookingResponse);
        //         }
        //         else
        //         {
        //             Console.WriteLine("Booking failed. API Response: " + JsonConvert.SerializeObject(response));
                  
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("Exception during booking: " + ex.Message);
        //         ModelState.AddModelError("", "An internal error occurred.");
        //     }

        //     return View(model);

        // } 
        // [HttpGet]
        // public async Task<IActionResult> GetBookedDates(int roomId)
        // {
        //     var response = await _bookingService.GetBookedDatesAsync<List<string>>(roomId);
        //     return Json(response);
        // } 
    }
}