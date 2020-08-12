using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS_Admin.Models
{
    public class Facility
    {
        public int Fac_ID { get; set; }
        public string Fac_Name { get; set; }
        public string Fac_Address { get; set; }
        public string Fac_Phone { get; set; }
        public string Fac_Desc { get; set; }
        public double Fac_Latitude { get; set; }
        public double Fac_Longitude { get; set; }
        public double? Fac_Rating { get; set; }
        public string Fac_Img { get; set; }
        //public ICollection<Room> Rooms { get; set; }
    }
}
