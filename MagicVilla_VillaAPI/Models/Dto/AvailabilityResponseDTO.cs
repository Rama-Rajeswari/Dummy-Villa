using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class AvailabilityResponseDTO
    {
        public bool IsAvailable { get; set; }
        public string CheckIn { get; set; }  // Date formatted as string (YYYY-MM-DD)
    public string CheckOut { get; set; } // Date formatted as string (YYYY-MM-DD)
    }
}