using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS_Admin.Models
{
    public class RoomVM
    {
        public Room Room { get; set; }
        public IEnumerable<Facility> Facilities { get; set; }
        public IEnumerable<RoomType> RoomTypes { get; set; }
    }
}
