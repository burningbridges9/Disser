using HydrodynamicStudies.Calculs;
using HydrodynamicStudies.Models;
using MathNet.Numerics.Random;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            TestParallelMH();
            //TestMH();
            //RestoreFromFile();
        }

        private static void TestMH()
        {
            MetropolisHastings modelMH = new MetropolisHastings()
            {
                C = 1,
                WalksCount = 60000,
                Ns = 10,
                S_0 = 0.005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                IncludedK = true,
                IncludedKappa = true,
                IncludedKsi = false,
                IncludedP0 = false,

                MinK = Math.Pow(10.0, -15) * 8,
                MinKappa = (1.0 / 3600.0) * 1,
                MinKsi = 0,
                MinP0 = Math.Pow(10.0, 6) * 3,

                MaxK = Math.Pow(10.0, -15) * 11,
                MaxKappa = (1.0 / 3600.0) * 5,
                MaxKsi = 0,
                MaxP0 = Math.Pow(10.0, 6) * 3,

                StepK = Math.Pow(10.0, -15) * 0.6,
                StepKappa = (1.0 / 3600.0) * 0.8,
                StepKsi = 0,
                StepP0 = 0,
            };
            Mode mode = Mode.Direct;
            WellsList wellsList = new WellsList(GetWells());
            var list = Functions.MetropolisHastingsAlgorithm(wellsList, modelMH, mode);
            //var list = Functions.ParallelMetropolisHastingsAlgorithm(wellsList, modelMH, 1, mode);
            Console.WriteLine($"Accepted count = {list.LastOrDefault().AcceptedCount}");
            WriteToFile(list, 2);

        }

        private static void TestParallelMH()
        {
            MetropolisHastings modelMH = new MetropolisHastings()
            {
                C = 1,
                WalksCount = 5000,
                Ns = 10,
                S_0 = 0.005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                IncludedK = true,
                IncludedKappa = true,
                IncludedKsi = false,
                IncludedP0 = false,

                MinK = Math.Pow(10.0, -15) * 8,
                MinKappa = (1.0 / 3600.0) * 1,
                MinKsi = 0,
                MinP0 = Math.Pow(10.0, 6) * 3,

                MaxK = Math.Pow(10.0, -15) * 11,
                MaxKappa = (1.0 / 3600.0) * 5,
                MaxKsi = 0,
                MaxP0 = Math.Pow(10.0, 6) * 3,

                StepK = Math.Pow(10.0, -15) * 0.6,
                StepKappa = (1.0 / 3600.0) * 0.8,
                StepKsi = 0,
                StepP0 = 0,
            };
            Mode mode = Mode.Direct;
            WellsList wellsList = new WellsList(GetWells());
            const int trNum = 8;
            MetropolisParallelObject[] metropolisParallelObjects = new MetropolisParallelObject[trNum];
            System.Random rng = new Random();
            Thread[] threads = new Thread[trNum];
            for (int i = 0; i < trNum; i++)
            {
                metropolisParallelObjects[i] = new MetropolisParallelObject()
                {
                    mode = mode,
                    ModelMH = modelMH,
                    WellsListCurrent = wellsList,
                    rng= rng,
                };
                threads[i] = new Thread(new ParameterizedThreadStart(Functions.ParallelMetropolisHastingsAlgorithm));
                threads[i].Start(metropolisParallelObjects[i]);
            }
            threads.ToList().ForEach(t => t.Join());
            var list = new List<AcceptedValueMH>();
            foreach (var o in metropolisParallelObjects)
            {
                list.AddRange(o.AcceptedValues);
            }
            WriteToFile(list.ToList(), 2);

        }

        static void WriteToFile(List<AcceptedValueMH> accepteds, int values)
        {
            var writePath1 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\K_Q1.txt";
            var writePath2 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\Kappa_Q1.txt";
            var writePathProb = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\Probability_Q1.txt";
            var writePathObj = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\Acc1.txt";
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

                    var json = JsonConvert.SerializeObject(accepteds, Formatting.Indented);
                    using (StreamWriter sw = new StreamWriter(writePathObj, false, Encoding.Default))
                    {
                        sw.Write(json);
                    }
                    break;
                default:
                    break;
            }

        }

        static List<AcceptedValueMH> RestoreFromFile()
        {
            var writePathObj = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\Acc1.txt";
            var json = string.Empty;
            using (StreamReader sw = new StreamReader(writePathObj))
            {
                json = sw.ReadToEnd();
            }
            var objs = JsonConvert.DeserializeObject<List<AcceptedValueMH>>(json);
            return objs;
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
                    N = Convert.ToInt32(50),
                };
                wells.Add(well);
            }
            return wells;
        }
    }
}
