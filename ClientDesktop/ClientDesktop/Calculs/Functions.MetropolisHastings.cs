using HydrodynamicStudies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using HydrodynamicStudies.Calculs.Helpers;

namespace HydrodynamicStudies.Calculs
{
    public partial class Functions
    {
        public static List<AcceptedValueMH> MetropolisHastingsAlgorithm(WellsList wellsListCurrent, MetropolisHastings modelMH, Mode mode = Mode.Direct)
        {
            List<AcceptedValueMH> acceptedValueMHs = new List<AcceptedValueMH>();

            // Preparations
            // calculate some initial values
            //if(mode == Mode.Direct) then do some actions
            wellsListCurrent.Wells.ForEach(x => x.Mode = mode);
            PressuresAndTimes pressuresAndTimes = GetPressures(wellsListCurrent);
            //ConsumptionsAndTimes consumptionsAndTimes = GetConsumptions(wellsListCurrent);

            System.Random rng = SystemRandomSource.Default;

            int acceptedCount = 0;

            switch (modelMH.M)
            {
                case 1:
                    for (int i = 0; i < modelMH.WalksCount; i++)
                    {
                        double w = rng.NextDouble();
                        double p = rng.NextDouble();
                        MainLogicMH(ref wellsListCurrent, modelMH, mode, acceptedValueMHs, ref acceptedCount, w, d: null, p);
                    }
                        break;
                case 2:
                    for (int i = 0; i < modelMH.WalksCount; i++)
                    {
                        double w = rng.NextDouble();
                        double d = rng.NextDouble();
                        double p = rng.NextDouble();
                        MainLogicMH(ref wellsListCurrent, modelMH, mode, acceptedValueMHs, ref acceptedCount, w, d, p);
                    }
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }

            return acceptedValueMHs;
        }

        private static void MainLogicMH(ref WellsList wellsListCurrent, MetropolisHastings modelMH, Mode mode, List<AcceptedValueMH> acceptedValueMHs, ref int acceptedCount, double w, double? d, double p)
        {
            (double currentFmin, double current_k, double current_kappa, double current_ksi, double current_p0) = GetCurrentValues(wellsListCurrent);
            #region evaluate candidates
            HCalc hCalc = new HCalc();
            (double temp_k, double temp_kappa, double temp_ksi, double temp_p0) = d.HasValue ?
                GetTempValues(modelMH, hCalc, w, d.Value, current_k, current_kappa, current_ksi, current_p0) :
                GetTempValues(modelMH, hCalc, w, current_k, current_kappa, current_ksi, current_p0);

            WellsList tempWellsList = GetWellsList(wellsListCurrent);
            UpdateWellsList(mode, temp_k, temp_kappa, temp_ksi, temp_p0, tempWellsList);

            double tempFmin = GetTempFmin(tempWellsList);
            double likelihoodValue = LikelihoodFunction(modelMH, tempFmin, currentFmin);
            double p_i = AcceptTempModelProbability(likelihoodValue, out bool accepted);
            #endregion
            acceptedCount = accepted ? ++acceptedCount : acceptedCount;

            (double next_k, double next_kappa, double next_ksi, double next_p0) =
                GetNextParameterValues(modelMH, p, current_k, current_kappa, current_ksi, current_p0, temp_k, temp_kappa, temp_ksi, temp_p0, p_i);

            wellsListCurrent = GetWellsList(wellsListCurrent);
            UpdateWellsList(mode, next_k, next_kappa, next_ksi, next_p0, wellsListCurrent);

            if (acceptedCount % modelMH.Ns == 0)
            {
                Accept(modelMH, acceptedValueMHs, acceptedCount, currentFmin, next_k, next_kappa, next_ksi, next_p0);
            }
        }

        private static void Accept(MetropolisHastings modelMH, List<AcceptedValueMH> acceptedValueMHs, int acceptedCount, double currentFmin, double next_k, double next_kappa, double next_ksi, double next_p0)
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

        private static double GetTempFmin(WellsList tempWellsList)
        {
            ConsumptionsAndTimes tempConsumptionsAndTimes = GetConsumptions(tempWellsList);
            double tempFmin = GetObjectFunctionValue(tempWellsList.Wells.ToArray());
            return tempFmin;
        }

        private static (double temp_k, double temp_kappa, double temp_ksi, double temp_p0) GetTempValues(MetropolisHastings modelMH, HCalc hCalc, double w, double current_k, double current_kappa, double current_ksi, double current_p0)
        {
            double temp_k = TempValue(modelMH.IncludedK, current_k, modelMH.StepK, modelMH.MinK, modelMH.MaxK, hCalc, w);
            double temp_kappa = TempValue(modelMH.IncludedKappa, current_kappa, modelMH.StepKappa, modelMH.MinKappa, modelMH.MaxKappa, hCalc, w);
            double temp_ksi = TempValue(modelMH.IncludedKsi, current_ksi, modelMH.StepKsi, modelMH.MinKsi, modelMH.MaxKsi, hCalc, w);
            double temp_p0 = TempValue(modelMH.IncludedP0, current_p0, modelMH.StepP0, modelMH.MinP0, modelMH.MaxP0, hCalc, w);
            return (temp_k, temp_kappa, temp_ksi, temp_p0);
        }

        private static (double temp_k, double temp_kappa, double temp_ksi, double temp_p0) GetTempValues(MetropolisHastings modelMH, HCalc hCalc, double w, double d, double current_k, double current_kappa, double current_ksi, double current_p0)
        {
            double temp_k = TempValue(modelMH.IncludedK, current_k, modelMH.StepK, modelMH.MinK, modelMH.MaxK, hCalc, w, d);
            double temp_kappa = TempValue(modelMH.IncludedKappa, current_kappa, modelMH.StepKappa, modelMH.MinKappa, modelMH.MaxKappa, hCalc, w, d);
            double temp_ksi = TempValue(modelMH.IncludedKsi, current_ksi, modelMH.StepKsi, modelMH.MinKsi, modelMH.MaxKsi, hCalc, w, d);
            double temp_p0 = TempValue(modelMH.IncludedP0, current_p0, modelMH.StepP0, modelMH.MinP0, modelMH.MaxP0, hCalc, w, d);
            return (temp_k, temp_kappa, temp_ksi, temp_p0);
        }

        private static (double currentFmin, double current_k, double current_kappa, double current_ksi, double current_p0) GetCurrentValues(WellsList wellsListCurrent)
        {
            ConsumptionsAndTimes currentConsumptionsAndTimes = GetConsumptions(wellsListCurrent);
            double currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
            double current_k = wellsListCurrent.Wells.FirstOrDefault().K;
            double current_kappa = wellsListCurrent.Wells.FirstOrDefault().Kappa;
            double current_ksi = wellsListCurrent.Wells.FirstOrDefault().Ksi;
            double current_p0 = wellsListCurrent.Wells.FirstOrDefault().P0;
            return (currentFmin, current_k, current_kappa, current_ksi, current_p0);
        }

        private static WellsList GetWellsList(WellsList wellsListCurrent)
        {
            List<Well> updatedWithTempOrNextValsWells = new List<Well>();
            updatedWithTempOrNextValsWells.AddRange(wellsListCurrent.Wells);
            WellsList tempWellsList = new WellsList(updatedWithTempOrNextValsWells);
            return tempWellsList;
        }

        private static (double next_k, double next_kappa, double next_ksi, double next_p0) GetNextParameterValues(MetropolisHastings modelMH, double p, double current_k, double current_kappa, double current_ksi, double current_p0, double temp_k, double temp_kappa, double temp_ksi, double temp_p0, double p_i)
        {
            double next_k = modelMH.IncludedK ? NextValue(p, p_i, current_k, temp_k) : temp_k;
            double next_kappa = modelMH.IncludedKappa ? NextValue(p, p_i, current_kappa, temp_kappa) : temp_kappa;
            double next_ksi = modelMH.IncludedKsi ? NextValue(p, p_i, current_ksi, temp_ksi) : temp_ksi;
            double next_p0 = modelMH.IncludedP0 ? NextValue(p, p_i, current_p0, temp_p0) : temp_p0;
            return (next_k, next_kappa, next_ksi, next_p0);
        }

        private static void UpdateWellsList(Mode mode, double temp_k, double temp_kappa, double temp_ksi, double temp_p0, WellsList tempWellsList)
        {
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
            double returnValue = 0;
            if (currentValue + H < minValue)
            {
                returnValue = 2 * minValue - currentValue - H;
            }
            else if (minValue < currentValue + H && maxValue > currentValue + H)
            {
                returnValue = currentValue + H;
            }
            else if (maxValue < currentValue + H)// && currentValue < H)
            {
                returnValue = 2 * maxValue - currentValue - H;
            }
            else
            { }
            return returnValue;
        }
    }
}
