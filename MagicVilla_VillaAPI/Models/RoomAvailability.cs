using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Models
{
    public class RoomAvailability
    {
         [Key]
         public int Id { get; set; }
         public int RoomId { get; set; }
         [ForeignKey("RoomId")]
         public Room Room { get; set; }
         [Required]
         public DateTime Date { get; set; }
         [Required]
         public bool IsAvailable { get; set; } 
    }
}