using DisserNET.Calculs.Helpers;
using DisserNET.Models;
using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DisserNET.Calculs
{
    public partial class Functions
    {
        private static Object lockObj = new Object();

        private static int _seedCount = 0;
        private static ThreadLocal<Random> _tlRng = new ThreadLocal<Random>(() => new Random(GenerateSeed()));
        private static int GenerateSeed()
        {
            // note the usage of Interlocked, remember that in a shared context we can't just "_seedCount++"
            return (int)((DateTime.Now.Ticks << 4) + (Interlocked.Increment(ref _seedCount)));
        }

        public static List<AcceptedValueMH> ParallelMetropolisHastingsAlgorithm(WellsList wellsListCurrent, MetropolisHastings modelMH, int threadsNumber, Mode mode = Mode.Direct)
        {
            var tasks = new List<Task<List<AcceptedValueMH>>>();
            var l = new List<AcceptedValueMH>();
            for (int i = 0; i < threadsNumber; i++)
            {
                //tasks.Add(Task<List<AcceptedValueMH>>.Factory.StartNew(() => MetropolisHastingsAlgorithmForConsumptions(wellsListCurrent, modelMH, mode), TaskCreationOptions.LongRunning));
            }
            var results = new List<AcceptedValueMH>();
            Task.WaitAll(tasks.ToArray());
            tasks.ForEach(x => results.AddRange(x.Result));
            return results;
        }

        public static void ParallelMetropolisHastingsAlgorithm(object obj)
        {
            MetropolisParallelObject metropolisParallelObject = obj as MetropolisParallelObject;
            WellsList wellsListCurrent = metropolisParallelObject.WellsListCurrent;
            MetropolisHastings modelMH = metropolisParallelObject.ModelMH;
            Mode mode = metropolisParallelObject.mode;
            // Preparations
            // calculate some initial values
            //if(mode == Mode.Direct) then do some actions
            wellsListCurrent.Wells.ForEach(x => x.Mode = mode);
            PressuresAndTimes pressuresAndTimes = GetPressures(wellsListCurrent);
            ConsumptionsAndTimes consumptionsAndTimes = GetConsumptions(wellsListCurrent);


            System.Random rng = new Random();
            int acceptedCount = 0;

            switch (modelMH.M)
            {
                case 1:
                    for (int i = 0; i < modelMH.WalksCount; i++)
                    {
                        HCalc hCalc = new HCalc();
                        double w = rng.NextDouble();
                        double p = rng.NextDouble();

                        double currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        double current_k, current_kappa, current_ksi, current_p0;
                        GetCurrentValues(wellsListCurrent, out current_k, out current_kappa, out current_ksi, out current_p0);

                        #region evaluate candidates
                        double temp_k, temp_kappa, temp_ksi, temp_p0;
                        GetTempValues(modelMH, hCalc, w, current_k, current_kappa, current_ksi, current_p0, out temp_k, out temp_kappa, out temp_ksi, out temp_p0);

                        List<Well> updatedWithTempWells = new List<Well>();
                        updatedWithTempWells.AddRange(wellsListCurrent.Wells);
                        WellsList tempWellsList = new WellsList(updatedWithTempWells);
                        for (int l = 0; l < tempWellsList.Wells.Count; l++)
                        {
                            tempWellsList.Wells[l].K = temp_k;
                            tempWellsList.Wells[l].Kappa = temp_kappa;
                            tempWellsList.Wells[l].P0 = temp_p0;
                            tempWellsList.Wells[l].Ksi = temp_ksi;
                            tempWellsList.Wells[l].Mode = mode;
                            tempWellsList.Wells[l].CalcMQ = 0;
                            tempWellsList.Wells[l].CalculatedQ = 0;
                        }
                        ConsumptionsAndTimes tempConsumptionsAndTimes = GetConsumptions(tempWellsList);
                        double tempFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        double likelihoodValue = LikelihoodFunction(modelMH, tempFmin, currentFmin);
                        double p_i = AcceptTempModelProbability(likelihoodValue, out bool accepted);
                        #endregion

                        acceptedCount = accepted ? ++acceptedCount : acceptedCount;

                        double next_k, next_kappa, next_ksi, next_p0;
                        GetNextValues(modelMH, p, current_k, current_kappa, current_ksi, current_p0, temp_k, temp_kappa, temp_ksi, temp_p0, p_i, out next_k, out next_kappa, out next_ksi, out next_p0);

                        //wellsListCurrent.Clear();
                        List<Well> updatedWithNextValsWells = new List<Well>();
                        updatedWithNextValsWells.AddRange(wellsListCurrent.Wells);
                        wellsListCurrent = new WellsList(updatedWithNextValsWells);
                        for (int l = 0; l < tempWellsList.Wells.Count; l++)
                        {
                            wellsListCurrent.Wells[l].K = next_k;
                            wellsListCurrent.Wells[l].Kappa = next_kappa;
                            wellsListCurrent.Wells[l].P0 = next_p0;
                            wellsListCurrent.Wells[l].Ksi = next_ksi;
                            wellsListCurrent.Wells[l].Mode = mode;
                            wellsListCurrent.Wells[l].CalcMQ = 0;
                            wellsListCurrent.Wells[l].CalculatedQ = 0;
                        }

                        ConsumptionsAndTimes nextConsumptionsAndTimes = GetConsumptions(wellsListCurrent);
                        currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());

                        if (acceptedCount % modelMH.Ns == 0)
                        {
                            AcceptedValueMH acceptedValue = new AcceptedValueMH()
                            {
                                AcceptedCount = acceptedCount,
                                ProbabilityDensity = likelihoodValue,
                                Fmin = currentFmin,
                                K = next_k,
                                Kappa = next_kappa,
                                Ksi = next_ksi,
                                P0 = next_p0,
                                IncludedK = modelMH.IncludedK,
                                IncludedKappa = modelMH.IncludedKappa,
                                IncludedKsi = modelMH.IncludedKsi,
                                IncludedP0 = modelMH.IncludedP0,
                            };
                            metropolisParallelObject.AcceptedValues.Add(acceptedValue);
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < modelMH.WalksCount; i++)
                    {

                        HCalc hCalc = new HCalc();
                        double w;
                        double d;
                        double p;

                        lock (lockObj)
                        {
                            //Console.WriteLine($"i = {i}");
                            //Console.WriteLine($"ThreadId = {Thread.CurrentThread.ManagedThreadId}");
                            w = _tlRng.Value.NextDouble();
                            d = _tlRng.Value.NextDouble();
                            p = _tlRng.Value.NextDouble();
                            //Console.WriteLine($"w = {w}");
                            //Console.WriteLine($"d = {d}");
                            //Console.WriteLine($"p = {p}");
                        }
                        GetConsumptions(wellsListCurrent);
                        double currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        //Console.WriteLine($"currentFmin = {currentFmin}");

                        double current_k, current_kappa, current_ksi, current_p0;
                        GetCurrentValues(wellsListCurrent, out current_k, out current_kappa, out current_ksi, out current_p0);

                        #region evaluate candidates
                        double temp_k, temp_kappa, temp_ksi, temp_p0;
                        GetTempValues(modelMH, hCalc, w, d, current_k, current_kappa, current_ksi, current_p0, out temp_k, out temp_kappa, out temp_ksi, out temp_p0);
                        List<Well> updatedWithTempWells = new List<Well>();
                        updatedWithTempWells.AddRange(wellsListCurrent.Wells);
                        WellsList tempWellsList = new WellsList(updatedWithTempWells);
                        for (int l = 0; l < tempWellsList.Wells.Count; l++)
                        {
                            tempWellsList.Wells[l].K = temp_k;
                            tempWellsList.Wells[l].Kappa = temp_kappa;
                            tempWellsList.Wells[l].P0 = temp_p0;
                            tempWellsList.Wells[l].Ksi = temp_ksi;
                            tempWellsList.Wells[l].Mode = mode;
                            tempWellsList.Wells[l].CalcMQ = 0;
                            tempWellsList.Wells[l].CalculatedQ = 0;
                        }
                        ConsumptionsAndTimes tempConsumptionsAndTimes = GetConsumptions(tempWellsList);
                        double tempFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        //Console.WriteLine($"temp_k = {temp_k}");
                        //Console.WriteLine($"temp_kappa = {temp_kappa}");
                        //Console.WriteLine($"tempFmin = {tempFmin}");
                        double likelihoodValue = LikelihoodFunction(modelMH, tempFmin, currentFmin);
                        double p_i = AcceptTempModelProbability(likelihoodValue, out bool accepted);
                        #endregion

                        double next_k, next_kappa, next_ksi, next_p0;
                        GetNextValues(modelMH, p, current_k, current_kappa, current_ksi, current_p0, temp_k, temp_kappa, temp_ksi, temp_p0, p_i, out next_k, out next_kappa, out next_ksi, out next_p0);


                        List<Well> updatedWithNextValsWells = new List<Well>();
                        updatedWithNextValsWells.AddRange(wellsListCurrent.Wells);
                        wellsListCurrent = new WellsList(updatedWithNextValsWells);
                        for (int l = 0; l < tempWellsList.Wells.Count; l++)
                        {
                            wellsListCurrent.Wells[l].K = next_k;
                            wellsListCurrent.Wells[l].Kappa = next_kappa;
                            wellsListCurrent.Wells[l].P0 = next_p0;
                            wellsListCurrent.Wells[l].Ksi = next_ksi;
                            wellsListCurrent.Wells[l].Mode = mode;
                            wellsListCurrent.Wells[l].CalcMQ = 0;
                            wellsListCurrent.Wells[l].CalculatedQ = 0;
                        }
                        ConsumptionsAndTimes nextConsumptionsAndTimes = GetConsumptions(wellsListCurrent);


                        lock (lockObj)
                        {
                            Console.WriteLine($"i = {i}");
                            Console.WriteLine($"ThreadId = {Thread.CurrentThread.ManagedThreadId}");
                            Console.WriteLine($"acceptedCount = {acceptedCount}");
                            Console.WriteLine($"next_k = {next_k}");
                            Console.WriteLine($"next_kappa = {next_kappa}");
                            Console.WriteLine($"currentFmin = {currentFmin}");
                            Console.WriteLine($"tempFmin = {tempFmin}");
                        }


                        currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        if (accepted)
                        {
                            ++acceptedCount;
                            if (acceptedCount % modelMH.Ns == 0)
                            {
                                AcceptedValueMH acceptedValue = new AcceptedValueMH()
                                {
                                    AcceptedCount = acceptedCount,
                                    Fmin = currentFmin,
                                    K = next_k,
                                    Kappa = next_kappa,
                                    Ksi = next_ksi,
                                    P0 = next_p0,
                                    IncludedK = modelMH.IncludedK,
                                    IncludedKappa = modelMH.IncludedKappa,
                                    IncludedKsi = modelMH.IncludedKsi,
                                    IncludedP0 = modelMH.IncludedP0,
                                };
                                metropolisParallelObject.AcceptedValues.Add(acceptedValue);
                            }
                        }
                    }
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }

    }
}
