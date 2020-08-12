using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS.Models
{
    public class RoomType
    {
        [Column("ROOMTYPE_ID")]
        public int RoomType_ID { get; set; }
        public string RoomType_Name { get; set; }
        public int RoomType_MaxCusNum { get; set; }
        public double RoomType_Size { get; set; }
        public double RoomType_Price { get; set; }
        public string RoomType_Desc { get; set; }
        public string RoomType_FTList { get; set; }
        public ICollection<RoomType_Feature> RoomType_Feature { get; set; }
    }
}
