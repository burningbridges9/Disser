﻿using HydrodynamicStudies.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicStudies.Calculs
{
    public class Functions
    {
        public static double Pressure(Well W, double q, double t)
        {
            double P, arg;
            arg = Math.Pow(W.Rs, 2) / (4.0 * W.Kappa * t);
            if (t == 0.0)
            {
                return 0;
            }
            return P = (q * W.Mu) / (4.0 * Math.PI * W.H0 * W.K) * (W.Ksi + IntegralCalculator.EIntegral(arg));
        }

        public static void GetTimesAndPressures(WellsList wells, out PressuresAndTimes pressuresAndTimes)
        {
            List<double> times = GetTimes(wells.Wells, true);
            List<double> pressures = new List<double>();
            pressuresAndTimes = new PressuresAndTimes();
            #region Unused

            if (wells.Wells.Count == 1)
            {
                List<double> tOne = new List<double>(wells.Wells[0].N);
                List<double> pOne = new List<double>(wells.Wells[0].N);
                double step = (wells.Wells[0].Time2 - wells.Wells[0].Time1) / (wells.Wells[0].N - 1);
                for (int i = 0; i < wells.Wells[0].N; i++)
                {
                    tOne.Add(wells.Wells[0].Time1 + i * step);
                }
                for (int i = 0; i != wells.Wells[0].N; i++)
                {
                    if (tOne[i] == 0.0)
                    {
                        pOne.Add(0 + wells.Wells[0].P0);
                    }
                    else
                    {
                        pOne.Add(Pressure(wells.Wells[0], wells.Wells[0].Q, tOne[i]) + wells.Wells[0].P0);
                    }
                }
                pressures = pOne;
                times = tOne;
                pressuresAndTimes.Pressures1 = pressures;
                pressuresAndTimes.Times1 = times;
                //indexes.Add(times.Count);
                wells.Wells[0].P = pressures.Last();
            }

            if (wells.Wells.Count == 2)
            {
                List<double> tOne = new List<double>();
                List<double> tTwo = new List<double>();
                List<double> pOne = new List<double>();
                List<double> pTwo = new List<double>();
                int counter1 = 0;
                double step1 = (wells.Wells[0].Time2 - wells.Wells[0].Time1) / (wells.Wells[0].N - 1);
                double step2 = (wells.Wells[1].Time2 - wells.Wells[1].Time1) / (wells.Wells[1].N - 1);
                for (int i = 0; i != wells.Wells[0].N; i++)
                {
                    tOne.Add(wells.Wells[0].Time1 + i * step1);
                }
                for (int i = 0; i != wells.Wells[1].N; i++)
                {
                    tTwo.Add(wells.Wells[1].Time1 + i * step2);
                }
                times.AddRange(tOne);
                times.AddRange(tTwo);
                for (int i = 0; i != times.Count; i++)
                {
                    if (times[i] == 0.0)
                    {
                        pOne.Add(0.0 + wells.Wells[0].P0);
                    }
                    else
                    {
                        pOne.Add(Pressure(wells.Wells[0], wells.Wells[0].Q, times[i]) + wells.Wells[0].P0);

                        if ((times[i] >= wells.Wells[1].Time1) && (i >= wells.Wells[0].N))
                        {
                            counter1++;
                            {
                                pTwo.Add(Pressure(wells.Wells[0], wells.Wells[0].Q, times[i])
                                       + Pressure(wells.Wells[0], wells.Wells[1].Q - wells.Wells[0].Q, times[i] - wells.Wells[1].Time1) + wells.Wells[0].P0);

                            }
                        }

                    }
                }
                List<double> P1f = new List<double>();
                List<double> P1s = new List<double>();
                List<double> T1f = new List<double>();
                List<double> T1s = new List<double>();
                for (int i = 0; i != times.Count - counter1; i++)
                {
                    P1f.Add(pOne[i]);
                    T1f.Add(times[i]);
                }
                for (int i = 0; i != counter1; i++)
                {
                    P1s.Add(pOne[times.Count - counter1 + i]);
                    T1s.Add(times[times.Count - counter1 + i]);
                }

                List<double> P2new = new List<double>(counter1);
                List<double> T2new = new List<double>(counter1);
                for (int i = 0; i < counter1; i++)
                {
                    T2new.Add(times[times.Count - counter1 + i]);
                    P2new.Add(pTwo[i]);
                }

                // T2new to P1S
                pressures.AddRange(P1f);
                wells.Wells[0].P = P1f.Last();
                //indexes.Add(pressures.Count);
                pressures.AddRange(P2new);
                wells.Wells[1].P = P2new.Last();
                pressures.RemoveAt(wells.Wells[0].N);
                // indexes.Add(pressures.Count);
                times.RemoveAt(wells.Wells[0].N);

                pressuresAndTimes.Pressures1f = P1f;
                pressuresAndTimes.Times1f = T1f;
                pressuresAndTimes.Pressures1s = P1s;
                pressuresAndTimes.Times1s = T1s;
                pressuresAndTimes.Pressures2 = P2new;
                pressuresAndTimes.Times2 = T2new;
            }

            #endregion
            if (wells.Wells.Count == 3)
            {
                List<double> tOne = new List<double>();
                List<double> tTwo = new List<double>();
                List<double> tThree = new List<double>();
                List<double> pOne = new List<double>();
                List<double> pTwo = new List<double>();
                List<double> pThree = new List<double>();
                int index1 = 0;
                int index2 = 0;
                bool ind_flag1 = false;
                bool ind_flag2 = false;
                double step1 = (wells.Wells[0].Time2 - wells.Wells[0].Time1) / (wells.Wells[0].N - 1);
                double step2 = (wells.Wells[1].Time2 - wells.Wells[1].Time1) / (wells.Wells[1].N - 1);
                double step3 = (wells.Wells[2].Time2 - wells.Wells[2].Time1) / (wells.Wells[2].N - 1);
                for (int i = 0; i != wells.Wells[0].N; i++)
                {
                    tOne.Add(wells.Wells[0].Time1 + i * step1);
                }
                for (int i = 0; i != wells.Wells[1].N; i++)
                {
                    tTwo.Add(wells.Wells[1].Time1 + i * step2);
                }
                for (int i = 0; i != wells.Wells[2].N; i++)
                {
                    tThree.Add(wells.Wells[2].Time1 + i * step3);
                }
                double Q1, Q2, Q3;
                if (wells.Wells[0].Mode == Mode.Reverse)
                {
                    Q1 = wells.Wells[0].CalculatedQ;
                    Q2 = wells.Wells[1].CalculatedQ;
                    Q3 = wells.Wells[2].CalculatedQ;
                }
                else
                {
                    Q1 = wells.Wells[0].Q;
                    Q2 = wells.Wells[1].Q;
                    Q3 = wells.Wells[2].Q;
                }
                for (int i = 0; i != times.Count; i++)
                {
                    if (times[i] == 0.0)
                    {
                        pOne.Add(0.0 + wells.Wells[0].P0);
                    }
                    else
                    {
                        pOne.Add(Pressure(wells.Wells[0], Q1, times[i]) + wells.Wells[0].P0);
                        if (times[i] > wells.Wells[1].Time1 || i > wells.Wells[0].N - 1)
                        {
                            if (ind_flag1 == false)
                            {
                                index1 = i;
                                ind_flag1 = true;
                            }

                            pTwo.Add(Pressure(wells.Wells[0], Q1, times[i])
                                + Pressure(wells.Wells[1], Q2 - Q1, times[i] - wells.Wells[1].Time1)
                                + wells.Wells[0].P0);
                        }
                        if ((times[i] > wells.Wells[2].Time1) || (i > wells.Wells[0].N + wells.Wells[1].N - 1))
                        {
                            if (ind_flag2 == false)
                            {
                                index2 = i;
                                ind_flag2 = true;
                            }
                            pThree.Add(Pressure(wells.Wells[0], Q1, times[i])
                                  + Pressure(wells.Wells[1], Q2 - Q1, times[i] - wells.Wells[1].Time1)
                                  + Pressure(wells.Wells[1], Q3 - Q2, times[i] - wells.Wells[2].Time1)
                                   + wells.Wells[0].P0);
                        }
                    }
                }


                List<double> P1f = new List<double>();
                List<double> P1s = new List<double>();
                List<double> T1f = new List<double>();
                List<double> T1s = new List<double>();

                for (int i = 0; i < index1; i++)
                {
                    P1f.Add(pOne[i]);
                    T1f.Add(times[i]);
                }
                for (int i = 0; i < times.Count - (index1); i++)
                {
                    P1s.Add(pOne[i + index1]);
                    T1s.Add(times[i + index1]);
                }

                List<double> T2f = new List<double>((index2 + 1) - (index1 + 1));
                List<double> P2f = new List<double>((index2 + 1) - (index1 + 1));
                List<double> T2s = new List<double>((times.Count - 1) - (index2));
                List<double> P2s = new List<double>((times.Count - 1) - (index2));
                for (int i = 0; i < (index2 - index1); i++)
                {
                    T2f.Add(times[i + index1]);
                    P2f.Add(pTwo[i]);
                }
                for (int i = 0; i != (times.Count) - index2; i++)
                {
                    T2s.Add(times[i + index2]);
                    P2s.Add(pTwo[i + index2 - wells.Wells[0].N]);
                }
                List<double> T3new = new List<double>(times.Count - 1 - index2);
                List<double> P3new = new List<double>(times.Count - 1 - index2);
                for (int i = 0; i != times.Count - (index2); i++)
                {
                    T3new.Add(times[i + index2]);
                    P3new.Add(pThree[i]);
                }


                pressures.AddRange(P1f);
                wells.Wells[0].CalculatedP = P1f.Last();
                //indexes.Add(pressures.Count);
                pressures.AddRange(P2f);
                wells.Wells[1].CalculatedP = P2f.Last();
                pressures.RemoveAt(wells.Wells[0].N);
                //indexes.Add(pressures.Count);
                pressures.AddRange(P3new);
                wells.Wells[2].CalculatedP = P3new.Last();
                pressures.RemoveAt(wells.Wells[0].N + wells.Wells[1].N - 1);
                //indexes.Add(pressures.Count);                                   
                times.RemoveAt(wells.Wells[0].N);
                times.RemoveAt(wells.Wells[0].N + wells.Wells[1].N - 1);

                pressuresAndTimes.Pressures1f = P1f;
                pressuresAndTimes.Times1f = T1f;
                pressuresAndTimes.Pressures1s = P1s;
                pressuresAndTimes.Times1s = T1s;
                pressuresAndTimes.Pressures2f = P2f;
                pressuresAndTimes.Times2f = T2f;
                pressuresAndTimes.Pressures2s = P2s;
                pressuresAndTimes.Times2s = T2s;
                pressuresAndTimes.Pressures3 = P3new;
                pressuresAndTimes.Times3 = T3new;
            }

        }

        public static List<double> GetTimes(List<Well> wells, bool cameFromPressure)
        {
            List<double> times = new List<double>();
            double step1, step2, step3;
            switch (wells.Count)
            {
                #region Unused

                case 1:
                    step1 = (wells[0].Time2 - wells[0].Time1) / (wells[0].N - 1);
                    for (int i = 0; i < wells[0].N; i++)
                    {
                        times.Add(wells[0].Time1 + i * step1);
                    }
                    break;
                case 2:
                    step1 = (wells[0].Time2 - wells[0].Time1) / (wells[0].N - 1);
                    step2 = (wells[1].Time2 - wells[1].Time1) / (wells[1].N - 1);
                    for (int i = 0; i != wells[0].N; i++)
                    {
                        times.Add(wells[0].Time1 + i * step1);
                    }
                    for (int i = 0; i != wells[1].N; i++)
                    {
                        times.Add(wells[1].Time1 + i * step2);
                    }
                    if (wells[0].Mode == Mode.Reverse)
                    {
                        times.RemoveAt(wells[0].N);
                    }
                    break; 
                #endregion
                case 3:
                    step1 = (wells[0].Time2 - wells[0].Time1) / (wells[0].N - 1);
                    step2 = (wells[1].Time2 - wells[1].Time1) / (wells[1].N - 1);
                    step3 = (wells[2].Time2 - wells[2].Time1) / (wells[2].N - 1);
                    for (int i = 0; i != wells[0].N; i++)
                    {
                        times.Add(wells[0].Time1 + i * step1);
                    }
                    for (int i = 0; i != wells[1].N; i++)
                    {
                        times.Add(wells[1].Time1 + i * step2);
                    }
                    for (int i = 0; i != wells[2].N; i++)
                    {
                        times.Add(wells[2].Time1 + i * step3);
                    }
                    if (!cameFromPressure)
                    {
                        times.RemoveAt(wells[0].N);
                        times.RemoveAt(wells[0].N + wells[1].N - 1);
                    }
                    break;
            }

            return times;
        }

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
                        for (int i = 0; i < wells.Indexes[0]; i++)
                            eqPressures.Add(wells.Wells[0].CalculatedP - wells.Wells[0].P0);
                        for (int i = wells.Indexes[0]; i <= wells.Indexes[1]; i++)
                            eqPressures.Add(wells.Wells[1].CalculatedP - wells.Wells[0].P0);
                        for (int i = wells.Indexes[1]; i < wells.Indexes[2]; i++)
                            eqPressures.Add(wells.Wells[2].CalculatedP - wells.Wells[0].P0);
                        break;
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

        public static void PrepareStaticPressures(WellsList wells, List<double> staticPressures)
        {
            switch (wells.Indexes.Count)
            {
                case 1:
                    for (int i = 0; i != wells.Indexes[0]; i++)
                    {
                        staticPressures.Add(wells.Wells[0].P);
                    }
                    break;
                case 2:
                    for (int i = 0; i != wells.Indexes[0]; i++)
                    {
                        staticPressures.Add(wells.Wells[0].P);
                    }
                    for (int i = wells.Indexes[0]; i < wells.Indexes[1] + 1; i++)
                    {
                        staticPressures.Add(wells.Wells[1].P);
                    }
                    break;
                case 3:
                    for (int i = 0; i != wells.Wells[0].N; i++)
                    {
                        staticPressures.Add(wells.Wells[0].P);
                    }
                    for (int i = 0; i < wells.Wells[1].N; i++)
                    {
                        staticPressures.Add(wells.Wells[1].P);
                    }
                    for (int i = 0; i < wells.Wells[2].N; i++)
                    {
                        staticPressures.Add(wells.Wells[2].P);
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
            for (int i = 0; i < X.Count; i++)
            {
                var sum = 0.0;
                for (int j = 0; j < i; j++)
                {
                    sum += A[i][j] * X[j];
                }
                X[i] = (B[i] - sum) / A[i][i];
            }
        }

        private static bool Converge(List<double> xk, List<double> xkp)
        {
            double eps = 0.000001;
            double norm = 0;
            for (int i = 0; i < xk.Count; i++)
                norm += (xk[i] - xkp[i]) * (xk[i] - xkp[i]);
            return (Math.Sqrt(norm) < eps);
        }
        #endregion

        public static void GetConsumtions(WellsList wells, out List<double> consumptions)
        {
            List<double> times = GetTimes(wells.Wells, false);
            consumptions = new List<double>();
            for (int i = 0; i < times.Count - 1; i++)
                consumptions.Add(0);
            List<List<double>> coefs;
            List<double> eqPressures;
            PrepareEqPressures(wells, out eqPressures);
            PrepareCoefs(times, wells.Wells, out coefs);

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string writePath1 = Path.Combine(Environment.CurrentDirectory, "times.txt");

            using (StreamWriter sw1 = new StreamWriter(writePath1, false, Encoding.Default))
            {
                for (int k = 0; k < times.Count; k++)
                {
                    sw1.Write(times[k] + " ");
                }
            }

            GaussSeidel(coefs, eqPressures, consumptions);
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
            List<double> Qk;
            List<double> QkDelta;
            List<double> QkappaDelta;
            List<double> QksiDelta;
            List<double> QP0Delta;

            WellsList wlGradWells = new WellsList { Wells = gradientWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            GetConsumtions(wlGradWells, out Qk); // Q_k

            WellsList wlKWells = new WellsList { Wells = kWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            GetConsumtions(wlKWells, out QkDelta); // k+delta

            WellsList wlKappaWells = new WellsList { Wells = kappaWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            GetConsumtions(wlKappaWells, out QkappaDelta); // kappa+delta

            WellsList wlKsiWells = new WellsList { Wells = ksiWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            GetConsumtions(wlKsiWells, out QksiDelta); // ksi+delta

            WellsList wlP0Wells = new WellsList { Wells = p0wells, Indexes = gradientAndWellsList.WellsList.Indexes };
            GetConsumtions(wlP0Wells, out QP0Delta); // p0+delta
                                                     // подсчет градиента
            #endregion

            #region gradient projections evaluation
            double gradientK = 0;
            double gradientKappa = 0;
            double gradientKsi = 0;
            double gradientP0 = 0;
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

                    gradientK = (fk - f) / (gradientAndWellsList.Gradient.DeltaK*Math.Pow(10.0, 15)*Math.Pow(10,3));

                    var fkappa = (Math.Pow(gradientWells[0].Q - QkappaDelta[wlKWells.Indexes[0] - 2], 2)
                                  + Math.Pow(gradientWells[1].Q - QkappaDelta[wlKWells.Indexes[1] - 1], 2)
                                  + Math.Pow(gradientWells[2].Q - QkappaDelta.Last(), 2))
                                  / (Math.Pow(gradientWells[0].Q, 2)
                                  + Math.Pow(gradientWells[1].Q, 2)
                                  + Math.Pow(gradientWells[2].Q, 2));
                    gradientKappa = (fkappa - f) /( gradientAndWellsList.Gradient.DeltaKappa*3600.0 * Math.Pow(10, 3));
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
            #region adaptive step evaluation
            int i1 = (gradientAndWellsList.Gradient.UsedK ?? false) ? 1 : 0;
            int i2 = (gradientAndWellsList.Gradient.UsedKappa ?? false) ? 1 : 0;
            int i3 = (gradientAndWellsList.Gradient.UsedKsi ?? false) ? 1 : 0;
            int i4 = (gradientAndWellsList.Gradient.UsedP0 ?? false) ? 1 : 0;
            int kNum = DegreeEvaluation(gradientAndWellsList.Gradient.UsedK, gradientK, gradientAndWellsList.Gradient.ChangedK, gradientAndWellsList.Gradient.DeltaK);
            int kappaNum = DegreeEvaluation(gradientAndWellsList.Gradient.UsedKappa, gradientKappa, gradientAndWellsList.Gradient.ChangedKappa, gradientAndWellsList.Gradient.DeltaKappa);
            int ksiNum = DegreeEvaluation(gradientAndWellsList.Gradient.UsedKsi, gradientKsi, gradientAndWellsList.Gradient.ChangedKsi, gradientAndWellsList.Gradient.DeltaKsi);
            int p0Num = /*!(i1 == 0 && i2 == 0 && i3 == 0) ? 0 :*/ DegreeEvaluation(gradientAndWellsList.Gradient.UsedP0, gradientP0, gradientAndWellsList.Gradient.ChangedP0, gradientAndWellsList.Gradient.DeltaP0);
            //int p0NumNorm = DegreeEvaluation(gradientAndWellsList.Gradient.UsedP0, gradientP0norm, gradientAndWellsList.Gradient.ChangedP0*Math.Pow(10,-6), gradientAndWellsList.Gradient.DeltaP0 * Math.Pow(10, -6));
            //nextGradient.Lambda *= Math.Pow(10, MaxDegreeEvaluation(kNum, kappaNum, ksiNum, p0Num));
            #endregion

            double kNext = /*(1 - 2 * nextGradient.Lambda * i1) * */gradientAndWellsList.Gradient.ChangedK * Math.Pow(10.0, 15) - i1 * nextGradient.Lambda * gradientK;
            double kappaNext = /*(1 - 2 * nextGradient.Lambda * i2) **/ gradientAndWellsList.Gradient.ChangedKappa * 3600.0  - i2 * nextGradient.Lambda * gradientKappa;
            double ksiNext =/* (1 - 2 * nextGradient.Lambda * i3) **/ gradientAndWellsList.Gradient.ChangedKsi - i3 * nextGradient.Lambda * gradientKsi;
            double p0Next =/* (1 - 2 * nextGradient.Lambda * i4) * */gradientAndWellsList.Gradient.ChangedP0 * Math.Pow(10.0, -6) - i4 * nextGradient.Lambda * gradientP0;
            if ((kNext > 0) && (kappaNext > 0) && (ksiNext >= 0) && (p0Next >= 0))
            {
                nextGradient.ChangedK = kNext * Math.Pow(10.0, -15);
                nextGradient.ChangedKappa = kappaNext * (1.0 / 3600.0);
                nextGradient.ChangedKsi = ksiNext;
                nextGradient.ChangedP0 = p0Next * Math.Pow(10.0, 6);
                //stack.push_front(*grK1);
                List<Well> Qk1wells = new List<Well>();
                Qk1wells.AddRange(gradientWells);
                for (int i = 0; i < Qk1wells.Count; i++)
                {
                    Qk1wells[i].K = nextGradient.ChangedK;
                    Qk1wells[i].Kappa = nextGradient.ChangedKappa ;
                    Qk1wells[i].Ksi = nextGradient.ChangedKsi;
                    Qk1wells[i].P0 = nextGradient.ChangedP0;
                }
                List<double> Qk1;
                WellsList wlQk1Wells = new WellsList { Wells = Qk1wells, Indexes = gradientAndWellsList.WellsList.Indexes };
                GetConsumtions(wlQk1Wells, out Qk1);
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
                gradientAndConsumptions.ConsumptionsAndTimes = consumptionsAndTimes;
                gradientAndConsumptions.QGradient = nextGradient;
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

            #region Qk QkDelta QkappaDelta QksiDelta QP0Delta evaluation

            WellsList wlGradWells = new WellsList { Wells = gradientWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            GetTimesAndPressures(wlGradWells, out PressuresAndTimes Pk); // Q_k

            WellsList wlKWells = new WellsList { Wells = kWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            GetTimesAndPressures(wlKWells, out PressuresAndTimes PkDelta); // k+delta

            WellsList wlKappaWells = new WellsList { Wells = kappaWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            GetTimesAndPressures(wlKappaWells, out PressuresAndTimes PkappaDelta); // kappa+delta

            WellsList wlKsiWells = new WellsList { Wells = ksiWells, Indexes = gradientAndWellsList.WellsList.Indexes };
            GetTimesAndPressures(wlKsiWells, out PressuresAndTimes PksiDelta); // ksi+delta

            WellsList wlP0Wells = new WellsList { Wells = p0wells, Indexes = gradientAndWellsList.WellsList.Indexes };
            GetTimesAndPressures(wlP0Wells, out PressuresAndTimes PP0Delta); // p0+delta
                                                           // подсчет градиента
            #endregion

            #region gradient projections evaluation
            double gradientK = 0;
            double gradientKappa = 0;
            double gradientKsi = 0;
            double gradientP0 = 0;
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

                    if (gradientAndWellsList.Gradient.DeltaKsi == 0)
                    {
                        gradientKsi = 0;
                    }
                    else
                    {
                        gradientKsi = ((Math.Pow((gradientWells[0].P - PksiDelta.Pressures1f.Last()), 2) +
                            Math.Pow((gradientWells[1].P - PksiDelta.Pressures2f.Last()), 2)
                            + Math.Pow((gradientWells[2].P - PksiDelta.Pressures3.Last()), 2))
                            - (Math.Pow((gradientWells[0].P - Pk.Pressures1f.Last()), 2)
                            + Math.Pow((gradientWells[1].P - Pk.Pressures2f.Last()), 2)
                            + Math.Pow((gradientWells[2].P - Pk.Pressures3.Last()), 2))) / gradientAndWellsList.Gradient.DeltaKsi;
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
            #region adaptive step evaluation
            int i1 = (gradientAndWellsList.Gradient.UsedK ?? false) ? 1 : 0;
            int i2 = (gradientAndWellsList.Gradient.UsedKappa ?? false) ? 1 : 0;
            int i3 = (gradientAndWellsList.Gradient.UsedKsi ?? false) ? 1 : 0;
            int i4 = (gradientAndWellsList.Gradient.UsedP0 ?? false) ? 1 : 0;
            int kNum = DegreeEvaluation(gradientAndWellsList.Gradient.UsedK, gradientK, gradientAndWellsList.Gradient.ChangedK, gradientAndWellsList.Gradient.DeltaK);
            int kappaNum = DegreeEvaluation(gradientAndWellsList.Gradient.UsedKappa, gradientKappa, gradientAndWellsList.Gradient.ChangedKappa, gradientAndWellsList.Gradient.DeltaKappa);
            int ksiNum = DegreeEvaluation(gradientAndWellsList.Gradient.UsedKsi, gradientKsi, gradientAndWellsList.Gradient.ChangedKsi, gradientAndWellsList.Gradient.DeltaKsi);
            int p0Num =/* !(i1 == 0 && i2 == 0 && i3 == 0) ? 0 : */DegreeEvaluation(gradientAndWellsList.Gradient.UsedP0, gradientP0, gradientAndWellsList.Gradient.ChangedP0, gradientAndWellsList.Gradient.DeltaP0);
            //int p0NumNorm = DegreeEvaluation(gradientAndWellsList.Gradient.UsedP0, gradientP0norm, gradientAndWellsList.Gradient.ChangedP0 * Math.Pow(10, -6), gradientAndWellsList.Gradient.DeltaP0 * Math.Pow(10, -6));
            //nextGradient.Lambda *= Math.Pow(10, MaxDegreeEvaluation(kNum, kappaNum, ksiNum, p0Num));
            #endregion

            double kNext = gradientAndWellsList.Gradient.ChangedK * Math.Pow(10.0, 15) - i1 * nextGradient.Lambda * gradientK;
            double kappaNext = gradientAndWellsList.Gradient.ChangedKappa * 3600.0 - i2 * nextGradient.Lambda * gradientKappa;
            double ksiNext = gradientAndWellsList.Gradient.ChangedKsi - i3 * nextGradient.Lambda * gradientKsi;
            double p0Next = gradientAndWellsList.Gradient.ChangedP0 * Math.Pow(10.0, -6) - i4 * nextGradient.Lambda * gradientP0;
            if ((kNext > 0) && (kappaNext > 0) && (ksiNext >= 0) && (p0Next >= 0))
            {
                nextGradient.ChangedK = kNext * Math.Pow(10.0, -15);
                nextGradient.ChangedKappa = kappaNext * (1.0 / 3600.0);
                nextGradient.ChangedKsi = ksiNext;
                nextGradient.ChangedP0 = p0Next * Math.Pow(10.0, 6);
                //stack.push_front(*grK1);
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
                //GetConsumtions(wlQk1Wells, out Qk1);
                GetTimesAndPressures(wlPk1Wells, out PressuresAndTimes Pk1);
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
                        Fmin = Math.Sqrt(Fmin / (Math.Pow(gradientWells[0].P, 2) + Math.Pow(gradientWells[1].P, 2) + Math.Pow(gradientWells[2].P, 2)));
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
                gradientAndPressures.PressuresAndTimes = pressuresAndTimes;
                gradientAndPressures.PGradient = nextGradient;
            }
        }

        private static int DegreeEvaluation(bool? usedProjection, double gradientValue, double changedValue, double deltaValue)
        {
            int degree = 0;
            if ((bool)usedProjection)
            {
                int count1 = 0;
                int count2 = 0;
                if (Math.Abs(changedValue + deltaValue) < 1 && (changedValue + deltaValue) != 0)
                {
                    double tempVal = Math.Abs(changedValue + deltaValue);
                    while (tempVal < 1)
                    {
                        count1--;
                        tempVal *= 10;
                    }
                }
                if (Math.Abs(changedValue + deltaValue) > 1 && (changedValue + deltaValue) != 0)
                {
                    double tempVal = Math.Abs(changedValue + deltaValue);
                    while (tempVal > 1)
                    {
                        count1++;
                        tempVal /= 10;
                    }
                }

                if (Math.Abs(gradientValue) < 1 && gradientValue != 0)
                {
                    double tempVal = Math.Abs(gradientValue);
                    while (tempVal < 1)
                    {
                        count2--;
                        tempVal *= 10;
                    }
                }
                if (Math.Abs(gradientValue) > 1 && gradientValue != 0)
                {
                    double tempVal = Math.Abs(gradientValue);
                    while (tempVal > 1)
                    {
                        count2++;
                        tempVal /= 10;
                    }
                }
                degree = count1 - count2;
            }
            return degree;
        }

        private static int MaxDegreeEvaluation(int kNum, int kappaNum, int ksiNum, int p0Num)
        {
            int maxDegree = 0;
            int returnVal = 0;
            bool b1 = false;
            bool b2 = false;
            bool b3 = false;
            bool b4 = false;
            if (maxDegree < Math.Abs(kNum))
            {
                b1 = true;
                b2 = b3 = b4 = false;
                maxDegree = Math.Abs(kNum);
                returnVal = kNum;
            }
            if (maxDegree < Math.Abs(kappaNum))
            {
                b2 = true;
                b1 = b3 = b4 = false;
                maxDegree = Math.Abs(kappaNum);
                returnVal = kappaNum;
            }
            if (maxDegree < Math.Abs(ksiNum))
            {
                b3 = true;
                b2 = b1 = b4 = false;
                maxDegree = Math.Abs(ksiNum);
                returnVal = ksiNum;
            }
            if (maxDegree < Math.Abs(p0Num))
            {
                b4 = true;
                b3 = b2 = b1 = false;
                maxDegree = Math.Abs(p0Num);
                returnVal = p0Num;
            }
            //if (b1 == true)
            //{
            //    nextGradient.Lambda *= Math.Pow(10, kNum);
            //}
            //if (b2 == true)
            //{
            //    nextGradient.Lambda *= Math.Pow(10, kappaNum);
            //}
            //if (b3 == true)
            //{
            //    nextGradient.Lambda *= Math.Pow(10, ksiNum);
            //}
            //if (b4 == true)
            //{
            //    nextGradient.Lambda *= Math.Pow(10, p0Num);
            //}
            return returnVal;
        }
        #endregion
    }
}
