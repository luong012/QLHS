using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS.Models
{
    public class Bill
    {
        public int Bill_ID { get; set; }
        public string Emp_ID { get; set; }
        public string Cus_ID { get; set; }
        public int Bill_Status { get; set; }
        public DateTime Bill_CheckInDate { get; set; }
        public DateTime Bill_CheckOutDate { get; set; }
        public int Bill_CusNum { get; set; }
        public double Bill_BookCost { get; set; }
        public double Bill_ExtraCost { get; set; }
        public double Bill_Total { get; set; }
    }
}
