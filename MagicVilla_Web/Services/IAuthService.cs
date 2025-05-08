using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services
{
    public interface IAuthService
    {
        Task<T>LoginAsync<T>(LoginRequestDTO objToCreate);
        Task<T>RegisterAsync<T>(RegisterationRequestDTO objToCreate);
        Task<T> ForgotPasswordAsync<T>(ForgotPasswordDTO obj);
    }
}