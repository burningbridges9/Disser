using DisserNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DisserNET.Calculs
{
    public partial class Functions
    {
        #region Prepare slae
        public static void PrepareEqPressures(WellsList wells, out List<double> eqPressures)
        {
            eqPressures = new List<double>();
            
            if (wells.Wells[0].Mode == Mode.Direct)
            {
                switch (wells.Wells.Count)
                {
                    case 1:
                        for (int i = 0; i < wells.Indexes[0]; i++)
                            eqPressures.Add(wells.Wells[0].CalculatedP - wells.Wells[0].P0);
                        break;
                    case 2:
                        for (int i = 0; i < wells.Indexes[0]; i++)
                            eqPressures.Add(wells.Wells[0].CalculatedP - wells.Wells[0].P0);
                        for (int i = wells.Indexes[0]; i < wells.Indexes[1] + 1; i++)
                            eqPressures.Add(wells.Wells[1].CalculatedP - wells.Wells[0].P0);
                        break;
                    case 3:
                        var pr = new double[wells.Indexes[2]];
                        for (int i = 0; i < wells.Indexes[0]; i++)
                        {
                            pr[i] = wells.Wells[0].CalculatedP - wells.Wells[0].P0;
                            pr[i + wells.Indexes[0]] = wells.Wells[1].CalculatedP - wells.Wells[0].P0;
                            pr[i + wells.Indexes[1]- 1] = wells.Wells[2].CalculatedP - wells.Wells[0].P0;
                        }
                        eqPressures = pr.ToList();
                        return;
                }

                eqPressures.RemoveAt(eqPressures.Count - 1);
            }
            else
                switch (wells.Wells.Count)
                {
                    case 1:
                        for (int i = 0; i < wells.Indexes[0]; i++)
                            eqPressures.Add(wells.Wells[0].P - wells.Wells[0].P0);
                        break;
                    case 2:
                        for (int i = 0; i < wells.Indexes[0]; i++)
                            eqPressures.Add(wells.Wells[0].P - wells.Wells[0].P0);
                        for (int i = wells.Indexes[0]; i < wells.Indexes[1] + 1; i++)
                            eqPressures.Add(wells.Wells[1].P - wells.Wells[0].P0);
                        break;
                    case 3:
                        for (int i = 0; i < wells.Indexes[0]; i++)
                            eqPressures.Add(wells.Wells[0].P - wells.Wells[0].P0);
                        for (int i = wells.Indexes[0]; i < wells.Indexes[1] + 1; i++)
                            eqPressures.Add(wells.Wells[1].P - wells.Wells[0].P0);
                        for (int i = wells.Indexes[1]; i < wells.Indexes[2] + 1; i++)
                            eqPressures.Add(wells.Wells[2].P - wells.Wells[0].P0);
                        break;
                }
            eqPressures.RemoveAt(0);
        }

        public static void PrepareStaticConsumptions(WellsList wells, List<double> staticConsumptions)
        {
            switch (wells.Indexes.Count)
            {
                case 1:
                    for (int i = 0; i != wells.Indexes[0]; i++)
                    {
                        staticConsumptions.Add(wells.Wells[0].Q);
                    }
                    break;
                case 2:
                    for (int i = 0; i != wells.Indexes[0]; i++)
                    {
                        staticConsumptions.Add(wells.Wells[0].Q);
                    }
                    for (int i = wells.Indexes[0]; i < wells.Indexes[1] + 1; i++)
                    {
                        staticConsumptions.Add(wells.Wells[1].Q);
                    }
                    break;
                case 3:
                    for (int i = 0; i != wells.Wells[0].N; i++)
                    {
                        staticConsumptions.Add(wells.Wells[0].Q);
                    }
                    for (int i = 0; i < wells.Wells[1].N; i++)
                    {
                        staticConsumptions.Add(wells.Wells[1].Q);
                    }
                    for (int i = 0; i < wells.Wells[2].N; i++)
                    {
                        staticConsumptions.Add(wells.Wells[2].Q);
                    }
                    break;
            }
        }

        public static void PrepareCoefs(List<double> times, List<Well> wells, out List<List<double>> coefs)
        {
            coefs = new List<List<double>>(times.Count - 1);
            for (int i = 0; i < times.Count - 1; i++)
                coefs.Add(new List<double>());
            int n = 1;
            for (int i = 0; i != times.Count - 1; i++)
            {
                int k = 1;
                for (int j = 0; j != times.Count - 1; j++)
                {
                    if (k <= n)
                    {
                        double E1, E2, arg1, arg2;
                        arg1 = Math.Pow(wells[0].Rs, 2) * 1.0 / (4 * wells[0].Kappa * (times[n] - times[k - 1]));
                        if (k == n)
                        {
                            arg2 = 0.0;
                        }
                        else
                        {
                            arg2 = Math.Pow(wells[0].Rs, 2) * 1.0 / (4 * wells[0].Kappa * (times[n] - times[k]));
                        }
                        if (arg1 < 1)
                        {
                            E1 = IntegralCalculator.PolyApproxExpIntegral2(arg1);
                        }
                        else
                        {
                            E1 = IntegralCalculator.PolyApproxExpIntegral1(arg1);
                        }
                        if (k == n)
                        {
                            E1 = E1 + wells[0].Ksi;
                        }
                        if ((arg2 < 1) && (arg2 > 0.0))
                        {
                            E2 = IntegralCalculator.PolyApproxExpIntegral2(arg2);
                        }
                        else
                        {
                            E2 = 0;
                        }
                        var coefBefore = wells[0].Mu / (4.0 * Math.PI * wells[0].K * wells[0].H0); // 39788735772.973831
                        coefs[i].Add(coefBefore * (E1 - E2));
                    }
                    else
                    {
                        coefs[i].Add(0);
                    }
                    k++;
                }
                n++;
            }
        }
        #endregion

        #region SLAE solve method

        public static void Gauss(List<List<double>> a, List<double> y, List<double> x)
        {
            double max, temp;
            int k, index;
            const double eps = 0.00001;  // точность
            k = 0;
            while (k < a.Count)
            {
                // Поиск строки с максимальным a[i][k]
                max = Math.Abs(a[k][k]);
                index = k;
                for (int i = k + 1; i < a.Count; i++)
                {
                    if (Math.Abs(a[i][k]) > max)
                    {
                        max = Math.Abs(a[i][k]);
                        index = i;
                    }
                }
                // Перестановка строк
                if (max < eps)
                {
                    throw new Exception("cool story bob");
                }
                for (int j = 0; j < a.Count; j++)
                {
                    temp = a[k][j];
                    a[k][j] = a[index][j];
                    a[index][j] = temp;
                }
                temp = y[k];
                y[k] = y[index];
                y[index] = temp;
                // Нормализация уравнений
                for (int i = k; i < a.Count; i++)
                {
                    temp = a[i][k];
                    if (Math.Abs(temp) < eps) continue; // для нулевого коэффициента пропустить
                    for (int j = 0; j < a.Count; j++)
                        a[i][j] = a[i][j] / temp;
                    y[i] = y[i] / temp;
                    if (i == k) continue; // уравнение не вычитать само из себя
                    for (int j = 0; j < a.Count; j++)
                        a[i][j] = a[i][j] - a[k][j];
                    y[i] = y[i] - y[k];
                }
                k++;
            }
            // обратная подстановка
            for (k = a.Count - 1; k >= 0; k--)
            {
                x[k] = y[k];
                for (int i = 0; i < k; i++)
                    y[i] = y[i] - a[i][k] * x[k];
            }
        }

        public static void GaussSeidel(List<List<double>> A, List<double> B, List<double> X)
        {
            //for (int i = 0; i < X.Count; i++)
            //{
            //    var sum = 0.0;
            //    for (int j = 0; j < i; j++)
            //    {
            //        sum += A[i][j] * X[j];
            //    }
            //    X[i] = (B[i] - sum) / A[i][i];
            //}

            List<double> prev = new List<double>();
            do
            {
                prev = X.ToList();
                for (int i = 0; i < X.Count; i++)
                {
                    double var = 0;
                    for (int j = 0; j < i; j++)
                        var += (A[i][j] * X[j]);
                    for (int j = i + 1; j < X.Count; j++)
                        var += (A[i][j] * prev[j]);
                    X[i] = (B[i] - var) / A[i][i];
                }
            }
            while (!Converge(X, prev));
        }

        private static bool Converge(List<double> xk, List<double> xkp)
        {
            double eps = 0.001;
            double norm = 0;
            for (int i = 0; i < xk.Count; i++)
                norm += (xk[i] - xkp[i]) * (xk[i] - xkp[i]);
            return (Math.Sqrt(norm) < eps);
        }
        #endregion

        public static List<double> GetConsumtions(WellsList wells)
        {
            List<double> times = GetTimes(wells.Wells, false);
            List<double> consumptions = Enumerable.Repeat<double>(0.0, times.Count - 1).ToList();
            List<List<double>> coefs;
            List<double> eqPressures;
            PrepareEqPressures(wells, out eqPressures);
            PrepareCoefs(times, wells.Wells, out coefs);


            GaussSeidel(coefs, eqPressures, consumptions);
            return consumptions;
        }

        public static ConsumptionsAndTimes GetConsumtionsAndTimes(WellsList wells)
        {
            List<double> times = GetTimes(wells.Wells, false);
            List<double> consumptions = Enumerable.Repeat<double>(0.0, times.Count - 1).ToList();
            List<List<double>> coefs;
            List<double> eqPressures;
            PrepareEqPressures(wells, out eqPressures);
            PrepareCoefs(times, wells.Wells, out coefs);


            GaussSeidel(coefs, eqPressures, consumptions);
            return new ConsumptionsAndTimes {Consumptions = consumptions, Times = times };
        }


        #region Get Next Gradient Iteration
        public static void GetNextGradientIteration(GradientAndWellsList<QGradient> gradientAndWellsList, List<Well> gradientWells,
            out QGradientAndConsumptions gradientAndConsumptions)
        {
            gradientAndConsumptions = new QGradientAndConsumptions();

            #region Wells fill
            List<Well> kWells = new List<Well>();
            List<Well> kappaWells = new List<Well>();
            List<Well> ksiWells = new List<Well>();
            List<Well> p0wells = new List<Well>();
            foreach (var v in gradientWells)
            {
                kWells.Add(new Well
                {

                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,

                });
                kappaWells.Add(new Well
                {

                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,

                });
                ksiWells.Add(new Well
                {

                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,

                });
                p0wells.Add(new Well
                {

                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,

                });
            }
            for (int i = 0; i < gradientWells.Count; i++)
            {
                kWells[i].K = gradientAndWellsList.Gradient.ChangedK + gradientAndWellsList.Gradient.DeltaK;
                kappaWells[i].Kappa = gradientAndWellsList.Gradient.ChangedKappa + gradientAndWellsList.Gradient.DeltaKappa;
                ksiWells[i].Ksi = gradientAndWellsList.Gradient.ChangedKsi + gradientAndWellsList.Gradient.DeltaKsi;
                p0wells[i].P0 = gradientAndWellsList.Gradient.ChangedP0 + gradientAndWellsList.Gradient.DeltaP0;
            }
            #endregion

            #region Qk QkDelta QkappaDelta QksiDelta QP0Delta evaluation

            WellsList wlGradWells = new WellsList { Wells = gradientWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            List<double> Qk = GetConsumtions(wlGradWells); // Q_k

            WellsList wlKWells = new WellsList { Wells = kWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            List<double> QkDelta = GetConsumtions(wlKWells); // k+delta

            WellsList wlKappaWells = new WellsList { Wells = kappaWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            List<double> QkappaDelta = GetConsumtions(wlKappaWells); // kappa+delta

            WellsList wlKsiWells = new WellsList { Wells = ksiWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            List<double> QksiDelta = GetConsumtions(wlKsiWells); // ksi+delta

            WellsList wlP0Wells = new WellsList { Wells = p0wells, Indexes = gradientAndWellsList.WellsList.Indexes };
            List<double> QP0Delta = GetConsumtions(wlP0Wells); // p0+delta
                                                               // подсчет градиента
            #endregion

            #region gradient projections evaluation
            double gradientK, gradientKappa, gradientKsi, gradientP0;
            GradientProjectionsEvaluation(gradientAndWellsList, gradientWells, Qk, wlKWells, QkDelta, QkappaDelta, QksiDelta, QP0Delta, out gradientK, out gradientKappa, out gradientKsi, out gradientP0);
            #endregion

            QGradient nextGradient = new QGradient
            {
                Lambda = gradientAndWellsList.Gradient.Lambda,
                GradientK = gradientK,
                GradientKappa = gradientKappa,
                GradientKsi = gradientKsi,
                GradientP0 = gradientP0,
                UsedK = gradientAndWellsList.Gradient.UsedK,
                UsedKappa = gradientAndWellsList.Gradient.UsedKappa,
                UsedKsi = gradientAndWellsList.Gradient.UsedKsi,
                UsedP0 = gradientAndWellsList.Gradient.UsedP0,
            };
            (int i1, int i2, int i3, int i4) = IncludeProjections(gradientAndWellsList);
            (double kNext, double kappaNext, double ksiNext, double p0Next) = GetNextValues(gradientAndWellsList, gradientK, gradientKappa, gradientKsi, gradientP0, nextGradient, i1, i2, i3, i4);
            if ((kNext > 0) && (kappaNext > 0) && (ksiNext >= -1) && (p0Next >= 0))
            {
                GetChangedValuesForGradient(nextGradient, kNext, kappaNext, ksiNext, p0Next);
                List<Well> Qk1wells = new List<Well>();
                Qk1wells.AddRange(gradientWells);
                for (int i = 0; i < Qk1wells.Count; i++)
                {
                    Qk1wells[i].K = nextGradient.ChangedK;
                    Qk1wells[i].Kappa = nextGradient.ChangedKappa;
                    Qk1wells[i].Ksi = nextGradient.ChangedKsi;
                    Qk1wells[i].P0 = nextGradient.ChangedP0;
                };
                WellsList wlQk1Wells = new WellsList { Wells = Qk1wells, Indexes = gradientAndWellsList.WellsList.Indexes };
                List<double> Qk1 = GetConsumtions(wlQk1Wells);
                double Fmin = 0;
                switch (gradientWells.Count)
                {
                    case 1:
                        Fmin = Math.Pow((gradientWells[0].Q - Qk1.Last()), 2);
                        Fmin = Math.Sqrt(Fmin / (Math.Pow(gradientWells[0].Q, 2)));
                        break;
                    case 2:
                        Fmin = Math.Pow((gradientWells[0].Q - Qk1[wlQk1Wells.Indexes[0] - 2]), 2) + Math.Pow((gradientWells[1].Q - Qk1.Last()), 2);
                        Fmin = Math.Sqrt(Fmin / (Math.Pow(gradientWells[0].Q, 2) + Math.Pow(gradientWells[1].Q, 2)));
                        break;
                    case 3:
                        var q1 = Qk1[wlQk1Wells.Indexes[0] - 2];
                        var q2 = Qk1[wlQk1Wells.Indexes[1] - 1];
                        var q3 = Qk1.Last();
                        Fmin = Math.Pow((gradientWells[0].Q - q1), 2)
                                + Math.Pow((gradientWells[1].Q - q2), 2)
                                + Math.Pow((gradientWells[2].Q - q3), 2);
                        Fmin = Fmin / (Math.Pow(gradientWells[0].Q, 2) + Math.Pow(gradientWells[1].Q, 2) + Math.Pow(gradientWells[2].Q, 2));
                        break;

                }

                nextGradient.FminQ = Fmin;
                ConsumptionsAndTimes consumptionsAndTimes = new ConsumptionsAndTimes { Times = GetTimes(gradientWells, false), Consumptions = Qk1 };
                gradientAndConsumptions.ValuesAndTimes = consumptionsAndTimes;
                gradientAndConsumptions.Grad = nextGradient;
            }
        }

        private static (int i1, int i2, int i3, int i4) IncludeProjections<T>(GradientAndWellsList<T>  gradientAndWellsList) where T : Gradient
        {
            var i1 = (gradientAndWellsList.Gradient.UsedK ?? false) ? 1 : 0;
            var i2  =  (gradientAndWellsList.Gradient.UsedKappa ?? false) ? 1 : 0;
            var i3  =  (gradientAndWellsList.Gradient.UsedKsi ?? false) ? 1 : 0;
            var i4 = (gradientAndWellsList.Gradient.UsedP0 ?? false) ? 1 : 0;

            return (i1, i2, i3, i4);
        }

        private static void GradientProjectionsEvaluation(GradientAndWellsList<QGradient> gradientAndWellsList, List<Well> gradientWells, List<double> Qk, WellsList wlKWells, List<double> QkDelta, List<double> QkappaDelta, List<double> QksiDelta, List<double> QP0Delta, out double gradientK, out double gradientKappa, out double gradientKsi, out double gradientP0)
        {
            gradientK = 0;
            gradientKappa = 0;
            gradientKsi = 0;
            gradientP0 = 0;
            switch (gradientWells.Count)
            {
                #region case 1
                case 1:
                    gradientK = (Math.Pow((gradientWells[0].Q - QkDelta.Last()), 2) -
                        Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradientAndWellsList.Gradient.DeltaK;
                    gradientKappa = (Math.Pow((gradientWells[0].Q - QkappaDelta.Last()), 2) -
                        Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradientAndWellsList.Gradient.DeltaKappa;
                    if (gradientAndWellsList.Gradient.DeltaKsi == 0)
                    {
                        gradientKsi = 0;
                    }
                    else
                    {
                        gradientKsi = (Math.Pow((gradientWells[0].Q - QksiDelta.Last()), 2) -
                            Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradientAndWellsList.Gradient.DeltaKsi;
                    }
                    if (gradientAndWellsList.Gradient.DeltaP0 == 0)
                    {
                        gradientP0 = 0;
                    }
                    else
                    {
                        gradientP0 = (Math.Pow((gradientWells[0].Q - QP0Delta.Last()), 2) -
                            Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradientAndWellsList.Gradient.DeltaP0;
                    }
                    break;
                #endregion
                #region case 2
                case 2:
                    gradientK = ((Math.Pow((gradientWells[0].Q - QkDelta[wlKWells.Indexes[0] - 2]), 2) +
                            Math.Pow((gradientWells[1].Q - QkDelta.Last()), 2))
                            - (Math.Pow((gradientWells[0].Q - Qk[wlKWells.Indexes[0] - 2]), 2)
                            + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradientAndWellsList.Gradient.DeltaK;
                    gradientKappa = ((Math.Pow((gradientWells[0].Q - QkappaDelta[wlKWells.Indexes[0] - 2]), 2)
                            + Math.Pow((gradientWells[1].Q - QkappaDelta.Last()), 2))
                            - (Math.Pow((gradientWells[0].Q - Qk[wlKWells.Indexes[0] - 2]), 2)
                            + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradientAndWellsList.Gradient.DeltaKappa;
                    if (gradientAndWellsList.Gradient.DeltaKsi == 0)
                    {
                        gradientKsi = 0;
                    }
                    else
                    {
                        gradientKsi = ((Math.Pow((gradientWells[0].Q - QksiDelta[wlKWells.Indexes[0] - 2]), 2)
                                + Math.Pow((gradientWells[1].Q - QksiDelta.Last()), 2))
                                - (Math.Pow((gradientWells[0].Q - Qk[wlKWells.Indexes[0] - 2]), 2)
                                + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradientAndWellsList.Gradient.DeltaKsi;
                    }
                    if (gradientAndWellsList.Gradient.DeltaP0 == 0)
                    {
                        gradientP0 = 0;
                    }
                    else
                    {
                        gradientP0 = ((Math.Pow((gradientWells[0].Q - QP0Delta[wlKWells.Indexes[0] - 2]), 2)
                                + Math.Pow((gradientWells[1].Q - QP0Delta.Last()), 2))
                                - (Math.Pow((gradientWells[0].Q - Qk[wlKWells.Indexes[0] - 2]), 2)
                                + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradientAndWellsList.Gradient.DeltaP0;
                    }
                    break;
                #endregion
                #region case 3
                case 3:
                    var f = (Math.Pow(gradientWells[0].Q - Qk[wlKWells.Indexes[0] - 2], 2)
                            + Math.Pow(gradientWells[1].Q - Qk[wlKWells.Indexes[1] - 1], 2)
                            + Math.Pow(gradientWells[2].Q - Qk.Last(), 2))
                            / (Math.Pow(gradientWells[0].Q, 2)
                            + Math.Pow(gradientWells[1].Q, 2)
                            + Math.Pow(gradientWells[2].Q, 2));

                    var fk = (Math.Pow(gradientWells[0].Q - QkDelta[wlKWells.Indexes[0] - 2], 2)
                            + Math.Pow(gradientWells[1].Q - QkDelta[wlKWells.Indexes[1] - 1], 2)
                            + Math.Pow(gradientWells[2].Q - QkDelta.Last(), 2))
                            / (Math.Pow(gradientWells[0].Q, 2)
                            + Math.Pow(gradientWells[1].Q, 2)
                            + Math.Pow(gradientWells[2].Q, 2));

                    gradientK = (fk - f) / (gradientAndWellsList.Gradient.DeltaK * Math.Pow(10.0, 15) * Math.Pow(10, 3));

                    var fkappa = (Math.Pow(gradientWells[0].Q - QkappaDelta[wlKWells.Indexes[0] - 2], 2)
                                  + Math.Pow(gradientWells[1].Q - QkappaDelta[wlKWells.Indexes[1] - 1], 2)
                                  + Math.Pow(gradientWells[2].Q - QkappaDelta.Last(), 2))
                                  / (Math.Pow(gradientWells[0].Q, 2)
                                  + Math.Pow(gradientWells[1].Q, 2)
                                  + Math.Pow(gradientWells[2].Q, 2));
                    gradientKappa = (fkappa - f) / (gradientAndWellsList.Gradient.DeltaKappa * 3600.0 * Math.Pow(10, 3));
                    if (gradientAndWellsList.Gradient.DeltaKsi == 0)
                    {
                        gradientKsi = 0;
                    }
                    else
                    {
                        gradientKsi = ((Math.Pow((gradientWells[0].Q - QksiDelta[wlKWells.Indexes[0] - 2]), 2) +
                            Math.Pow((gradientWells[1].Q - QksiDelta[wlKWells.Indexes[1] - 1]), 2)
                            + Math.Pow((gradientWells[2].Q - QksiDelta.Last()), 2))
                            - (Math.Pow((gradientWells[0].Q - Qk[wlKWells.Indexes[0] - 2]), 2)
                            + Math.Pow((gradientWells[1].Q - Qk[wlKWells.Indexes[1] - 1]), 2)
                            + Math.Pow((gradientWells[2].Q - Qk.Last()), 2))) / gradientAndWellsList.Gradient.DeltaKsi;
                    }
                    if (gradientAndWellsList.Gradient.DeltaP0 == 0)
                    {
                        gradientP0 = 0;
                    }
                    else
                    {
                        var fp = (Math.Pow(gradientWells[0].Q - QP0Delta[wlKWells.Indexes[0] - 2], 2)
                            + Math.Pow(gradientWells[1].Q - QP0Delta[wlKWells.Indexes[1] - 1], 2)
                            + Math.Pow(gradientWells[2].Q - QP0Delta.Last(), 2))
                            / (Math.Pow(gradientWells[0].Q, 2)
                                  + Math.Pow(gradientWells[1].Q, 2)
                                  + Math.Pow(gradientWells[2].Q, 2));
                        gradientP0 = (fp - f) / (gradientAndWellsList.Gradient.DeltaP0 * Math.Pow(10.0, -6) * Math.Pow(10, 3));
                    }
                    break;
                    #endregion
            }
        }

        public static void GetNextPGradientIteration(GradientAndWellsList<PGradient> gradientAndWellsList, List<Well> gradientWells,
            out PGradientAndPressures gradientAndPressures)
        {
            gradientAndPressures = new PGradientAndPressures();

            #region Wells fill
            List<Well> kWells = new List<Well>();
            List<Well> kappaWells = new List<Well>();
            List<Well> ksiWells = new List<Well>();
            List<Well> p0wells = new List<Well>();
            foreach (var v in gradientWells)
            {
                kWells.Add(new Well
                {

                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,

                });
                kappaWells.Add(new Well
                {

                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,

                });
                ksiWells.Add(new Well
                {

                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,

                });
                p0wells.Add(new Well
                {

                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,

                });
            }
            for (int i = 0; i < gradientWells.Count; i++)
            {
                kWells[i].K = gradientAndWellsList.Gradient.ChangedK + gradientAndWellsList.Gradient.DeltaK;
                kappaWells[i].Kappa = gradientAndWellsList.Gradient.ChangedKappa + gradientAndWellsList.Gradient.DeltaKappa;
                ksiWells[i].Ksi = gradientAndWellsList.Gradient.ChangedKsi + gradientAndWellsList.Gradient.DeltaKsi;
                p0wells[i].P0 = gradientAndWellsList.Gradient.ChangedP0 + gradientAndWellsList.Gradient.DeltaP0;
            }
            #endregion

            #region Pk PkDelta PkappaDelta PksiDelta PP0Delta evaluation

            WellsList wlGradWells = new WellsList { Wells = gradientWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            PressuresAndTimes Pk = GetTimesAndPressures(wlGradWells); // Q_k

            WellsList wlKWells = new WellsList { Wells = kWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            PressuresAndTimes PkDelta = GetTimesAndPressures(wlKWells); // k+delta

            WellsList wlKappaWells = new WellsList { Wells = kappaWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            PressuresAndTimes PkappaDelta = GetTimesAndPressures(wlKappaWells); // kappa+delta

            WellsList wlKsiWells = new WellsList { Wells = ksiWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            PressuresAndTimes PksiDelta = GetTimesAndPressures(wlKsiWells); // ksi+delta

            WellsList wlP0Wells = new WellsList { Wells = p0wells, Indexes = gradientAndWellsList.WellsList.Indexes };
            PressuresAndTimes PP0Delta = GetTimesAndPressures(wlP0Wells); // p0+delta
                                                                          // подсчет градиента
            #endregion

            #region gradient projections evaluation
            double gradientK, gradientKappa, gradientKsi, gradientP0;
            GradientProjectionsEvaluation(gradientAndWellsList, gradientWells, Pk, PkDelta, PkappaDelta, PksiDelta, PP0Delta, out gradientK, out gradientKappa, out gradientKsi, out gradientP0);
            #endregion

            PGradient nextGradient = new PGradient
            {
                Lambda = gradientAndWellsList.Gradient.Lambda,
                GradientK = gradientK,
                GradientKappa = gradientKappa,
                GradientKsi = gradientKsi,
                GradientP0 = gradientP0,
                UsedK = gradientAndWellsList.Gradient.UsedK,
                UsedKappa = gradientAndWellsList.Gradient.UsedKappa,
                UsedKsi = gradientAndWellsList.Gradient.UsedKsi,
                UsedP0 = gradientAndWellsList.Gradient.UsedP0,
            };


            (int i1, int i2, int i3, int i4) = IncludeProjections(gradientAndWellsList);
            (double kNext, double kappaNext, double ksiNext, double p0Next) = GetNextValues(gradientAndWellsList, gradientK, gradientKappa, gradientKsi, gradientP0, nextGradient, i1, i2, i3, i4);
            if ((kNext > 0) && (kappaNext > 0) && (ksiNext >= -1) && (p0Next >= 0))
            {
                GetChangedValuesForGradient(nextGradient, kNext, kappaNext, ksiNext, p0Next);
                List<Well> Pk1wells = new List<Well>();
                Pk1wells.AddRange(gradientWells);
                for (int i = 0; i < Pk1wells.Count; i++)
                {
                    Pk1wells[i].K = nextGradient.ChangedK;
                    Pk1wells[i].Kappa = nextGradient.ChangedKappa;
                    Pk1wells[i].Ksi = nextGradient.ChangedKsi;
                    Pk1wells[i].P0 = nextGradient.ChangedP0;
                }
                WellsList wlPk1Wells = new WellsList { Wells = Pk1wells, Indexes = gradientAndWellsList.WellsList.Indexes };
                PressuresAndTimes Pk1 = GetTimesAndPressures(wlPk1Wells);
                double Fmin = 0;
                switch (gradientWells.Count)
                {
                    //case 1:
                    //    Fmin = Math.Pow((gradientWells[0].Q - Pk1.Pressures1f.Last()), 2);
                    //    Fmin = Math.Sqrt(Fmin / (Math.Pow(gradientWells[0].P, 2)));
                    //    break;
                    //case 2:
                    //    Fmin = Math.Pow((gradientWells[0].P - Pk1[wlPk1Wells.Indexes[0] - 2]), 2) + Math.Pow((gradientWells[1].P - Pk1.Last()), 2);
                    //    Fmin = Math.Sqrt(Fmin / (Math.Pow(gradientWells[0].P, 2) + Math.Pow(gradientWells[1].P, 2)));
                    //    break;
                    case 3:
                        Fmin = Math.Pow((gradientWells[0].P - Pk1.Pressures1f.Last()), 2)
                                + Math.Pow((gradientWells[1].P - Pk1.Pressures2f.Last()), 2)
                                + Math.Pow((gradientWells[2].P - Pk1.Pressures3.Last()), 2);
                        Fmin = (Fmin / (Math.Pow(gradientWells[0].P, 2) + Math.Pow(gradientWells[1].P, 2) + Math.Pow(gradientWells[2].P, 2)));
                      
                        break;

                }

                nextGradient.FminP = Fmin;
                PressuresAndTimes pressuresAndTimes = new PressuresAndTimes
                {
                    Times1 = Pk1.Times1,
                    Times1f = Pk1.Times1f,
                    Times1s = Pk1.Times1s,
                    Times2 = Pk1.Times2,
                    Times2f = Pk1.Times2f,
                    Times2s = Pk1.Times2s,
                    Times3 = Pk1.Times3,
                    Pressures1 = Pk1.Pressures1,
                    Pressures1f = Pk1.Pressures1f,
                    Pressures1s = Pk1.Pressures1s,
                    Pressures2 = Pk1.Pressures2,
                    Pressures2f = Pk1.Pressures2f,
                    Pressures2s = Pk1.Pressures2s,
                    Pressures3 = Pk1.Pressures3,
                };
                gradientAndPressures.ValuesAndTimes = pressuresAndTimes;
                gradientAndPressures.Grad = nextGradient;
            }
        }

        private static void GetChangedValuesForGradient(Gradient nextGradient, double kNext, double kappaNext, double ksiNext, double p0Next)
        {
            nextGradient.ChangedK = Converter.Convert(kNext, ValueType.K);
            nextGradient.ChangedKappa = Converter.Convert(kappaNext, ValueType.Kappa);
            nextGradient.ChangedKsi = Converter.Convert(ksiNext, ValueType.Ksi);
            nextGradient.ChangedP0 = Converter.Convert(p0Next, ValueType.P);
        }

        private static (double kNext, double kappaNext, double ksiNext, double p0Next) GetNextValues<T>(GradientAndWellsList<T> gradientAndWellsList, double gradientK, double gradientKappa, double gradientKsi, double gradientP0, T nextGradient,
            int i1, int i2, int i3, int i4) where T : Gradient
        {
            var kNext = Converter.ConvertBack(gradientAndWellsList.Gradient.ChangedK, ValueType.K) - i1 * nextGradient.Lambda * gradientK;
            var kappaNext = Converter.ConvertBack(gradientAndWellsList.Gradient.ChangedKappa, ValueType.Kappa) - i2 * nextGradient.Lambda * gradientKappa;
            var ksiNext = Converter.ConvertBack(gradientAndWellsList.Gradient.ChangedKsi, ValueType.Ksi) - i3 * nextGradient.Lambda * gradientKsi;
            var p0Next = Converter.ConvertBack(gradientAndWellsList.Gradient.ChangedP0, ValueType.P) - i4 * nextGradient.Lambda * gradientP0;
            return (kNext, kappaNext, ksiNext, p0Next);
        }

        private static void GradientProjectionsEvaluation(GradientAndWellsList<PGradient> gradientAndWellsList, List<Well> gradientWells, PressuresAndTimes Pk, PressuresAndTimes PkDelta, PressuresAndTimes PkappaDelta, PressuresAndTimes PksiDelta, PressuresAndTimes PP0Delta, 
            out double gradientK, out double gradientKappa, out double gradientKsi, out double gradientP0)
        {
            gradientK = 0;
            gradientKappa = 0;
            gradientKsi = 0;
            gradientP0 = 0;
            switch (gradientWells.Count)
            {
                #region case 1
                //case 1:
                //    gradientK = (Math.Pow((gradientWells[0].P - PkDelta.Last()), 2) -
                //        Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradientAndWellsList.Gradient.DeltaK;
                //    gradientKappa = (Math.Pow((gradientWells[0].Q - QkappaDelta.Last()), 2) -
                //        Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradientAndWellsList.Gradient.DeltaKappa;
                //    if (gradientAndWellsList.Gradient.DeltaKsi == 0)
                //    {
                //        gradientKsi = 0;
                //    }
                //    else
                //    {
                //        gradientKsi = (Math.Pow((gradientWells[0].Q - QksiDelta.Last()), 2) -
                //            Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradientAndWellsList.Gradient.DeltaKsi;
                //    }
                //    if (gradientAndWellsList.Gradient.DeltaP0 == 0)
                //    {
                //        gradientP0 = 0;
                //    }
                //    else
                //    {
                //        gradientP0 = (Math.Pow((gradientWells[0].Q - QP0Delta.Last()), 2) -
                //            Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradientAndWellsList.Gradient.DeltaP0;
                //    }
                //    break;
                //#endregion
                //#region case 2
                //case 2:
                //    gradientK = ((Math.Pow((gradientWells[0].Q - QkDelta[wlKWells.Indexes[0] - 2]), 2) +
                //            Math.Pow((gradientWells[1].Q - QkDelta.Last()), 2))
                //            - (Math.Pow((gradientWells[0].Q - Qk[wlKWells.Indexes[0] - 2]), 2)
                //            + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradientAndWellsList.Gradient.DeltaK;
                //    gradientKappa = ((Math.Pow((gradientWells[0].Q - QkappaDelta[wlKWells.Indexes[0] - 2]), 2)
                //            + Math.Pow((gradientWells[1].Q - QkappaDelta.Last()), 2))
                //            - (Math.Pow((gradientWells[0].Q - Qk[wlKWells.Indexes[0] - 2]), 2)
                //            + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradientAndWellsList.Gradient.DeltaKappa;
                //    if (gradientAndWellsList.Gradient.DeltaKsi == 0)
                //    {
                //        gradientKsi = 0;
                //    }
                //    else
                //    {
                //        gradientKsi = ((Math.Pow((gradientWells[0].Q - QksiDelta[wlKWells.Indexes[0] - 2]), 2)
                //                + Math.Pow((gradientWells[1].Q - QksiDelta.Last()), 2))
                //                - (Math.Pow((gradientWells[0].Q - Qk[wlKWells.Indexes[0] - 2]), 2)
                //                + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradientAndWellsList.Gradient.DeltaKsi;
                //    }
                //    if (gradientAndWellsList.Gradient.DeltaP0 == 0)
                //    {
                //        gradientP0 = 0;
                //    }
                //    else
                //    {
                //        gradientP0 = ((Math.Pow((gradientWells[0].Q - QP0Delta[wlKWells.Indexes[0] - 2]), 2)
                //                + Math.Pow((gradientWells[1].Q - QP0Delta.Last()), 2))
                //                - (Math.Pow((gradientWells[0].Q - Qk[wlKWells.Indexes[0] - 2]), 2)
                //                + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradientAndWellsList.Gradient.DeltaP0;
                //    }
                //    break;
                #endregion
                #region case 3
                case 3:
                    var f = (Math.Pow(gradientWells[0].P - Pk.Pressures1f.Last(), 2)
                            + Math.Pow(gradientWells[1].P - Pk.Pressures2f.Last(), 2)
                            + Math.Pow(gradientWells[2].P - Pk.Pressures3.Last(), 2))
                            / (Math.Pow(gradientWells[0].P, 2)
                            + Math.Pow(gradientWells[1].P, 2)
                            + Math.Pow(gradientWells[2].P, 2));

                    var fk = (Math.Pow(gradientWells[0].P - PkDelta.Pressures1f.Last(), 2)
                            + Math.Pow(gradientWells[1].P - PkDelta.Pressures2f.Last(), 2)
                            + Math.Pow(gradientWells[2].P - PkDelta.Pressures3.Last(), 2))
                            / (Math.Pow(gradientWells[0].P, 2)
                            + Math.Pow(gradientWells[1].P, 2)
                            + Math.Pow(gradientWells[2].P, 2));

                    gradientK = (fk - f) / (gradientAndWellsList.Gradient.DeltaK * Math.Pow(10.0, 15) * Math.Pow(10, 3));

                    var fkappa = (Math.Pow(gradientWells[0].P - PkappaDelta.Pressures1f.Last(), 2)
                                  + Math.Pow(gradientWells[1].P - PkappaDelta.Pressures2f.Last(), 2)
                                  + Math.Pow(gradientWells[2].P - PkappaDelta.Pressures3.Last(), 2))
                                  / (Math.Pow(gradientWells[0].P, 2)
                                  + Math.Pow(gradientWells[1].P, 2)
                                  + Math.Pow(gradientWells[2].P, 2));
                    gradientKappa = (fkappa - f) / (gradientAndWellsList.Gradient.DeltaKappa * 3600.0 * Math.Pow(10, 3));

                    var fksi = (Math.Pow(gradientWells[0].P - PksiDelta.Pressures1f.Last(), 2)
                                 + Math.Pow(gradientWells[1].P - PksiDelta.Pressures2f.Last(), 2)
                                 + Math.Pow(gradientWells[2].P - PksiDelta.Pressures3.Last(), 2))
                                 / (Math.Pow(gradientWells[0].P, 2)
                                 + Math.Pow(gradientWells[1].P, 2)
                                 + Math.Pow(gradientWells[2].P, 2));

                    if (gradientAndWellsList.Gradient.DeltaKsi == 0)
                    {
                        gradientKsi = 0;
                    }
                    else
                    {
                        gradientKsi = (fksi - f) / (gradientAndWellsList.Gradient.DeltaKsi);
                    }
                    if (gradientAndWellsList.Gradient.DeltaP0 == 0)
                    {
                        gradientP0 = 0;
                    }
                    else
                    {
                        var fp = (Math.Pow(gradientWells[0].P - PP0Delta.Pressures1f.Last(), 2)
                               + Math.Pow(gradientWells[1].P - PP0Delta.Pressures2f.Last(), 2)
                               + Math.Pow(gradientWells[2].P - PP0Delta.Pressures3.Last(), 2))
                               / (Math.Pow(gradientWells[0].P, 2)
                                     + Math.Pow(gradientWells[1].P, 2)
                                     + Math.Pow(gradientWells[2].P, 2));
                        gradientP0 = (fp - f) / (gradientAndWellsList.Gradient.DeltaP0 * Math.Pow(10.0, -6) * Math.Pow(10, 3));
                    }
                    break;
                    #endregion
            }
        }

        #endregion



        public static ConsumptionsAndTimes GetConsumptions(WellsList wellsList, bool prepareStatic = true)
        {
            ConsumptionsAndTimes consumptionsAndTimes = new ConsumptionsAndTimes();
            var consumptions = Functions.GetConsumtions(wellsList);
            List<double> staticConsumptions = new List<double>();
            if (prepareStatic)
            {
                Functions.PrepareStaticConsumptions(wellsList, staticConsumptions);
            }
            consumptionsAndTimes.Times = Functions.GetTimes(wellsList.Wells, false);
            consumptionsAndTimes.Consumptions = consumptions;
            if (wellsList.Wells[0].Mode == Mode.Direct)
                consumptionsAndTimes.StaticConsumptions = staticConsumptions;
            wellsList.Wells[0].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[0] - 2];
            wellsList.Wells[1].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[1] - 1];
            wellsList.Wells[2].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[2] - 2];
            return consumptionsAndTimes;
        }

        public static QGradientAndConsumptions QGradientMethod(GradientAndWellsList<QGradient> gradientAndWellsList)
        {
            List<Well> gradientWells = new List<Well>();
            foreach (var v in gradientAndWellsList.WellsList.Wells)
                gradientWells.Add(new Well
                {
                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,
                });
            for (int i = 0; i < gradientWells.Count; i++)
            {
                gradientWells[i].K = gradientAndWellsList.Gradient.ChangedK;
                gradientWells[i].Kappa = gradientAndWellsList.Gradient.ChangedKappa;
                gradientWells[i].Ksi = gradientAndWellsList.Gradient.ChangedKsi;
                gradientWells[i].P0 = gradientAndWellsList.Gradient.ChangedP0;
            }
            QGradientAndConsumptions gradientAndConsumptions = new QGradientAndConsumptions() { Grad = gradientAndWellsList.Gradient };
            Functions.GetNextGradientIteration(gradientAndWellsList, gradientWells, out gradientAndConsumptions);
            if (gradientAndConsumptions.ValuesAndTimes != null)
            {
                List<double> staticConsumptions = new List<double>();
                Functions.PrepareStaticConsumptions(gradientAndWellsList.WellsList, staticConsumptions);
                gradientAndConsumptions.ValuesAndTimes.StaticConsumptions = staticConsumptions;
            }
            return gradientAndConsumptions;
        }

        public static PGradientAndPressures PGradientMethod(GradientAndWellsList<PGradient> gradientAndWellsList)
        {
            List<Well> gradientWells = new List<Well>();
            foreach (var v in gradientAndWellsList.WellsList.Wells)
                gradientWells.Add(new Well
                {
                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,
                });
            for (int i = 0; i < gradientWells.Count; i++)
            {
                gradientWells[i].K = gradientAndWellsList.Gradient.ChangedK;
                gradientWells[i].Kappa = gradientAndWellsList.Gradient.ChangedKappa;
                gradientWells[i].Ksi = gradientAndWellsList.Gradient.ChangedKsi;
                gradientWells[i].P0 = gradientAndWellsList.Gradient.ChangedP0;
            }
            PGradientAndPressures pGradientAndPressures = new PGradientAndPressures() { Grad = gradientAndWellsList.Gradient };
            Functions.GetNextPGradientIteration(gradientAndWellsList, gradientWells, out pGradientAndPressures);
            if (pGradientAndPressures.ValuesAndTimes != null)
            {
                List<double> staticConsumptions = new List<double>();
                Functions.PrepareStaticPressures(gradientAndWellsList.WellsList, staticConsumptions);
                pGradientAndPressures.ValuesAndTimes.StaticPressures = staticConsumptions;
            }
            return pGradientAndPressures;
        }
    
        public static double GetObjectFunctionValue(Well[] wells)
        {
            double fMin = 0;
            if (wells[0].Mode == Mode.Direct)
            {
                fMin = Math.Pow(wells[0].Q - wells[0].CalculatedQ, 2)
                                   + Math.Pow(wells[1].Q - wells[1].CalculatedQ, 2)
                                   + Math.Pow(wells[2].Q - wells[2].CalculatedQ, 2);
                fMin = fMin / (Math.Pow(wells[0].Q, 2) + Math.Pow(wells[1].Q, 2) + Math.Pow(wells[2].Q, 2));
                return fMin;
                switch (wells.Count())
                {
                    case 3:
                        fMin = Math.Pow(wells[0].Q - wells[0].CalculatedQ, 2)
                                + Math.Pow(wells[1].Q - wells[1].CalculatedQ, 2)
                                + Math.Pow(wells[2].Q - wells[2].CalculatedQ, 2);
                        fMin = fMin / (Math.Pow(wells[0].Q, 2) + Math.Pow(wells[1].Q, 2) + Math.Pow(wells[2].Q, 2));
                        break;
                }
            }
            else
            {
                double num = Math.Pow(wells[0].P - wells[0].CalculatedP, 2)
                                   + Math.Pow(wells[1].P - wells[1].CalculatedP, 2)
                                   + Math.Pow(wells[2].P - wells[2].CalculatedP, 2);
                double denum = Math.Pow(wells[0].P, 2) + Math.Pow(wells[1].P, 2) + Math.Pow(wells[2].P, 2);
                fMin = num / denum;
                return fMin;
                switch (wells.Count())
                {
                    case 3:
                        num = Math.Pow(wells[0].P - wells[0].CalculatedP, 2)
                                + Math.Pow(wells[1].P - wells[1].CalculatedP, 2)
                                + Math.Pow(wells[2].P - wells[2].CalculatedP, 2);
                        denum = Math.Pow(wells[0].P, 2) + Math.Pow(wells[1].P, 2) + Math.Pow(wells[2].P, 2);
                        fMin = num / denum;
                        break;
                }
            }
            return fMin;            
        }

        public static double GetObjectFunctionValue(Well[] wells, PressuresAndTimes pressuresAndTimes)
        {
            double fMin = 0;
            switch (wells.Count())
            {
                //case 1:
                //    Fmin = Math.Pow((wellViewModel.Wells[0].Q - Qk1.Last()), 2);
                //    Fmin = Math.Sqrt(Fmin / (Math.Pow(wellViewModel.Wells[0].Q, 2)));
                //    break;
                //case 2:
                //    Fmin = Math.Pow((wellViewModel.Wells[0].Q - Qk1[indexes[0] - 2]), 2) + Math.Pow((wellViewModel.Wells[1].Q - Qk1.Last()), 2);
                //    Fmin = Math.Sqrt(Fmin / (Math.Pow(wellViewModel.Wells[0].Q, 2) + Math.Pow(wellViewModel.Wells[1].Q, 2)));
                //    break;
                case 3:
                    fMin = Math.Pow(wells[0].Q - pressuresAndTimes.Pressures1f.Last(), 2)
                            + Math.Pow(wells[1].Q - pressuresAndTimes.Pressures2f.Last(), 2)
                            + Math.Pow(wells[2].Q - pressuresAndTimes.Pressures3.Last(), 2);
                    fMin = fMin / (Math.Pow(wells[0].P, 2) + Math.Pow(wells[1].P, 2) + Math.Pow(wells[2].P, 2));
                    break;
            }
            return fMin;
        }

    }
}
