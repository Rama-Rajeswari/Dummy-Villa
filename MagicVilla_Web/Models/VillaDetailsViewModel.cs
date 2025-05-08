using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Models
{
    public class VillaDetailsViewModel
    {
        public VillaDTO Villa { get; set; }
        public List<RoomDTO> Rooms { get; set; } = new List<RoomDTO>();
        public List<FacilityDTO> Facilities { get; set; }
    }
}