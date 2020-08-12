using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS_Admin.Models
{
    public class AddServiceVM
    {
        public Service Service { get; set; }
        public IEnumerable<FFacility> FFacilities { get; set; }
    }
}
