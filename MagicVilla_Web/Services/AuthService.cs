using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services
{
    public class AuthService:BaseService,IAuthService
    {       
        private readonly IHttpClientFactory _clientFactory;
        private string villaUrl;
        public AuthService(IHttpClientFactory clientFactory,IConfiguration configuration):base(clientFactory)
        {
            _clientFactory=clientFactory;
            villaUrl=configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }
        public Task<T> LoginAsync<T>(LoginRequestDTO obj)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType=SD.ApiType.POST,
                Data=obj,
                Url=villaUrl+"/api/v1/UsersAuth/login"

            });
        }
        public Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
        {
           return SendAsync<T>(new APIRequest()
            {
                ApiType=SD.ApiType.POST,
                Data=obj,
                Url=villaUrl+"/api/v1/UsersAuth/register"

            });
        }
        public Task<T> ForgotPasswordAsync<T>(ForgotPasswordDTO obj)
        {
             return SendAsync<T>(new APIRequest()
             {
                 ApiType = SD.ApiType.POST,
                 Data = obj,
                 Url = villaUrl + "/api/v1/UsersAuth/resetpassword" // 👈 Your API endpoint
             });
        }

    }
}