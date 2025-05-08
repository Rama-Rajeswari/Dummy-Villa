using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_Web.Models.Dto
{
    public class GuestTypeDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }  
         public List<GuestTypeDTO> GuestTypes { get; set; } = new List<GuestTypeDTO>();
    }
}