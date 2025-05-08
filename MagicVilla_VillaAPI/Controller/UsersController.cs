using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MagicVilla_VillaAPI.Controller
{
    [Route("api/v{version:apiVersion}/UsersAuth")]
    [ApiController] 
     //[ApiVersion("1.0")]
     [ApiVersionNeutral]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        protected APIResponse _response;
        public UsersController(IUserRepository userRepo)
        {
            _userRepo=userRepo;
            this._response=new();
        }
        [HttpPost("login")]
        public async  Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var LoginResponse=await _userRepo.Login(model);
            if(LoginResponse.User==null||string.IsNullOrEmpty(LoginResponse.Token))
            {
                _response.StatusCode=HttpStatusCode.BadRequest;
                _response.IsSuccess=false;
                _response.ErrorMessages.Add("UserName or Password Incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode=HttpStatusCode.OK;
            _response.IsSuccess=true;
            _response.Result=LoginResponse ;
            return Ok(_response);                                                     
        }
        [HttpPost("register")]
        public async  Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            bool ifUserNameUnique=_userRepo.IsUniqueUser(model.UserName);
            if(!ifUserNameUnique)
            {
                _response.StatusCode=HttpStatusCode.BadRequest;
                _response.IsSuccess=false;
                _response.ErrorMessages.Add("UserName already exists");
                return BadRequest(_response);
            }
            var user=await _userRepo.Register(model);
            if(user==null)
            {
                _response.StatusCode=HttpStatusCode.BadRequest;
                _response.IsSuccess=false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);                                                    
            }
            _response.StatusCode=HttpStatusCode.OK;
            _response.IsSuccess=true;
            return Ok(_response);

        }


[HttpPost("ResetPassword")]
public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordDTO model)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var result = await _userRepo.ResetPasswordAsync(model);
    if (!result)
    {
        return BadRequest(new { message = "Invalid username or failed to reset password." });
    }

    return Ok(new { message = "Password reset successfully." });
}










//         [HttpPost("refresh")]
// public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO tokenRequest)
// {
//     var response = await _userRepo.RefreshToken(tokenRequest);
//     if (string.IsNullOrEmpty(response.Token) || string.IsNullOrEmpty(response.RefreshToken))
//     {
//         _response.IsSuccess = false;
//         _response.StatusCode = HttpStatusCode.BadRequest;
//         _response.ErrorMessages.Add("Invalid token or refresh token");
//         return BadRequest(_response);
//     }

//     _response.Result = response;
//     _response.IsSuccess = true;
//     _response.StatusCode = HttpStatusCode.OK;
//     return Ok(_response);
// }

        
    }
}