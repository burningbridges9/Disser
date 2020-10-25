using HydrodynamicStudies.Calculs.Helpers;
using HydrodynamicStudies.Models;
using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicStudies.Calculs
{
    public partial class Functions
    {
        static object locker = new object();
        public static List<AcceptedValueMH> ParallelMetropolisHastingsAlgorithm(WellsList wellsListCurrent, MetropolisHastings modelMH, int threadsNumber, Mode mode = Mode.Direct)
        {
            List<AcceptedValueMH> acceptedValueMHs = new List<AcceptedValueMH>();

            // Preparations
            // calculate some initial values
            //if(mode == Mode.Direct) then do some actions
            wellsListCurrent.Wells.ForEach(x => x.Mode = mode);
            PressuresAndTimes pressuresAndTimes = GetPressures(wellsListCurrent);
            ConsumptionsAndTimes consumptionsAndTimes = GetConsumptions(wellsListCurrent);

            System.Random rng = SystemRandomSource.Default;
            int acceptedCount = 0;
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = threadsNumber;
            switch (modelMH.M)
            {
                case 1:
                    break;
                case 2:

                    var result = Parallel.For(fromInclusive: 0,
                        toExclusive: modelMH.WalksCount,
                        po,
                        (i, state) =>
                    {
                        HCalc hCalc = new HCalc();
                        double w;
                        double d;
                        double p;
                        lock (locker)
                        {
                            w = rng.NextDouble();
                            d = rng.NextDouble();
                            p = rng.NextDouble();
                            Console.WriteLine($"Task.CurrentId - {Task.CurrentId}; i - {i}\np = {p};\nw = {w};\nd = {d}");
                        }
                        double currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
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
                        currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());

                        if (accepted)
                        {
                            lock (locker)
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
                                    acceptedValueMHs.Add(acceptedValue);
                                }
                            }
                        }
                    });

                    break;
                case 3:
                    break;
                case 4:
                    break;
            }

            return acceptedValueMHs;
        }
    }
}
