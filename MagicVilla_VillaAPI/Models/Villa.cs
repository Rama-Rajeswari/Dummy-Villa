using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Models
{
    public class Villa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id{get;set;}
        [Required]
        public string Name{get;set;}
        public string Details {get;set;}
        public int  Sqft { get; set; }
        public int Occupancy { get; set; }
        public string  ImageUrl { get; set; }
        public DateTime UpdatedDate{get;set;}
        public int? DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public Destination? Destination { get; set; }
        public ICollection<VillaFacility> VillaFacilities { get; set; } 
        public ICollection<Room> Rooms { get; set; } 
        
    }
}