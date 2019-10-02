using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputationalServer.Models
{
    public class Well
    {
        public double Q { get; set; }
        public double P { get; set; }
        public double P0 { get; set; }
        public double Time1 { get; set; }
        public double Time2 { get; set; }
        public double H0 { get; set; }
        public double Mu { get; set; }
        public double Rw { get; set; }
        public double K { get; set; }
        public double Kappa { get; set; }
        public double Rs { get; set; }
        public double Ksi { get; set; }
        public int N { get; set; }
    }
}
