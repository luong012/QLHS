using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS.Models
{

    public class Feature
    {
        [Column("FEATURE_ID")]
        public int Feature_ID { get; set; }
        public string Feature_Name { get; set; }
        public string Feature_Icon { get; set; }
        public ICollection<RoomType_Feature> RoomType_Feature { get; set; }
    }
}
