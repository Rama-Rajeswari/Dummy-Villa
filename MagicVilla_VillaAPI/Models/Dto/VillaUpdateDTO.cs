using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaUpdateDTO
    {
        [Required]
        public int Id{get;set;}
        [Required]
        [MaxLength(30)]
        public string Name{get;set;}
        public string Details {get;set;}
        
        [Required]
        public int Occupancy{get;set;}
        [Required]
        public int Sqft{get;set;}
        [Required]
        public string  ImageUrl { get; set; }
        public string DestinationName { get; set; }
        public List<RoomUpdateDTO> Rooms { get; set; } = new List<RoomUpdateDTO>();
        public List<string> Facilities { get; set; } = new List<string>();
    }
}