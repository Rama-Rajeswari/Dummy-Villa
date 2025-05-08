using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class LoginResponseDTO
    {
        public UserDTO User{get;set;}
        public string Token{get;set;}
       // public string RefreshToken { get; set; }  // <-- Add this
    }
}