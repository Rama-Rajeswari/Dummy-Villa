using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_Web.Models.Dto
{
    public class BookingUpdateDTO
    {
        public int Id{get;set;}
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Aadhar Number is required")]
        [RegularExpression("[0-9]{12}", ErrorMessage = "Aadhar Number must be 12 digits")] 
        public string AadharNumber { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Enter a valid phone number")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email{get;set;}
         [Required(ErrorMessage = "Number of People is required")]
        [Range(1, 10, ErrorMessage = "Number of people must be between 1 and 10")]
        public int NumberOfGuests { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }
        [Required(ErrorMessage = "End Date is required")]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage = "Villa Name is required")]
        public string VillaName{get;set;}
    }
}