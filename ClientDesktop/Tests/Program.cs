using HydrodynamicStudies.Calculs;
using HydrodynamicStudies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            TestMH();
        }

        private static void TestMH()
        {
            MetropolisHastings modelMH = new MetropolisHastings()
            {
                C = 1,
                WalksCount = 10,
                Ns = 2,
                S_0 = 0.025,
                IncludedK = true,
                IncludedKappa = true,
                IncludedKsi = false,
                IncludedP0 = false,

                MinK = Math.Pow(10.0, -15) * 5,
                MinKappa = (1.0 / 3600.0) * 2,
                MinKsi = 0,
                MinP0 = Math.Pow(10.0, 6) * 3,

                MaxK = Math.Pow(10.0, -15) * 15,
                MaxKappa = (1.0 / 3600.0) * 8,
                MaxKsi = 0,
                MaxP0 = Math.Pow(10.0, 6) * 3,

                StepK = 1,
                StepKappa = 2,
                StepKsi = 0,
                StepP0 = 0,
            };
            Mode mode = Mode.Direct;
            WellsList wellsList = new WellsList(GetWells());
            Functions.MetropolisHastingsAlgorithm(wellsList, modelMH, mode);
        }

        static List<Well> GetWells()
        {
            List<Well> wells = new List<Well>();
            for (int i = 1; i <= 3; i++)
            {
                Well well = new Well
                {
                    Q = 1.0 / (24.0 * 3600.0) * Convert.ToDouble(5) * i,
                    P = Math.Pow(10.0, 6) * Convert.ToDouble(5) * i,
                    P0 = Math.Pow(10.0, 6) * Convert.ToDouble(3),
                    Time1 = 3600.0 * Convert.ToDouble(5) * (i - 1),
                    Time2 = 3600.0 * Convert.ToDouble(5) * i,
                    H0 = Convert.ToDouble(1),
                    Mu = Math.Pow(10.0, -3) * Convert.ToDouble(1),
                    Rw = Convert.ToDouble(0.1),
                    K = Math.Pow(10.0, -15) * Convert.ToDouble(10),
                    Kappa = (1.0 / 3600.0) * Convert.ToDouble(4),
                    Rs = Convert.ToDouble(0.3),
                    Ksi = Convert.ToDouble(0),
                    N = Convert.ToInt32(100),
                };
                wells.Add(well);
            }
            return wells;
        }
    }
}
