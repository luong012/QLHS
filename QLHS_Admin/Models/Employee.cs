using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS_Admin.Models
{
    public class Employee
    {
        public string Emp_ID { get; set; }
        public string User_ID { get; set; }
        public string Emp_Name { get; set; }
        public string Emp_Gender { get; set; }
        public string Emp_Address { get; set; }
        public string Emp_Email { get; set; }
        public DateTime Emp_Birth { get; set; }
        public string Emp_Phone { get; set; }
        public int Emp_Status { get; set; }
        public DateTime Emp_StartDay { get; set; }
    }
}
