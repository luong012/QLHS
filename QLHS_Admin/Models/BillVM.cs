using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS_Admin.Models
{
    public class BillVM
    {
        public Bill Bill { get; set; }
        public List<RoomVM> RoomVMs { get; set; }
        public List<ServiceVM> ServiceVMs { get; set; }
    }
}
