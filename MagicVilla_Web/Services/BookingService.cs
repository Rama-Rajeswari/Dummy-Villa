using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using Newtonsoft.Json;

namespace MagicVilla_Web.Services
{
    public class BookingService:BaseService, IBookingService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string villaUrl;
        public BookingService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }
        public Task<T> CreateAsync<T>(BookingDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = villaUrl + "/api/v1/bookingAPI/book",
                Token = token
            });
        }
        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = villaUrl + "/api/v1/bookingAPI/" + id,
                Token = token
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaUrl + "/api/v1/bookingAPI",
                Token = token
            });
        }
        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaUrl + "/api/v1/bookingAPI/" + id,
                Token = token
            });
        }
        public Task<T> UpdateAsync<T>(BookingUpdateDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = villaUrl + "/api/v1/bookingAPI/" + dto.Id,
                Token = token
            });
        }
        // public Task<T> GetBookedDatesAsync<T>(int roomId)
        // {
        //     return SendAsync<T>(new APIRequest()
        //     {
        //         ApiType = SD.ApiType.GET,
        //         Url = villaUrl + "/api/v1/BookingAPI/GetBookedDates/" + roomId
        //     });
        // }
       public async Task<List<string>> GetBookedDatesAsync(int roomId)
{
    try
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync(villaUrl + $"/api/v1/BookingAPI/GetBookedDates/{roomId}");
        var json = await response.Content.ReadAsStringAsync();

        // ✅ Handle HTTP 500 error responses properly
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error fetching booked dates: {json}");
            return new List<string>(); // Return an empty list to avoid breaking the booking logic
        }

        // ✅ Deserialize API response correctly
        var apiResponse = JsonConvert.DeserializeObject<APIResponse>(json);

        return apiResponse?.Result != null
            ? JsonConvert.DeserializeObject<List<string>>(apiResponse.Result.ToString())
            : new List<string>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception fetching booked dates: {ex.Message}");
    }

    return new List<string>(); // Return an empty list on exception
}

      

         
    }
}