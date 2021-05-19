using DisserNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;
using DisserNET.Calculs.Helpers;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace DisserNET.Calculs
{
    public partial class Functions
    {
        public static void MetropolisHastingsAlgorithmForConsumptions(WellsList wellsListCurrent, MetropolisHastings modelMH, List<AcceptedValueMH> acceptedValueMHs,  Mode mode = Mode.Direct)
        {
            // Preparations
            // calculate some initial values
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

                        TryAccept(wellsListCurrent, modelMH, acceptedValueMHs, ref currentFmin, ref acceptedCount, accepted, i, next_k, next_kappa, next_ksi, next_p0);
                    }
                    break;
                case 2:
                    for (int i = 0; i < modelMH.WalksCount; i++)
                    {
                        //Console.WriteLine($"i = {i}");
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
                                current_k = modelMH.MinK;

                            if (current_kappa > modelMH.MaxKappa)
                                current_kappa = modelMH.MaxKappa;
                            else if (current_kappa < modelMH.MinKappa)
                                current_kappa = modelMH.MinKappa;

                            if (current_ksi > modelMH.MaxKsi)
                                current_ksi = modelMH.MaxKsi;
                            else if (current_ksi < modelMH.MinKsi)
                                current_ksi = modelMH.MinKsi;

                            if (current_p0 > modelMH.MaxP0)
                                current_p0 = modelMH.MaxP0;
                            else if (current_p0 < modelMH.MinP0)
                                current_p0 = modelMH.MinP0;

                            foreach (var v in modelMH.MHStartValues)
                            {
                                switch (v.ValueType)
                                {
                                    case ValueType.K:
                                        current_k = v.Value;
                                        break;
                                    case ValueType.Kappa:
                                        current_kappa = v.Value;
                                        break;
                                    case ValueType.Ksi:
                                        current_ksi = v.Value;
                                        break;
                                    case ValueType.P:
                                        current_p0 = v.Value;
                                        break;
                                }
                            }

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

                        TryAccept(wellsListCurrent, modelMH, acceptedValueMHs, ref currentFmin, ref acceptedCount, accepted, i, next_k, next_kappa, next_ksi, next_p0);

                    }
                    break;
                case 3:
                    for (int i = 0; i < modelMH.WalksCount; i++)
                    {
                        //Console.WriteLine($"i = {i}");
                        HCalc hCalc = new HCalc();
                        double w = rng.NextDouble();
                        double p = rng.NextDouble();

                        GetConsumptions(wellsListCurrent, prepareStatic: false);
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

                            if (current_ksi > modelMH.MaxKsi)
                                current_ksi = modelMH.MaxKsi;
                            else if (current_ksi < modelMH.MinKsi)
                                current_ksi = modelMH.MaxKsi;

                            if (current_p0 > modelMH.MaxP0)
                                current_p0 = modelMH.MaxP0;
                            else if (current_p0 < modelMH.MinP0)
                                current_p0 = modelMH.MaxP0;
                            foreach (var v in modelMH.MHStartValues)
                            {
                                switch (v.ValueType)
                                {
                                    case ValueType.K:
                                        current_k = v.Value;
                                        break;
                                    case ValueType.Kappa:
                                        current_kappa = v.Value;
                                        break;
                                    case ValueType.Ksi:
                                        current_ksi = v.Value;
                                        break;
                                    case ValueType.P:
                                        current_p0 = v.Value;
                                        break;
                                }
                            }

                            first = false;
                        }

                        #region evaluate candidates
                        GetTempValues(modelMH, hCalc, w, 3, current_k, current_kappa, current_ksi, current_p0,
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
                        ConsumptionsAndTimes tempConsumptionsAndTimes = GetConsumptions(tempWellsList, prepareStatic: false);
                        double tempFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        double likelihoodValue = LikelihoodFunction(modelMH, tempFmin, currentFmin);
                        //double p_i = modelMH.SelectLogic == SelectLogic.AcceptAll ? 1 : AcceptTempModelProbability(likelihoodValue, out accepted);
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

                        TryAccept(wellsListCurrent, modelMH, acceptedValueMHs, ref currentFmin, ref acceptedCount, accepted, i, next_k, next_kappa, next_ksi, next_p0);

                    }
                    break;
                case 4:
                    break;
            }

            //return acceptedValueMHs;
        }

        public static void MetropolisHastingsAlgorithmForPressures(WellsList wellsListCurrent, MetropolisHastings modelMH, List<AcceptedValueMH> acceptedValueMHs, Mode mode = Mode.Reverse)
        {
            // Preparations
            // calculate some initial values
            wellsListCurrent.Wells.ForEach(x => x.Mode = mode);
            ConsumptionsAndTimes consumptionsAndTimes = GetConsumptions(wellsListCurrent);
            GetPressures(wellsListCurrent);
            double currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
            System.Random rng = new Random();
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

                        GetPressures(wellsListCurrent);
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
                            tempWellsList.Wells[l].CalcMP = 0;
                            tempWellsList.Wells[l].CalculatedP = 0;
                        }
                        PressuresAndTimes tempPressuresAndTimes = GetPressures(tempWellsList);
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
                                        GetPressures(wellsListCurrent);
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
                                    GetPressures(wellsListCurrent);
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
                                    GetPressures(wellsListCurrent);
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
                        HCalc hCalc = new HCalc();
                        double w= rng.NextDouble();
                        double d= rng.NextDouble();
                        double p= rng.NextDouble();
                        //Console.WriteLine($"i = {i}");

                        GetPressures(wellsListCurrent);
                        currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        GetCurrentValues(wellsListCurrent, out double current_k, out double current_kappa, out double current_ksi, out double current_p0);
                        if (first)
                        {
                            if (current_k > modelMH.MaxK)
                                current_k = modelMH.MaxK;
                            else if (current_k < modelMH.MinK)
                                current_k = modelMH.MinK;

                            if (current_kappa > modelMH.MaxKappa)
                                current_kappa = modelMH.MaxKappa;
                            else if (current_kappa < modelMH.MinKappa)
                                current_kappa = modelMH.MinKappa;

                            //current_kappa = (1.0 / 3600.0) * 310;

                            if (current_ksi > modelMH.MaxKsi)
                                current_ksi = modelMH.MaxKsi;
                            else if (current_ksi < modelMH.MinKsi)
                                current_ksi = modelMH.MinKsi;

                            if (current_p0 > modelMH.MaxP0)
                                current_p0 = modelMH.MaxP0;
                            else if (current_p0 < modelMH.MinP0)
                                current_p0 = modelMH.MinP0;

                            foreach (var v in modelMH.MHStartValues)
                            {
                                switch (v.ValueType)
                                {
                                    case ValueType.K:
                                        current_k = v.Value;
                                        break;
                                    case ValueType.Kappa:
                                        current_kappa = v.Value;
                                        break;
                                    case ValueType.Ksi:
                                        current_ksi = v.Value;
                                        break;
                                    case ValueType.P:
                                        current_p0 = v.Value;
                                        break;
                                }
                            }

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
                            tempWellsList.Wells[l].CalcMP = 0;
                            tempWellsList.Wells[l].CalculatedP = 0;
                        }
                        PressuresAndTimes tempPressuresAndTimes = GetPressures(tempWellsList);
                        double tempFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        double likelihoodValue = LikelihoodFunction(modelMH, tempFmin, currentFmin);
                        //double p_i = modelMH.SelectLogic == SelectLogic.AcceptAll ? 1 : AcceptTempModelProbability(likelihoodValue, out accepted);
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
                        TryAccept(wellsListCurrent, modelMH, acceptedValueMHs, ref currentFmin, ref acceptedCount, accepted, i, next_k, next_kappa, next_ksi, next_p0);                        
                    }
                    break;
                case 3:
                    for (int i = 0; i < modelMH.WalksCount; i++)
                    {
                        HCalc hCalc = new HCalc();
                        double w = rng.NextDouble();
                        double p = rng.NextDouble();

                        GetPressures(wellsListCurrent, prepareStatic: false);
                        currentFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        GetCurrentValues(wellsListCurrent, out double current_k, out double current_kappa, out double current_ksi, out double current_p0);
                        if (first)
                        {
                            if (current_k > modelMH.MaxK)
                                current_k = modelMH.MaxK;
                            else if (current_k < modelMH.MinK)
                                current_k = modelMH.MinK;

                            if (current_kappa > modelMH.MaxKappa)
                                current_kappa = modelMH.MaxKappa;
                            else if (current_kappa < modelMH.MinKappa)
                                current_kappa = modelMH.MinKappa;

                            if (current_ksi > modelMH.MaxKsi)
                                current_ksi = modelMH.MaxKsi;
                            else if (current_ksi < modelMH.MinKsi)
                                current_ksi = modelMH.MinKsi;

                            if (current_p0 > modelMH.MaxP0)
                                current_p0 = modelMH.MaxP0;
                            else if (current_p0 < modelMH.MinP0)
                                current_p0 = modelMH.MinP0;

                            foreach (var v in modelMH.MHStartValues)
                            {
                                switch (v.ValueType)
                                {
                                    case ValueType.K:
                                        current_k = v.Value;
                                        break;
                                    case ValueType.Kappa:
                                        current_kappa = v.Value;
                                        break;
                                    case ValueType.Ksi:
                                        current_ksi = v.Value;
                                        break;
                                    case ValueType.P:
                                        current_p0 = v.Value;
                                        break;
                                }
                            }

                            first = false;
                        }

                        #region evaluate candidates
                        GetTempValues(modelMH, hCalc, w, 3, current_k, current_kappa, current_ksi, current_p0,
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
                            tempWellsList.Wells[l].CalcMP = 0;
                            tempWellsList.Wells[l].CalculatedP = 0;
                        }
                        PressuresAndTimes tempPressuresAndTimes = GetPressures(tempWellsList, prepareStatic: false);
                        double tempFmin = GetObjectFunctionValue(wellsListCurrent.Wells.ToArray());
                        double likelihoodValue = LikelihoodFunction(modelMH, tempFmin, currentFmin);
                        //double p_i = modelMH.SelectLogic == SelectLogic.AcceptAll ? 1 : AcceptTempModelProbability(likelihoodValue, out accepted);
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
                        TryAccept(wellsListCurrent, modelMH, acceptedValueMHs, ref currentFmin, ref acceptedCount, accepted, i, next_k, next_kappa, next_ksi, next_p0);
                    }
                    break;
                case 4:
                    break;
            }

        }


        private static void TryAccept(WellsList wellsListCurrent, MetropolisHastings modelMH, List<AcceptedValueMH> acceptedValueMHs,
            ref double currentFmin, ref int acceptedCount, bool accepted, int i, double next_k, double next_kappa, double next_ksi, double next_p0)
        {
            if (accepted)
            {
                ++acceptedCount;
                //Console.WriteLine($"acceptedCount = {acceptedCount}");
                if (acceptedCount % modelMH.Ns == 0)
                {
                    Console.WriteLine($"i = {i}");
                    if (wellsListCurrent.Wells.First().Mode == Mode.Direct)
                        GetConsumptions(wellsListCurrent, prepareStatic: false);
                    else
                        GetPressures(wellsListCurrent, prepareStatic: false);
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
            return;
            switch (modelMH.SelectLogic)
            {
                case SelectLogic.BasedOnAccepted:
                    if (accepted)
                    {
                        ++acceptedCount;
                        //Console.WriteLine($"acceptedCount = {acceptedCount}");
                        if (acceptedCount % modelMH.Ns == 0)
                        {
                            if (wellsListCurrent.Wells.First().Mode == Mode.Direct)
                                GetConsumptions(wellsListCurrent);
                            else
                                GetPressures(wellsListCurrent);
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
                        if (wellsListCurrent.Wells.First().Mode == Mode.Direct)
                            GetConsumptions(wellsListCurrent);
                        else
                            GetPressures(wellsListCurrent);
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
                        //Console.WriteLine($"selectedCount = {acceptedValueMHs.Count}");
                    }
                    if (accepted)
                    {
                        ++acceptedCount;
                        //Console.WriteLine($"acceptedCount = {acceptedCount}");
                    }
                    break;
                case SelectLogic.AcceptAll:
                    ++acceptedCount;
                    Console.WriteLine($"acceptedCount = {acceptedCount}");
                    if (true)
                    {
                        if (wellsListCurrent.Wells.First().Mode == Mode.Direct)
                            GetConsumptions(wellsListCurrent);
                        else
                            GetPressures(wellsListCurrent);
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
                        //App.Current.Dispatcher.Invoke((Action)delegate
                        //{
                        //    acceptedValueMHs.Add(acceptedValue);
                        //});
                        acceptedValueMHs.Add(acceptedValue);
                    }
                    break;
            }
        }

        private static void GetTempValues(MetropolisHastings modelMH, HCalc hCalc, double w, double current_k, double current_kappa, double current_ksi, double current_p0, out double temp_k, out double temp_kappa, out double temp_ksi, out double temp_p0)
        {
            temp_k = TempValue(modelMH.IncludedK, current_k, modelMH.StepK, modelMH.MinK, modelMH.MaxK, hCalc, w, modelMH.MoveLogic);
            temp_kappa = TempValue(modelMH.IncludedKappa, current_kappa, modelMH.StepKappa, modelMH.MinKappa, modelMH.MaxKappa, hCalc, w, modelMH.MoveLogic);
            temp_ksi = TempValue(modelMH.IncludedKsi, current_ksi, modelMH.StepKsi, modelMH.MinKsi, modelMH.MaxKsi, hCalc, w, modelMH.MoveLogic);
            temp_p0 = TempValue(modelMH.IncludedP0, current_p0, modelMH.StepP0, modelMH.MinP0, modelMH.MaxP0, hCalc, w, modelMH.MoveLogic);
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
            temp_k = TempValue(modelMH.IncludedK, current_k, modelMH.StepK, modelMH.MinK, modelMH.MaxK, hCalc, w, d, modelMH.MoveLogic);
            temp_kappa = TempValue(modelMH.IncludedKappa, current_kappa, modelMH.StepKappa, modelMH.MinKappa, modelMH.MaxKappa, hCalc, w, d, modelMH.MoveLogic);
            temp_ksi = TempValue(modelMH.IncludedKsi, current_ksi, modelMH.StepKsi, modelMH.MinKsi, modelMH.MaxKsi, hCalc, w, d, modelMH.MoveLogic);
            temp_p0 = TempValue(modelMH.IncludedP0, current_p0, modelMH.StepP0, modelMH.MinP0, modelMH.MaxP0, hCalc, w, d, modelMH.MoveLogic);
        }


        private static void GetTempValues(MetropolisHastings modelMH, HCalc hCalc, double w, int varCount, double current_k, double current_kappa, double current_ksi, double current_p0, out double temp_k, out double temp_kappa, out double temp_ksi, out double temp_p0)
        {
            temp_k = TempValue(modelMH.IncludedK, current_k, modelMH.StepK, modelMH.MinK, modelMH.MaxK, hCalc, w, varCount, modelMH.MoveLogic);
            temp_kappa = TempValue(modelMH.IncludedKappa, current_kappa, modelMH.StepKappa, modelMH.MinKappa, modelMH.MaxKappa, hCalc, w, varCount, modelMH.MoveLogic);
            temp_ksi = TempValue(modelMH.IncludedKsi, current_ksi, modelMH.StepKsi, modelMH.MinKsi, modelMH.MaxKsi, hCalc, w, varCount, modelMH.MoveLogic);
            temp_p0 = TempValue(modelMH.IncludedP0, current_p0, modelMH.StepP0, modelMH.MinP0, modelMH.MaxP0, hCalc, w, varCount, modelMH.MoveLogic);
        }

        private static void GetCurrentValues(WellsList wellsListCurrent, out double current_k, out double current_kappa, out double current_ksi, out double current_p0)
        {
            current_k = wellsListCurrent.Wells.FirstOrDefault().K;
            current_kappa = wellsListCurrent.Wells.FirstOrDefault().Kappa;
            current_ksi = wellsListCurrent.Wells.FirstOrDefault().Ksi;
            current_p0 = wellsListCurrent.Wells.FirstOrDefault().P0;
        }

        private static double TempValue(bool included, double current, double step, double minVal, double maxVal, HCalc hCalc, double w, MoveLogic moveLogic)
        {
            double temp;
            if (included)
            {
                double H_k = hCalc.NextH(step, w);
                temp = TemporaryValueCalcWithBoundaries(minVal, maxVal, H_k, current, moveLogic);
            }
            else
            {
                temp = current;
            }
            return temp;
        }

        private static double TempValue(bool included, double current, double step, double minVal, double maxVal, HCalc hCalc, double w, double d, MoveLogic moveLogic)
        {
            double temp;
            if (included)
            {
                double H_k = hCalc.NextH(step, w, d);
                temp = TemporaryValueCalcWithBoundaries(minVal, maxVal, H_k, current, moveLogic);
            }
            else
            {
                temp = current;
            }
            return temp;
        }

        private static double TempValue(bool included, double current, double step, double minVal, double maxVal, HCalc hCalc, double w, int varCount, MoveLogic moveLogic)
        {
            double temp;
            if (included)
            {
                double H_k = hCalc.NextH(step, w, varCount);
                temp = TemporaryValueCalcWithBoundaries(minVal, maxVal, H_k, current, moveLogic);
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

        private static double TemporaryValueCalcWithBoundaries(double minValue, double maxValue, double H, double currentValue, MoveLogic moveLogic = MoveLogic.Cyclic)
        {
            double returnValue = currentValue;
            return CyclicMove(minValue, maxValue, H, currentValue);
            switch (moveLogic)
            {
                case MoveLogic.Cyclic:
                    return CyclicMove(minValue, maxValue, H, currentValue);

                case MoveLogic.StickToBorder:
                    if (currentValue + H < minValue)
                    {
                        returnValue = minValue;
                    }
                    else if (minValue <= currentValue + H && maxValue >= currentValue + H)
                    {
                        returnValue = currentValue + H;
                    }
                    else if (maxValue < currentValue + H)
                    {
                        returnValue = maxValue;
                    }
                    return returnValue;

                case MoveLogic.StepBack:
                    if (currentValue + H < minValue)
                    {
                        returnValue = 2 * minValue - currentValue - H;
                    }
                    else if (minValue <= currentValue + H && maxValue >= currentValue + H)
                    {
                        returnValue = currentValue + H;
                    }
                    else if (maxValue < currentValue + H)
                    {
                        returnValue = 2 * maxValue - currentValue - H;
                    }
                    return returnValue;

                case MoveLogic.Reject:
                    if (currentValue + H < minValue || maxValue < currentValue + H)
                    {
                        returnValue = currentValue;
                    }
                    else if (minValue <= currentValue + H && maxValue >= currentValue + H)
                    {
                        returnValue = currentValue + H;
                    }
                    return returnValue;

                default:
                    return returnValue;
            }
        }

        private static double CyclicMove(double minValue, double maxValue, double H, double currentValue)
        {
            double returnValue = 0;
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
