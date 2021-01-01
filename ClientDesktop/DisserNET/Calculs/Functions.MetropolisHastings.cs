using DisserNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using DisserNET.Calculs.Helpers;
using System.Threading;

namespace DisserNET.Calculs
{
    public partial class Functions
    {
        public static Action<AcceptedValueMH> OnAcceptAction;

        public static List<AcceptedValueMH> MetropolisHastingsAlgorithm(WellsList wellsListCurrent, MetropolisHastings modelMH, Mode mode = Mode.Direct)
        {
            List<AcceptedValueMH> acceptedValueMHs = new List<AcceptedValueMH>();

            // Preparations
            // calculate some initial values
            //if(mode == Mode.Direct) then do some actions
            wellsListCurrent.Wells.ForEach(x => x.Mode = mode);
            PressuresAndTimes pressuresAndTimes = GetPressures(wellsListCurrent);
            GetConsumptions(wellsListCurrent);
            double currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
            System.Random rng = SystemRandomSource.Default;
            int acceptedCount = 0;
            bool accepted = false;
            bool first = true;
            switch (modelMH.M)
            {
                case 1:

                    for (int i = 0; i < modelMH.WalksCount; i++)
                    {
                        Console.WriteLine($"i = {i}");
                        HCalc hCalc = new HCalc();
                        double w = StaticRandom.Rand(); //rng.NextDouble();
                        double p = StaticRandom.Rand(); //rng.NextDouble();

                        GetConsumptions(wellsListCurrent);
                        currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        GetCurrentValues(wellsListCurrent, out double current_k, out double current_kappa, out double current_ksi, out double current_p0);

                        #region evaluate candidates
                        GetTempValues(modelMH, hCalc, w, current_k, current_kappa, current_ksi, current_p0,
                            out double temp_k, out double temp_kappa, out double temp_ksi, out double temp_p0);

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
                        double p_i = AcceptTempModelProbability(likelihoodValue, out accepted);
                        #endregion


                        GetNextValues(modelMH, p, current_k, current_kappa, current_ksi, current_p0, temp_k, temp_kappa, temp_ksi, temp_p0, p_i,
                            out double next_k, out double next_kappa, out double next_ksi, out double next_p0);

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
                        }

                        switch (modelMH.SelectLogic)
                        {
                            case SelectLogic.BasedOnAccepted:
                                if (accepted)
                                {
                                    ++acceptedCount;
                                    Console.WriteLine($"acceptedCount = {acceptedCount}");
                                    if (acceptedCount % modelMH.Ns == 0)
                                    {
                                        GetConsumptions(wellsListCurrent);
                                        currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
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
                                break;
                            case SelectLogic.BasedOnWalks:
                                if ((i + 1) % modelMH.Ns == 0)
                                {
                                    GetConsumptions(wellsListCurrent);
                                    currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
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
                                    Console.WriteLine($"selectedCount = {acceptedValueMHs.Count}");
                                }
                                if (accepted)
                                {
                                    ++acceptedCount;
                                    Console.WriteLine($"acceptedCount = {acceptedCount}");
                                }
                                break;
                            case SelectLogic.AcceptAll:
                                ++acceptedCount;
                                Console.WriteLine($"acceptedCount = {acceptedCount}");
                                if (true)
                                {
                                    GetConsumptions(wellsListCurrent);
                                    currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
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
                                break;
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < modelMH.WalksCount; i++)
                    {
                        Console.WriteLine($"i = {i}");
                        HCalc hCalc = new HCalc();
                        double w = rng.NextDouble();
                        double d = rng.NextDouble();
                        double p = rng.NextDouble();

                        GetConsumptions(wellsListCurrent);
                        currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        GetCurrentValues(wellsListCurrent, out double current_k, out double current_kappa, out double current_ksi, out double current_p0);
                        if (first)
                        {
                            if (current_k > modelMH.MaxK)
                                current_k = modelMH.MaxK;
                            else if (current_k < modelMH.MinK)
                                current_k = modelMH.MaxK;

                            if (current_kappa > modelMH.MaxKappa)
                                current_kappa = modelMH.MaxKappa;
                            else if (current_kappa < modelMH.MinKappa)
                                current_kappa = modelMH.MaxK;

                            //if (current_ksi > modelMH.MaxKsi)
                            //    current_ksi = modelMH.MaxKsi;
                            //else if (current_ksi < modelMH.MaxKsi)
                            //    current_ksi = modelMH.MaxKsi;

                            //if (current_p0 > modelMH.MaxP0)
                            //    current_p0 = modelMH.MaxP0;
                            //else if (current_p0 < modelMH.MaxP0)
                            //    current_p0 = modelMH.MaxP0;


                            first = false;
                        }

                        #region evaluate candidates
                        GetTempValues(modelMH, hCalc, w, d, current_k, current_kappa, current_ksi, current_p0,
                            out double temp_k, out double temp_kappa, out double temp_ksi, out double temp_p0);
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
                        double p_i = modelMH.SelectLogic == SelectLogic.AcceptAll ? 1 : AcceptTempModelProbability(likelihoodValue, out accepted);
                        #endregion

                        GetNextValues(modelMH, p, current_k, current_kappa, current_ksi, current_p0, temp_k, temp_kappa, temp_ksi, temp_p0, p_i,
                            out double next_k, out double next_kappa, out double next_ksi, out double next_p0);


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
                        }

                        switch (modelMH.SelectLogic)
                        {
                            case SelectLogic.BasedOnAccepted:
                                if (accepted)
                                {
                                    ++acceptedCount;
                                    Console.WriteLine($"acceptedCount = {acceptedCount}");
                                    if (acceptedCount % modelMH.Ns == 0)
                                    {
                                        GetConsumptions(wellsListCurrent);
                                        currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
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
                                break;
                            case SelectLogic.BasedOnWalks:
                                if ((i + 1) % modelMH.Ns == 0)
                                {
                                    GetConsumptions(wellsListCurrent);
                                    currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
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
                                    Console.WriteLine($"selectedCount = {acceptedValueMHs.Count}");
                                }
                                if (accepted)
                                {
                                    ++acceptedCount;
                                    Console.WriteLine($"acceptedCount = {acceptedCount}");
                                }
                                break;
                            case SelectLogic.AcceptAll:
                                ++acceptedCount;
                                Console.WriteLine($"acceptedCount = {acceptedCount}");
                                if (true)
                                {
                                    GetConsumptions(wellsListCurrent);
                                    currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
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
                                break;
                        }

                    }
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }

            return acceptedValueMHs;
        }

        private static void GetTempValues(MetropolisHastings modelMH, HCalc hCalc, double w, double current_k, double current_kappa, double current_ksi, double current_p0, out double temp_k, out double temp_kappa, out double temp_ksi, out double temp_p0)
        {
            temp_k = TempValue(modelMH.IncludedK, current_k, modelMH.StepK, modelMH.MinK, modelMH.MaxK, hCalc, w);
            temp_kappa = TempValue(modelMH.IncludedKappa, current_kappa, modelMH.StepKappa, modelMH.MinKappa, modelMH.MaxKappa, hCalc, w);
            temp_ksi = TempValue(modelMH.IncludedKsi, current_ksi, modelMH.StepKsi, modelMH.MinKsi, modelMH.MaxKsi, hCalc, w);
            temp_p0 = TempValue(modelMH.IncludedP0, current_p0, modelMH.StepP0, modelMH.MinP0, modelMH.MaxP0, hCalc, w);
        }

        private static void GetNextValues(MetropolisHastings modelMH, double p, double current_k, double current_kappa, double current_ksi, double current_p0, double temp_k, double temp_kappa, double temp_ksi, double temp_p0, double p_i, out double next_k, out double next_kappa, out double next_ksi, out double next_p0)
        {
            next_k = modelMH.IncludedK ? NextValue(p, p_i, current_k, temp_k) : temp_k;
            next_kappa = modelMH.IncludedKappa ? NextValue(p, p_i, current_kappa, temp_kappa) : temp_kappa;
            next_ksi = modelMH.IncludedKsi ? NextValue(p, p_i, current_ksi, temp_ksi) : temp_ksi;
            next_p0 = modelMH.IncludedP0 ? NextValue(p, p_i, current_p0, temp_p0) : temp_p0;
        }

        private static void GetTempValues(MetropolisHastings modelMH, HCalc hCalc, double w, double d, double current_k, double current_kappa, double current_ksi, double current_p0, out double temp_k, out double temp_kappa, out double temp_ksi, out double temp_p0)
        {
            temp_k = TempValue(modelMH.IncludedK, current_k, modelMH.StepK, modelMH.MinK, modelMH.MaxK, hCalc, w, d);
            temp_kappa = TempValue(modelMH.IncludedKappa, current_kappa, modelMH.StepKappa, modelMH.MinKappa, modelMH.MaxKappa, hCalc, w, d);
            temp_ksi = TempValue(modelMH.IncludedKsi, current_ksi, modelMH.StepKsi, modelMH.MinKsi, modelMH.MaxKsi, hCalc, w, d);
            temp_p0 = TempValue(modelMH.IncludedP0, current_p0, modelMH.StepP0, modelMH.MinP0, modelMH.MaxP0, hCalc, w, d);
        }

        private static void GetCurrentValues(WellsList wellsListCurrent, out double current_k, out double current_kappa, out double current_ksi, out double current_p0)
        {
            current_k = wellsListCurrent.Wells.FirstOrDefault().K;
            current_kappa = wellsListCurrent.Wells.FirstOrDefault().Kappa;
            current_ksi = wellsListCurrent.Wells.FirstOrDefault().Ksi;
            current_p0 = wellsListCurrent.Wells.FirstOrDefault().P0;
        }

        private static double TempValue(bool included, double current, double step, double minVal, double maxVal, HCalc hCalc, double w)
        {
            double temp;
            if (included)
            {
                double H_k = hCalc.NextH(step, w);
                temp = TemporaryValueCalcWithBoundaries(minVal, maxVal, H_k, current);
            }
            else
            {
                temp = current;
            }
            return temp;
        }

        private static double TempValue(bool included, double current, double step, double minVal, double maxVal, HCalc hCalc, double w, double d)
        {
            double temp;
            if (included)
            {
                double H_k = hCalc.NextH(step, w, d);
                temp = TemporaryValueCalcWithBoundaries(minVal, maxVal, H_k, current);
            }
            else
            {
                temp = current;
            }
            return temp;
        }

        private static double NextValue(double p, double p_i, double current, double temp)
        {
            return 0.5 * (temp * (1 + Math.Sign(p_i - p)) + current * (1 + Math.Sign(p - p_i)));
        }

        private static double LikelihoodFunction(MetropolisHastings modelMH, double s_i_temp, double s_i_current)
        {
            double retValue = 1;
            var sSquare = s_i_temp - s_i_current;
            var denom = 2 * Math.Pow(modelMH.S_0, 2) * (modelMH.M + 1);
            retValue = modelMH.C * Math.Exp(-sSquare / denom);
            return retValue;
        }

        private static double AcceptTempModelProbability(double likelihoodValue, out bool accepted)
        {
            var retValue = Math.Min(1.0, likelihoodValue);
            accepted = retValue == 1.0;
            return retValue;
        }

        private static double TemporaryValueCalcWithBoundaries(double minValue, double maxValue, double H, double currentValue)
        {
            double returnValue = currentValue;
            if (currentValue + H < minValue)
            {
                returnValue = maxValue - (minValue - (currentValue + H));
            }
            else if (minValue <= currentValue + H && maxValue >= currentValue + H)
            {
                returnValue = currentValue + H;
            }
            else if (maxValue < currentValue + H)
            {
                returnValue = minValue + ((currentValue + H) - maxValue);
            }
            return returnValue;
        }
    }
}
