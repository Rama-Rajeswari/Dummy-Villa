using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_Web.Models.Dto
{
    public class DestinationCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string ImageUrl { get; set; } 
    }
}