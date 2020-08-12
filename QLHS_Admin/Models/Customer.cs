using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS_Admin.Models
{
    public class Customer
    {
        public string Cus_ID { get; set; }
        public string User_ID { get; set; }
        public string Cus_Name { get; set; }
        public string Cus_Gender { get; set; }
        public string Cus_Address { get; set; }
        public string Cus_Email { get; set; }
        public DateTime Cus_Birth { get; set; }
        public string Cus_Phone { get; set; }
    }
}
