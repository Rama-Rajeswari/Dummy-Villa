using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Repository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO>Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO>Register(RegisterationRequestDTO registerationRequestDTO);

        Task<bool> ResetPasswordAsync(ForgotPasswordDTO model);




        // string GenerateRefreshToken(string username);
        // Task<LoginResponseDTO> RefreshToken(TokenRequestDTO tokenRequestDTO);
    }
}