using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Models
{
    [PrimaryKey(nameof(RoomId), nameof(GuestTypeId))]
    public class RoomGuestType
    {
        public int RoomId { get; set; }
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        public int GuestTypeId { get; set; }
        [ForeignKey("GuestTypeId")]
        public GuestType GuestType { get; set; }
    }
}