using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Models
{
    public class GuestType
    {
        [Key]
        public int Id { get; set; } 
        [Required]
        public string Name { get; set; } 
        public ICollection<RoomGuestType> RoomGuestTypes { get; set; }= new List<RoomGuestType>();
    }
}