using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS.Models
{
    public class RoomType_Feature
    {
        [Column("ROOMTYPE_ID")]
        public int RoomType_ID { get; set; }
        public RoomType RoomType { get; set; }
        [Column("FEATURE_ID")]
        public int Feature_ID { get; set; }
        public Feature Feature { get; set; }
    }
}
