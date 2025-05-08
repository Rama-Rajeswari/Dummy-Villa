using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_Web.Models.Dto
{
    public class ForgotPasswordDTO
    {
       public string UserName { get; set; }

       [Required]
       [MinLength(6)]
       public string NewPassword { get; set; }

       [Required]
       [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
       public string ConfirmPassword { get; set; }
    }
}