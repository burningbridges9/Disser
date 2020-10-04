using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicStudies.Models
{
    public interface IWellFeaturesInclude
    {
        bool IncludedK { get; set; }
        bool IncludedKappa { get; set; }
        bool IncludedP0 { get; set; }
        bool IncludedKsi { get; set; }
    }
}
