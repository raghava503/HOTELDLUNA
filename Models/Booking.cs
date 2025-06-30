using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication7.Models
{

    public class Booking
    {
        public Guid BookingId { get; set; }
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal Bill { get; set; }
        public string Username { get; set; }

    }

}