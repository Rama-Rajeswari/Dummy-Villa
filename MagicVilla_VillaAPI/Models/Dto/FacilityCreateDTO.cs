using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class FacilityCreateDTO
    {
        [Required]
        public string Name { get; set; } 
    }
}