using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS_Admin.Models
{
    public class Service
    {
        public int Service_ID { get; set; }
        public string Service_Name { get; set; }
        public double Service_Price { get; set; }
        public string Service_Desc { get; set; }
        public TimeSpan Service_Dur { get; set; }
        public string Service_Img { get; set; }

    }
}
