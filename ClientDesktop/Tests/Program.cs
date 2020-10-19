using HydrodynamicStudies.Calculs;
using HydrodynamicStudies.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
                WalksCount = 1000,
                Ns = 10,
                S_0 = 0.025,
                IncludedK = true,
                IncludedKappa = true,
                IncludedKsi = false,
                IncludedP0 = false,

                MinK = Math.Pow(10.0, -15) * 3,
                MinKappa = (1.0 / 3600.0) * 2,
                MinKsi = 0,
                MinP0 = Math.Pow(10.0, 6) * 3,

                MaxK = Math.Pow(10.0, -15) * 15,
                MaxKappa = (1.0 / 3600.0) * 12,
                MaxKsi = 0,
                MaxP0 = Math.Pow(10.0, 6) * 3,

                StepK = Math.Pow(10.0, -15) * 2,
                StepKappa = (1.0 / 3600.0) * 2,
                StepKsi = 0,
                StepP0 = 0,
            };
            Mode mode = Mode.Direct;
            WellsList wellsList = new WellsList(GetWells());
            var list = Functions.MetropolisHastingsAlgorithm(wellsList, modelMH, mode);
            WriteToFile(list, 2);

        }


        static void WriteToFile(List<AcceptedValueMH> accepteds, int values)
        {
            var writePath1 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\K_Q.txt";
            var writePath2 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\Kappa_Q.txt";
            var writePathProb = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\Probability_Q.txt";
            switch (values)
            {
                case 1:
                    using (StreamWriter sw1 = new StreamWriter(writePath1, false, Encoding.Default))
                    using (StreamWriter sw2 = new StreamWriter(writePathProb, false, Encoding.Default))
                    {
                        foreach (var a in accepteds)
                        {
                            sw1.Write(a.K * Math.Pow(10.0, 15) + " ");
                            sw2.Write(a.ProbabilityDensity + " ");
                        }
                    }
                    break;
                case 2:
                    using (StreamWriter sw1 = new StreamWriter(writePath1, false, Encoding.Default))
                    using (StreamWriter sw2 = new StreamWriter(writePathProb, false, Encoding.Default))
                    using (StreamWriter sw3 = new StreamWriter(writePath2, false, Encoding.Default))
                    {
                        foreach (var a in accepteds)
                        {
                            sw1.Write(a.K * Math.Pow(10.0, 15) + " ");
                            sw2.Write(a.Fmin + " ");
                            sw3.Write(a.Kappa * 3600.0 + " ");
                        }
                    }
                    break;
                default:
                    break;
            }
            
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
