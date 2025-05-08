using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser>_userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext context,IConfiguration configuration,UserManager<ApplicationUser> userManager,IMapper mapper,RoleManager<IdentityRole> roleManager)
        {
            _context=context;
            _mapper=mapper;
            _userManager=userManager;
            _roleManager=roleManager;
            secretKey=configuration.GetValue<string>("ApiSettings:Secret");
            
        }
        public bool IsUniqueUser(string username)
        {
           var user=_context.ApplicationUsers.FirstOrDefault(x=>x.UserName==username);
           if(user==null)
           {
            return true;
           }
           return false;
        }
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
          var user = _context.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

          if (user == null)
          {
            return new LoginResponseDTO() { Token = "", User = null };
          }

          bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
          if (!isValid)
         {
               return new LoginResponseDTO() { Token = "", User = null };
         }

            var roles = await _userManager.GetRolesAsync(user);
           if (roles == null || !roles.Any()) 
           {
               roles = new List<string> { "customer" }; 
           }

           var tokenHandler = new JwtSecurityTokenHandler();
           var key = Encoding.ASCII.GetBytes(secretKey);

           var claims = new List<Claim>
           {
                new Claim(ClaimTypes.Name, user.UserName ?? "UnknownUser"), 
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "customer") 
          };

            var tokenDescriptor = new SecurityTokenDescriptor
           {
                 Subject = new ClaimsIdentity(claims),
                 Expires = DateTime.UtcNow.AddDays(7),
                 //Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
           };

           var token = tokenHandler.CreateToken(tokenDescriptor);

          return new LoginResponseDTO()
         {
            Token = tokenHandler.WriteToken(token),
           // RefreshToken = GenerateRefreshToken(user.UserName),
             User = _mapper.Map<UserDTO>(user)
         };
    }
        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
           ApplicationUser user=new ()
           {
            UserName=registerationRequestDTO.UserName,
            Email=registerationRequestDTO.UserName,
            NormalizedEmail=registerationRequestDTO.UserName.ToUpper(),
            Name=registerationRequestDTO.Name
            
           };
        try
         {
           var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
           if (result.Succeeded)
           {
           
              if (!await _roleManager.RoleExistsAsync(registerationRequestDTO.Role))
              {
                await _roleManager.CreateAsync(new IdentityRole(registerationRequestDTO.Role));
              }
              await _userManager.AddToRoleAsync(user, registerationRequestDTO.Role);
 
             var userToReturn = _context.ApplicationUsers
                .FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);
             return _mapper.Map<UserDTO>(userToReturn);
           }
         }
         catch (Exception e)
         {
            Console.WriteLine($"Error: {e.Message}");
         }
 
         return new UserDTO();
           
        }


        public async Task<bool> ResetPasswordAsync(ForgotPasswordDTO model)
        {
    var user = await _userManager.FindByNameAsync(model.UserName);
    if (user == null)
    {
        return false;
    }

    // Remove existing password
    var removePassResult = await _userManager.RemovePasswordAsync(user);
    if (!removePassResult.Succeeded)
    {
        return false;
    }

    // Add new password
    var addPassResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
    return addPassResult.Succeeded;
}






//        public string GenerateRefreshToken(string username)
// {
//     var tokenHandler = new JwtSecurityTokenHandler();
//     var key = Encoding.ASCII.GetBytes(secretKey);

//     var tokenDescriptor = new SecurityTokenDescriptor
//     {
//         Subject = new ClaimsIdentity(new[]
//         {
//             new Claim(ClaimTypes.Name, username),
//             new Claim("TokenType", "RefreshToken") // optional tag
//         }),
//         Expires = DateTime.UtcNow.AddDays(7), // Refresh token valid for 7 days
//         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//     };

//     var token = tokenHandler.CreateToken(tokenDescriptor);
//     return tokenHandler.WriteToken(token);
// }

// public async Task<LoginResponseDTO> RefreshToken(TokenRequestDTO tokenRequestDTO)
// {
//     var tokenHandler = new JwtSecurityTokenHandler();
//     SecurityToken validatedAccessToken;
//     SecurityToken validatedRefreshToken;

//     try
//     {
//         var key = Encoding.ASCII.GetBytes(secretKey);

//         // 1️⃣ Validate Access Token (expired is OK)
//         var accessTokenPrincipal = tokenHandler.ValidateToken(tokenRequestDTO.Token, new TokenValidationParameters
//         {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(key),
//             ValidateIssuer = false,
//             ValidateAudience = false,
//             ValidateLifetime = false // We allow expired access token
//         }, out validatedAccessToken);

//         var username = accessTokenPrincipal.Identity?.Name;
//         if (string.IsNullOrEmpty(username))
//         {
//             return new LoginResponseDTO() { Token = "", RefreshToken = "", User = null };
//         }

//         // 2️⃣ Validate Refresh Token
//         var refreshTokenPrincipal = tokenHandler.ValidateToken(tokenRequestDTO.RefreshToken, new TokenValidationParameters
//         {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(key),
//             ValidateIssuer = false,
//             ValidateAudience = false,
//             ClockSkew = TimeSpan.Zero,
//             ValidateLifetime = true // Must NOT be expired
//         }, out validatedRefreshToken);

//         var refreshUsername = refreshTokenPrincipal.Identity?.Name;
//         if (refreshUsername != username)
//         {
//             return new LoginResponseDTO() { Token = "", RefreshToken = "", User = null };
//         }

//         // 3️⃣ Generate new tokens
//         var user = await _userManager.FindByNameAsync(username);
//         if (user == null)
//         {
//             return new LoginResponseDTO() { Token = "", RefreshToken = "", User = null };
//         }

//         var roles = await _userManager.GetRolesAsync(user);
//         var claims = new List<Claim>
//         {
//             new Claim(ClaimTypes.Name, user.UserName ?? "UnknownUser"),
//             new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "customer")
//         };

//         var tokenDescriptor = new SecurityTokenDescriptor
//         {
//             Subject = new ClaimsIdentity(claims),
//             Expires = DateTime.UtcNow.AddMinutes(5),
//             SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//         };

//         var newAccessToken = tokenHandler.CreateToken(tokenDescriptor);
//         var newRefreshToken = GenerateRefreshToken(username);

//         return new LoginResponseDTO()
//         {
//             Token = tokenHandler.WriteToken(newAccessToken),
//             RefreshToken = newRefreshToken,
//             User = _mapper.Map<UserDTO>(user)
//         };
//     }
//     catch (Exception ex)
//     {
//         // Optionally log the error
//         return new LoginResponseDTO() { Token = "", RefreshToken = "", User = null };
//     }
// }





    }
}
    