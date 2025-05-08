using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_Web.Models.Dto
{
    public class RoomCreateDTO
    {
        [Required]
        public string RoomType { get; set; } 
        [Required]
        public int MaxGuests { get; set; }
        public List<int> GuestTypeIds { get; set; }
        [Required]
        public double WeekdayPrice { get; set; }  
        [Required]
        public double WeekendPrice { get; set; }  
        [Required]
        public int VillaId { get; set; } 
    }
}