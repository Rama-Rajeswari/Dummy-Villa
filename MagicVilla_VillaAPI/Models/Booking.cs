using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile Number must be 10 digits")]
        public string MobileNumber { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        public int VillaId { get; set; }  
        public Villa Villa { get; set; }  
        public int? RoomId { get; set; }  
        public Room Room { get; set; }  
        [Required]
        public int Adults { get; set; }
        public int Children { get; set; }=0;
        private DateTime _checkIn;
        [Required]
        public DateTime CheckIn
        {
            get => _checkIn;
            set => _checkIn = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private DateTime _checkOut;
        [Required]
        public DateTime CheckOut
        {
            get => _checkOut;
            set => _checkOut = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
        [Required]
        public double TotalCost { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsFullVillaBooking => RoomId == null; 
        [NotMapped]
        public string VillaName => Villa?.Name;  
        [NotMapped]
        public string RoomType => Room?.RoomType;  
    }
}