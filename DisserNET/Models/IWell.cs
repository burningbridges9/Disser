using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Models
{
    public interface IWell
    {
         double K { get; set; }
         double Kappa { get; set; }
         double P0 { get; set; }
         double Ksi { get; set; }
    }
}
