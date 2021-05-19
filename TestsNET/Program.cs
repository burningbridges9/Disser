using DisserNET.Calculs;
using DisserNET.Models;
using DisserNET.Utils;
using MathNet.Numerics.Random;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestsNET
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");


#pragma warning disable CA1416 // Validate platform compatibility
            SerialTest();

            //var path = @"C:\Users\Rustam\Desktop\Master\MHREPO~1\RE2873~1\EXP_20~1.000\Fmin.txt";
            //string fmins = "";
            //using (StreamReader sw = new StreamReader(path))
            //{
            //    fmins = sw.ReadToEnd();
            //}

            //var fm = fmins.Split(" ").Where(x=> !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).Select(x => double.Parse(x)).ToList();

        }


        private static void TestMH(MetropolisHastings modelMH, Mode mode)
        {
            ReportDb reportDb = new ReportDb(null);
#pragma warning disable CA1416 // Validate platform compatibility
            WellsList wellsList = new WellsList(GetWells());

            List<AcceptedValueMH> list = new List<AcceptedValueMH>();
            if (mode == Mode.Direct)
                Functions.MetropolisHastingsAlgorithmForConsumptions(wellsList, modelMH, list, mode);
            else
                Functions.MetropolisHastingsAlgorithmForPressures(wellsList, modelMH, list, mode);

            //List<AcceptedValueMH> list = Functions.ParallelMetropolisHastingsAlgorithm(wellsList, modelMH, 3, mode).Result;
            reportDb.WriteMHInfo(modelMH, list.ToList());
            //var list = Functions.ParallelMetropolisHastingsAlgorithm(wellsList, modelMH, 8, mode);
            Console.WriteLine($"Accepted count = {list.LastOrDefault().AcceptedCount}");
            //WriteToFile(list, 2);
        }

        private static void TestParallelMH()
        {
            MetropolisHastings modelMH = new MetropolisHastings()
            {
                C = 1,
                WalksCount = 500000, // 250000, 500000 
                Ns = 10,
                S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                IncludedK = true,
                IncludedKappa = false,
                IncludedKsi = true,
                IncludedP0 = false,

                MinK = Math.Pow(10.0, -15) * 26.0,
                MinKappa = (1.0 / 3600.0) * 290.0,
                MinKsi = 1,
                MinP0 = Math.Pow(10.0, 6) * 13,

                MaxK = Math.Pow(10.0, -15) * 28.0,
                MaxKappa = (1.0 / 3600.0) * 310.0,
                MaxKsi = 4,
                MaxP0 = Math.Pow(10.0, 6) * 16,

                StepK = Math.Pow(10.0, -15) * (32.0 - 28.0) / 30.0,
                StepKappa = (1.0 / 3600.0) * (310.0 - 290.0) / 30.0,
                StepKsi = (7 - 3) / 30.0,
                StepP0 = Math.Pow(10.0, 6) * (16.0 - 13.0) / 30.0,

                SelectLogic = SelectLogic.BasedOnAccepted,
                Mode = Mode.Reverse,
                MoveLogic = MoveLogic.Cyclic,

                MHStartValues = new List<MetropolisHastingsStartValue>()
                {
                    new MetropolisHastingsStartValue(DisserNET.Calculs.ValueType.Kappa, 295.2),
                },
            };
            Mode mode = Mode.Reverse;
            WellsList wellsList = new WellsList(GetWells());
            const int trNum = 2;
            MetropolisParallelObject[] metropolisParallelObjects = new MetropolisParallelObject[trNum];
            System.Random rng = new CryptoRandomSource(threadSafe: true);

            Thread[] threads = new Thread[trNum];
            for (int i = 0; i < trNum; i++)
            {
                metropolisParallelObjects[i] = new MetropolisParallelObject()
                {
                    mode = mode,
                    ModelMH = modelMH,
                    WellsListCurrent = wellsList,
                    rng = rng,
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

        static void WriteToFile(List<AcceptedValueMH> accepteds, int values, int wrP = 0)
        {
            var writePath1 = @"C:\Users\Rustam\Desktop\Master\MHReports\report_2021-01-07\exp_2021-01-07-04-14;\K.txt"; // Q1 = 0.001, Q2 = 0.002, Q3 = 0.0015, Q4 
            var writePath2 = @"C:\Users\Rustam\Desktop\Master\MHReports\report_2021-01-07\exp_2021-01-07-04-14;\Kappa.txt";
            var writePathProb = @"C:\Users\Rustam\Desktop\Master\MHReports\report_2021-01-07\exp_2021-01-07-04-14;\Fmin.txt";
            var writePathObj = @"C:\Users\Rustam\Desktop\Master\MHReports\report_2021-01-07\exp_2021-01-07-04-14;\Acc.txt";
            switch (values)
            {
                case 1:
                    using (StreamWriter sw1 = new StreamWriter(wrP == 1 ? writePath1 : writePath2, false, Encoding.Default))
                    using (StreamWriter sw2 = new StreamWriter(writePathProb, false, Encoding.Default))
                    {
                        foreach (var a in accepteds)
                        {
                            var strtowrite = wrP == 1 ? a.K * Math.Pow(10.0, 15) : a.Kappa * 3600.0;
                            sw1.Write(strtowrite + " ");
                            sw2.Write(a.Fmin + " ");
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
            var writePathObj = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\Acc2.txt";
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
                    Q = 1.0 / (24.0 * 3600.0) * Convert.ToDouble(25) * i,
                    P = Math.Pow(10.0, 6) * Convert.ToDouble(25) * i,
                    P0 = Math.Pow(10.0, 6) * Convert.ToDouble(15),
                    Time1 = 3600.0 * Convert.ToDouble(10) * (i - 1),
                    Time2 = 3600.0 * Convert.ToDouble(10) * i,
                    H0 = Convert.ToDouble(5),
                    Mu = Math.Pow(10.0, -3) * Convert.ToDouble(1), // 1- water, 5 - oil
                    Rw = Convert.ToDouble(0.1),
                    K = Math.Pow(10.0, -15) * Convert.ToDouble(30),
                    Kappa = (1.0 / 3600.0) * Convert.ToDouble(300), // 300- water, 75 - oil
                    Rs = Convert.ToDouble(0.5),
                    Ksi = Convert.ToDouble(5),
                    N = Convert.ToInt32(50),
                };
                wells.Add(well);
            }
            return wells;
        }



        static void SerialTest()
        {
            List<MetropolisHastings> l = new List<MetropolisHastings>()
            {
                #region 14_02_2021
                //// 0.0002
                //// 20x20
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0002, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 7.5,
                //    MinKappa = (1.0 / 3600.0) * 0.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 9.5,
                //    MaxKappa = (1.0 / 3600.0) * 3.5,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (9.5-7.5)/20.0,
                //    StepKappa = (1.0 / 3600.0) * (3.5-0.5)/20.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //// 30x30
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0002, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 7.5,
                //    MinKappa = (1.0 / 3600.0) * 0.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 2,

                //    MaxK = Math.Pow(10.0, -15) * 9.5,
                //    MaxKappa = (1.0 / 3600.0) * 3.5,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (9.5-7.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (3.5-0.5)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 3,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                ////20x20
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0002, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 9.5,
                //    MinKappa = (1.0 / 3600.0) * 2.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 10.5,
                //    MaxKappa = (1.0 / 3600.0) * 4.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (10.5-9.5)/20.0,
                //    StepKappa = (1.0 / 3600.0) * (4.0-2.0)/20.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                ////30x30
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0002, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 9.5,
                //    MinKappa = (1.0 / 3600.0) * 2.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 10.5,
                //    MaxKappa = (1.0 / 3600.0) * 4.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (10.5-9.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (4.0-2.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                //// 0.0001
                //// 20x20
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 7.5,
                //    MinKappa = (1.0 / 3600.0) * 0.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 9.5,
                //    MaxKappa = (1.0 / 3600.0) * 3.5,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (9.5-7.5)/20.0,
                //    StepKappa = (1.0 / 3600.0) * (3.5-0.5)/20.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //// 30x30
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 7.5,
                //    MinKappa = (1.0 / 3600.0) * 0.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 2,

                //    MaxK = Math.Pow(10.0, -15) * 9.5,
                //    MaxKappa = (1.0 / 3600.0) * 3.5,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (9.5-7.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (3.5-0.5)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 3,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                ////20x20
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 9.5,
                //    MinKappa = (1.0 / 3600.0) * 2.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 10.5,
                //    MaxKappa = (1.0 / 3600.0) * 4.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (10.5-9.5)/20.0,
                //    StepKappa = (1.0 / 3600.0) * (4.0-2.0)/20.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                ////30x30
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 9.5,
                //    MinKappa = (1.0 / 3600.0) * 2.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 10.5,
                //    MaxKappa = (1.0 / 3600.0) * 4.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (10.5-9.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (4.0-2.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                // // 0.001
                //// 20x20
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 7.5,
                //    MinKappa = (1.0 / 3600.0) * 0.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 9.5,
                //    MaxKappa = (1.0 / 3600.0) * 3.5,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (9.5-7.5)/20.0,
                //    StepKappa = (1.0 / 3600.0) * (3.5-0.5)/20.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //// 30x30
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 7.5,
                //    MinKappa = (1.0 / 3600.0) * 0.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 2,

                //    MaxK = Math.Pow(10.0, -15) * 9.5,
                //    MaxKappa = (1.0 / 3600.0) * 3.5,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (9.5-7.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (3.5-0.5)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 3,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                ////20x20
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 9.5,
                //    MinKappa = (1.0 / 3600.0) * 2.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 10.5,
                //    MaxKappa = (1.0 / 3600.0) * 4.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (10.5-9.5)/20.0,
                //    StepKappa = (1.0 / 3600.0) * (4.0-2.0)/20.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                ////30x30
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 9.5,
                //    MinKappa = (1.0 / 3600.0) * 2.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 10.5,
                //    MaxKappa = (1.0 / 3600.0) * 4.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (10.5-9.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (4.0-2.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                #endregion

                #region 15_02_2021

                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 7.5,
                //    MinKappa = (1.0 / 3600.0) * 0.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 8.5,
                //    MaxKappa = (1.0 / 3600.0) * 1.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (8.5-7.5)/20.0,
                //    StepKappa = (1.0 / 3600.0) * (1.0-0.5)/20.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 9.5,
                //    MinKappa = (1.0 / 3600.0) * 2.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 10.5,
                //    MaxKappa = (1.0 / 3600.0) * 4.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (10.5-9.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (4.0-2.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                #endregion

                #region 16_02_2021                
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 7.5,
                //    MinKappa = (1.0 / 3600.0) * 0.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 8.5,
                //    MaxKappa = (1.0 / 3600.0) * 1.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (8.5-7.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (1.0-0.5)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                #endregion

                #region 17_02_2021                
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 5 * 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 7.5,
                //    MinKappa = (1.0 / 3600.0) * 0.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 8.5,
                //    MaxKappa = (1.0 / 3600.0) * 1.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (8.5-7.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (1.0-0.5)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 5 * 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 9.5,
                //    MinKappa = (1.0 / 3600.0) * 3.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 10.5,
                //    MaxKappa = (1.0 / 3600.0) * 4.5,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (10.5-9.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (4.5-3.5)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                // new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 5 * 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 10.5,
                //    MinKappa = (1.0 / 3600.0) * 4.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 12.5,
                //    MaxKappa = (1.0 / 3600.0) * 6.5,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (12.5-10.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (6.5-4.5)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                #endregion
                #region 18_02_2021
                //  new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 3000000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 9.5,
                //    MinKappa = (1.0 / 3600.0) * 3.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 12.5,
                //    MaxKappa = (1.0 / 3600.0) * 6.5,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (12.5-9.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (6.5-3.5)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                // new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 12.3,
                //    MinKappa = (1.0 / 3600.0) * 26.6,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 3,

                //    MaxK = Math.Pow(10.0, -15) * 13.3,
                //    MaxKappa = (1.0 / 3600.0) * 27.2,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 3,

                //    StepK = Math.Pow(10.0, -15) * (13.3-12.3)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (27.2-26.6)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                #endregion
                #region 22_02_2021

                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 100000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 25.0,
                //    MinKappa = (1.0 / 3600.0) * 295.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 35.0,
                //    MaxKappa = (1.0 / 3600.0) * 305.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (35.0-25.0)/50.0,
                //    StepKappa = (1.0 / 3600.0) * (305.0 - 295.5)/50.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 100000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 25.0,
                //    MinKappa = (1.0 / 3600.0) * 295.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 35.0,
                //    MaxKappa = (1.0 / 3600.0) * 305.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (35.0-25.0)/50.0,
                //    StepKappa = (1.0 / 3600.0) * (305.0 - 295.5)/50.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 100000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 30.0,
                //    MinKappa = (1.0 / 3600.0) * 293.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 296.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (32.0-30.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (296.0 - 293.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 100000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 304.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 30.0,
                //    MaxKappa = (1.0 / 3600.0) * 306.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (30.0-28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (306.0 - 304.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                // new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 100000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 26.0,
                //    MinKappa = (1.0 / 3600.0) * 290.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 31.0,
                //    MaxKappa = (1.0 / 3600.0) * 295.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (31.0-26.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (295.0 - 290.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 100000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 305.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 30.0,
                //    MaxKappa = (1.0 / 3600.0) * 309.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (30.0-28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (309.0 - 305.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                // new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 20.0,
                //    MinKappa = (1.0 / 3600.0) * 200.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 40.0,
                //    MaxKappa = (1.0 / 3600.0) * 400.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (40.0-20.0)/60.0,
                //    StepKappa = (1.0 / 3600.0) * (400.0 - 200.0)/60.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.AcceptAll,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 20.0,
                //    MinKappa = (1.0 / 3600.0) * 200.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 40.0,
                //    MaxKappa = (1.0 / 3600.0) * 400.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (40.0-20.0)/60.0,
                //    StepKappa = (1.0 / 3600.0) * (400.0 - 200.0)/60.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.AcceptAll,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 25.0,
                //    MinKappa = (1.0 / 3600.0) * 100.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 35.0,
                //    MaxKappa = (1.0 / 3600.0) * 250.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (35.0-25.0)/60.0,
                //    StepKappa = (1.0 / 3600.0) * (250.0 - 100.0)/60.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.AcceptAll,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 25.0,
                //    MinKappa = (1.0 / 3600.0) * 400.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 35.0,
                //    MaxKappa = (1.0 / 3600.0) * 550.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (35.0-25.0)/60.0,
                //    StepKappa = (1.0 / 3600.0) * (550.0 - 400.0)/60.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.AcceptAll,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 50000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 25.0,
                //    MinKappa = (1.0 / 3600.0) * 50.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 30.0,
                //    MaxKappa = (1.0 / 3600.0) * 100.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (30.0-25.0)/60.0,
                //    StepKappa = (1.0 / 3600.0) * (100.0 - 50.0)/60.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.AcceptAll,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 50000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 30.0,
                //    MinKappa = (1.0 / 3600.0) * 550.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 35.0,
                //    MaxKappa = (1.0 / 3600.0) * 700.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (35.0-30.0)/60.0,
                //    StepKappa = (1.0 / 3600.0) * (700.0 - 550.0)/60.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.AcceptAll,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 5 * 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 25.5,
                //    MinKappa = (1.0 / 3600.0) * 68.5,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 27.0,
                //    MaxKappa = (1.0 / 3600.0) * 72.5,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (27.0-25.5)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (72.5 - 68.5)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 5 * 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 31.0,
                //    MinKappa = (1.0 / 3600.0) * 690.0,
                //    MinKsi = 0,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 34.0,
                //    MaxKappa = (1.0 / 3600.0) * 695.0,
                //    MaxKsi = 0,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (34.0-31.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (695.0 - 690.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                #endregion
                #region 25_02_2021

                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 50000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 25.0,
                //    MinKappa = (1.0 / 3600.0) * 200.0,
                //    MinKsi = 5,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 35.0,
                //    MaxKappa = (1.0 / 3600.0) * 295.0,
                //    MaxKsi = 5,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (35.0-25.0)/70.0,
                //    StepKappa = (1.0 / 3600.0) * (295.0 - 200.0)/70.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.AcceptAll,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 50000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 25.0,
                //    MinKappa = (1.0 / 3600.0) * 300.0,
                //    MinKsi = 5,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 35.0,
                //    MaxKappa = (1.0 / 3600.0) * 400.0,
                //    MaxKsi = 5,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (35.0-25.0)/70.0,
                //    StepKappa = (1.0 / 3600.0) * (400.0 - 300.0)/70.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.AcceptAll,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 5* 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 26.0,
                //    MinKappa = (1.0 / 3600.0) * 70.0,
                //    MinKsi = 5,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 28.0,
                //    MaxKappa = (1.0 / 3600.0) * 75.0,
                //    MaxKsi = 5,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (35.0-25.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (75.0 - 70.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 5 * 0.0001, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 29.0,
                //    MinKappa = (1.0 / 3600.0) * 700.0,
                //    MinKsi = 5,
                //    MinP0 = Math.Pow(10.0, 6) * 15,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 710.0,
                //    MaxKsi = 5,
                //    MaxP0 = Math.Pow(10.0, 6) * 15,

                //    StepK = Math.Pow(10.0, -15) * (32.0-29.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (710.0 - 700.0)/30.0,
                //    StepKsi = 0,
                //    StepP0 = Math.Pow(10.0, 6) * 0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic
                //},

                #endregion
                #region MyRegion       
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = false,
                //    IncludedKsi = false,
                //    IncludedP0 = true,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 290.0,
                //    MinKsi = 2,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 310.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (310.0 - 290.0)/30.0,
                //    StepKsi = (7-2)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic,

                //    MHStartValues = new List<MetropolisHastingsStartValue>()
                //    {
                //        new MetropolisHastingsStartValue(DisserNET.Calculs.ValueType.Kappa, 308.7),
                //    },
                //},

                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = false,
                //    IncludedKsi = true,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 290.0,
                //    MinKsi = 3,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 310.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 16,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (310.0 - 290.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (16.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic,

                //    MHStartValues = new List<MetropolisHastingsStartValue>()
                //    {
                //        new MetropolisHastingsStartValue(DisserNET.Calculs.ValueType.Kappa, 295.2),
                //    },
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = false,
                //    IncludedKsi = true,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 290.0,
                //    MinKsi = 3,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 310.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 16,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (310.0 - 290.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (16.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic,

                //    MHStartValues = new List<MetropolisHastingsStartValue>()
                //    {
                //        new MetropolisHastingsStartValue(DisserNET.Calculs.ValueType.Kappa, 308.7),
                //    },
                //},




                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = false,
                //    IncludedKsi = false,
                //    IncludedP0 = true,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 290.0,
                //    MinKsi = 2,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 310.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (310.0 - 290.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic,

                //    MHStartValues = new List<MetropolisHastingsStartValue>()
                //    {
                //        new MetropolisHastingsStartValue(DisserNET.Calculs.ValueType.Kappa, 705.0),
                //    },
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = false,
                //    IncludedKsi = false,
                //    IncludedP0 = true,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 290.0,
                //    MinKsi = 2,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 310.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (310.0 - 290.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic,

                //    MHStartValues = new List<MetropolisHastingsStartValue>()
                //    {
                //        new MetropolisHastingsStartValue(DisserNET.Calculs.ValueType.Kappa, 309.0),
                //    },
                //},

                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = false,
                //    IncludedKsi = true,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 290.0,
                //    MinKsi = 3,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 310.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 16,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (310.0 - 290.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (16.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic,

                //    MHStartValues = new List<MetropolisHastingsStartValue>()
                //    {
                //        new MetropolisHastingsStartValue(DisserNET.Calculs.ValueType.Kappa, 308.8),
                //    },
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = false,
                //    IncludedKsi = true,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 290.0,
                //    MinKsi = 3,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 310.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 16,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (310.0 - 290.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (16.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic,

                //    MHStartValues = new List<MetropolisHastingsStartValue>()
                //    {
                //        new MetropolisHastingsStartValue(DisserNET.Calculs.ValueType.Kappa, 705.1),
                //    },
                //},

                #endregion

                #region P Ksi
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = false,
                //    IncludedKappa = false,
                //    IncludedKsi = true,
                //    IncludedP0 = true,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 280.0,
                //    MinKsi = 2,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 320.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic,
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = false,
                //    IncludedKappa = false,
                //    IncludedKsi = true,
                //    IncludedP0 = true,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 280.0,
                //    MinKsi = 2,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 320.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic,
                //},
                #endregion

                #region Kappa P
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = false,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = true,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 300.0,
                //    MinKsi = 2,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 350.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (350.0 - 300.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic,
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = false,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = true,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 260.0,
                //    MinKsi = 2,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 300.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic,
                //},
                #endregion

                #region Kappa Ksi
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = false,
                //    IncludedKappa = true,
                //    IncludedKsi = true,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 280.0,
                //    MinKsi = 2,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 320.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic,
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = false,
                //    IncludedKappa = true,
                //    IncludedKsi = true,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 280.0,
                //    MinKsi = 2,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 320.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic,
                //},
                #endregion

                #region K Ksi
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = false,
                //    IncludedKsi = true,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 30.0,
                //    MinKappa = (1.0 / 3600.0) * 280.0,
                //    MinKsi = 5,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 35.0, // 35
                //    MaxKappa = (1.0 / 3600.0) * 320.0,
                //    MaxKsi = 9,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (35.0 - 30.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic,
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = true,
                //    IncludedKsi = false,
                //    IncludedP0 = false,

                //    MinK = Math.Pow(10.0, -15) * 29.0,
                //    MinKappa = (1.0 / 3600.0) * 700.0,
                //    MinKsi = 3,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 710.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 29.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (710.0 - 700.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic,
                //},
#endregion

                #region K P0
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = false,
                //    IncludedKsi = false,
                //    IncludedP0 = true,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 280.0,
                //    MinKsi = 3,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0, // 35
                //    MaxKappa = (1.0 / 3600.0) * 320.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Reverse,
                //    MoveLogic = MoveLogic.Cyclic,
                //},
                //new MetropolisHastings()
                //{
                //    C = 1,
                //    WalksCount = 500000, // 250000, 500000 
                //    Ns = 10,
                //    S_0 = 0.0005, // 0.015; 0.04; 0.025 // 0.01; 0.005
                //    IncludedK = true,
                //    IncludedKappa = false,
                //    IncludedKsi = false,
                //    IncludedP0 = true,

                //    MinK = Math.Pow(10.0, -15) * 28.0,
                //    MinKappa = (1.0 / 3600.0) * 280.0,
                //    MinKsi = 3,
                //    MinP0 = Math.Pow(10.0, 6) * 13,

                //    MaxK = Math.Pow(10.0, -15) * 32.0,
                //    MaxKappa = (1.0 / 3600.0) * 320.0,
                //    MaxKsi = 7,
                //    MaxP0 = Math.Pow(10.0, 6) * 17,

                //    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                //    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                //    StepKsi = (7-3)/30.0,
                //    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                //    SelectLogic = SelectLogic.BasedOnAccepted,
                //    Mode = Mode.Direct,
                //    MoveLogic = MoveLogic.Cyclic,
                //},
#endregion

                // berem
                new MetropolisHastings()
                {
                    C = 1,
                    WalksCount = 1000000, // 250000, 500000 
                    Ns = 10,
                    S_0 = 0.05, // 0.015; 0.04; 0.025 // 0.01; 0.005
                    IncludedK = true,
                    IncludedKappa = true,
                    IncludedKsi = false,
                    IncludedP0 = true,

                    MinK = Math.Pow(10.0, -15) * 28.0,
                    MinKappa = (1.0 / 3600.0) * 280.0,
                    MinKsi = 3,
                    MinP0 = Math.Pow(10.0, 6) * 13,

                    MaxK = Math.Pow(10.0, -15) * 32.0, // 35
                    MaxKappa = (1.0 / 3600.0) * 320.0,
                    MaxKsi = 7,
                    MaxP0 = Math.Pow(10.0, 6) * 17,

                    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                    StepKsi = (7-3)/30.0,
                    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                    SelectLogic = SelectLogic.BasedOnAccepted,
                    Mode = Mode.Reverse,
                    MoveLogic = MoveLogic.Cyclic,
                },
                 new MetropolisHastings()
                {
                    C = 1,
                    WalksCount = 1000000, // 250000, 500000 
                    Ns = 10,
                    S_0 = 0.05, // 0.015; 0.04; 0.025 // 0.01; 0.005
                    IncludedK = true,
                    IncludedKappa = true,
                    IncludedKsi = true,
                    IncludedP0 = false,

                    MinK = Math.Pow(10.0, -15) * 28.0,
                    MinKappa = (1.0 / 3600.0) * 280.0,
                    MinKsi = 3,
                    MinP0 = Math.Pow(10.0, 6) * 13,

                    MaxK = Math.Pow(10.0, -15) * 32.0, // 35
                    MaxKappa = (1.0 / 3600.0) * 320.0,
                    MaxKsi = 7,
                    MaxP0 = Math.Pow(10.0, 6) * 17,

                    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                    StepKsi = (7-3)/30.0,
                    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                    SelectLogic = SelectLogic.BasedOnAccepted,
                    Mode = Mode.Reverse,
                    MoveLogic = MoveLogic.Cyclic,
                },
                 new MetropolisHastings()
                {
                    C = 1,
                    WalksCount = 1000000, // 250000, 500000 
                    Ns = 10,
                    S_0 = 0.05, // 0.015; 0.04; 0.025 // 0.01; 0.005
                    IncludedK = true,
                    IncludedKappa = false,
                    IncludedKsi = true,
                    IncludedP0 = true,

                    MinK = Math.Pow(10.0, -15) * 28.0,
                    MinKappa = (1.0 / 3600.0) * 280.0,
                    MinKsi = 3,
                    MinP0 = Math.Pow(10.0, 6) * 13,

                    MaxK = Math.Pow(10.0, -15) * 32.0, // 35
                    MaxKappa = (1.0 / 3600.0) * 320.0,
                    MaxKsi = 7,
                    MaxP0 = Math.Pow(10.0, 6) * 17,

                    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                    StepKsi = (7-3)/30.0,
                    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                    SelectLogic = SelectLogic.BasedOnAccepted,
                    Mode = Mode.Reverse,
                    MoveLogic = MoveLogic.Cyclic,
                }, 
                new MetropolisHastings()
                {
                    C = 1,
                    WalksCount = 1000000, // 250000, 500000 
                    Ns = 10,
                    S_0 = 0.05, // 0.015; 0.04; 0.025 // 0.01; 0.005
                    IncludedK = false,
                    IncludedKappa = true,
                    IncludedKsi = true,
                    IncludedP0 = true,

                    MinK = Math.Pow(10.0, -15) * 28.0,
                    MinKappa = (1.0 / 3600.0) * 280.0,
                    MinKsi = 3,
                    MinP0 = Math.Pow(10.0, 6) * 13,

                    MaxK = Math.Pow(10.0, -15) * 32.0, // 35
                    MaxKappa = (1.0 / 3600.0) * 320.0,
                    MaxKsi = 7,
                    MaxP0 = Math.Pow(10.0, 6) * 17,

                    StepK = Math.Pow(10.0, -15) * (32.0 - 28.0)/30.0,
                    StepKappa = (1.0 / 3600.0) * (320.0 - 280.0)/30.0,
                    StepKsi = (7-3)/30.0,
                    StepP0 = Math.Pow(10.0, 6) * (17.0-13.0)/30.0,

                    SelectLogic = SelectLogic.BasedOnAccepted,
                    Mode = Mode.Reverse,
                    MoveLogic = MoveLogic.Cyclic,
                },
            };


            foreach (var m in l)
            {
                TestMH(m, m.Mode);
            }
        }
    }
}
