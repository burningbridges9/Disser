using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicStudies.Models
{
    public class MetropolisParallelObject
    {
        public List<AcceptedValueMH> AcceptedValues { get; set; } = new List<AcceptedValueMH>();
        public WellsList WellsListCurrent { get; set; }
        public MetropolisHastings ModelMH { get; set; }
        public Mode mode = Mode.Direct;
        public System.Random rng { get; set; }
    }
}
