using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicVilla_Web.Models.Dto
{
    public class BookingResponseDTO
    {
        public string Message { get; set; }
        public double TotalCost { get; set; }
        public int VillaId { get; set; }
        public string VillaName { get; set; }
        public int? RoomId { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}