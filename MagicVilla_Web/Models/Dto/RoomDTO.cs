using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_Web.Models.Dto
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string RoomType { get; set; }  
        public int MaxGuests { get; set; }      
        public double WeekdayPrice { get; set; }
        public double WeekendPrice { get; set; }
        
        public int VillaId { get; set; }  
       public string VillaName { get; set; } 
       public List<GuestTypeDTO> GuestTypes { get; set; } = new List<GuestTypeDTO>();
    }
}