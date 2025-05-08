using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string RoomType { get; set; } 
        public int MaxGuests { get; set; }
        [Required]
        public double WeekdayPrice { get; set; }  
        [Required]
        public double WeekendPrice { get; set; }       
        public int VillaId { get; set; }
        [ForeignKey("VillaId")]
        public Villa Villa { get; set; }
        
        public ICollection<RoomGuestType> RoomGuestTypes { get; set; }= new List<RoomGuestType>();
    }
}