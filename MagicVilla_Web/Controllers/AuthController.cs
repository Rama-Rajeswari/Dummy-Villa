using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    
    public class AuthController : Controller
    {
       
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService=authService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO obj=new();
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
           APIResponse response = await _authService.LoginAsync<APIResponse>(obj);
           if (response != null && response.IsSuccess)
            {
              LoginResponseDTO models = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));

              var handler = new JwtSecurityTokenHandler();
              var jwt = handler.ReadJwtToken(models.Token);

              var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

        
              var nameClaim = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.UniqueName)?.Value 
                        ?? jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value 
                        ?? obj.UserName; 

              var roleClaim = jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Role|| u.Type == "role" || u.Type == "roles")?.Value ?? "User";

             identity.AddClaim(new Claim(ClaimTypes.Name, nameClaim)); 
             identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim));

             var principal = new ClaimsPrincipal(identity);
             await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

       
             HttpContext.Session.SetString("Name", nameClaim);
             HttpContext.Session.SetString("UserRole", roleClaim);
             HttpContext.Session.SetString(SD.SessionToken, models.Token);

             return RedirectToAction("Index", "Home");
            }
         else
         {
           ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
           return View(obj);
         }
       }        
        [HttpGet]
        public IActionResult Profile()
        {    
          var userRole = HttpContext.Session.GetString("UserRole");
          var name=HttpContext.Session.GetString("Name");
          if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(userRole))
          {
            return RedirectToAction("Login", "Auth");
          }
 
        var userProfile = new RegisterationRequestDTO
        {
             Role = userRole,
             Name=name
        };
          return View(userProfile);
        }
 
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDTO obj)
        {
            APIResponse result=await _authService.RegisterAsync<APIResponse>(obj);
            if(result !=null && result.IsSuccess)
            {
                return RedirectToAction("Login");
            }
            //return View();
            return View(obj);
        }
         public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken,"");
            return RedirectToAction("Index","Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
 
        [HttpGet("ForgotPassword")]
public IActionResult ForgotPassword()
{
    return View();
}

 [HttpPost("ForgotPassword")]
public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
{
    if (!ModelState.IsValid)
        return View(model);

    var response = await _authService.ForgotPasswordAsync<APIResponse>(model);

    if (response != null && response.IsSuccess)
    {
        TempData["success"] = "Password reset successfully!";
        return RedirectToAction("Login");
    }

    ModelState.AddModelError("", "Error resetting password.");
    return View(model);
}

        
    }
}
    








