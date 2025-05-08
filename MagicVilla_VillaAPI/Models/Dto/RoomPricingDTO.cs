using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class RoomPricingDTO
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
    }
}