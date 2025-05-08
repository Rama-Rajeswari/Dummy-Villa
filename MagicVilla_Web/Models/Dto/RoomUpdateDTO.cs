using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_Web.Models.Dto
{
    public class RoomUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string RoomType { get; set; }
        [Required]
        public int MaxGuests { get; set; }
        [Required]
        public int GuestTypeId { get; set; }
        [Required]
        public double WeekdayPrice { get; set; }  
        [Required]
        public double WeekendPrice { get; set; }  
        [Required]
        public int VillaId { get; set; }
    }
}