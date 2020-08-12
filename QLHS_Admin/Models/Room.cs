using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS_Admin.Models
{
    public class Room
    {
        public int Room_ID { get; set; }
        public int RoomType_ID { get; set; }
        public int Fac_ID { get; set; }
        public int Room_Status { get; set; }
        public string Room_Img { get; set; }
        //public Facility Facility { get; set; }
        //public RoomType RoomType { get; set; }
    }
}
