using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_Web.Models.Dto
{
    public class BookingDTO
    {
         public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile Number must be 10 digits")]
        public string MobileNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required]
        public int VillaId { get; set; }

        public int? RoomId { get; set; }  

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "At least 1 adult is required")]
        public int Adults { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Children count cannot be negative")]
        public int Children { get; set; } = 0;

         private DateTime _checkIn;
         public DateTime CheckIn
         {
             get => _checkIn;
             set => _checkIn = DateTime.SpecifyKind(value, DateTimeKind.Utc);
         }

         private DateTime _checkOut;
         public DateTime CheckOut
         {
             get => _checkOut;
             set => _checkOut = DateTime.SpecifyKind(value, DateTimeKind.Utc);
         }
        public string VillaName { get; set; }
        public string RoomName { get; set; }
        public int MaxGuests { get; set; }
    }
}