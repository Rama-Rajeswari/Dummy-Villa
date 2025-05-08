using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_Web.Models.Dto
{
    public class VillaCreateDTO
    {      
        [Required]
        [MaxLength(30)]
        public string Name{get;set;}
        public string Details {get;set;}
        public int Occupancy{get;set;}
        public int Sqft{get;set;}
        public string  ImageUrl { get; set; }
        public string DestinationName{get;set;}
    }
    
}