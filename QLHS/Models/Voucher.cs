using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS.Models
{
    public class Voucher
    {
        public string Voucher_ID { get; set; }
        public string Voucher_Desc { get; set; }
        public int Voucher_Value { get; set; }
        public int Voucher_Quantity { get; set; }
    }
}
