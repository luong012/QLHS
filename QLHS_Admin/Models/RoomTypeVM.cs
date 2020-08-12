using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS_Admin.Models
{
    public class RoomTypeVM
    {
        public RoomType RoomType { get; set; }
        public IEnumerable<Feature> Features { get; set; }
    }
}
