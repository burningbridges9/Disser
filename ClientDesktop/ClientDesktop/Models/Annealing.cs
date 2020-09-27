using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicStudies.Models
{
    public class Annealing
    {
        public int MaxIterations { get; set; } = 100;
        public double KMin { get; set; } = Math.Pow(10.0, -15) * 6;
        public double KMax { get; set; } = Math.Pow(10.0, -15) * 15;
        public double KappaMin { get; set; } = (1.0 / 3600.0) * 2;
        public double KappaMax { get; set; } = (1.0 / 3600.0) * 12;
        public double P0Min { get; set; } = Math.Pow(10.0, 6) * 0;
        public double P0Max { get; set; } = Math.Pow(10.0, 6) * 20;

        public double deltaK { get; set; }

        public double Rand()
        {
            MathNet.Numerics.Distributions.Normal normalDist = new Normal();
            return normalDist.Sample();
        }

    }
}
