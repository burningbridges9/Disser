using ComputationalServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputationalServer.Actions
{
    public class Functions
    {
        public static double Pressure(Models.Well W, double q, double t)
        {
            double P, arg;
            arg = Math.Pow(W.Rs, 2) / (4.0 * W.Kappa * t);
            if (t == 0.0)
            {
                return 0;
            }
            return P = (q * W.Mu) / (4.0 * Math.PI * W.H0 * W.K) * (W.Ksi + IntegralCalculator.EIntegral(arg));
        }

        public static void GetTimesAndPressures(List<Well> wells, out List<double> times, 
            out List<double> pressures, out List<int> indexes, out PressuresAndTimes pressuresAndTimes)
        {
            times = new List<double>();
            pressures= new List<double>();
            indexes = new List<int>();
            pressuresAndTimes = new PressuresAndTimes();
            if (wells.Count == 1)
            {
                List<double> tOne = new List<double>(wells[0].N);
                List<double> pOne = new List<double>(wells[0].N);
                double step = (wells[0].Time2- wells[0].Time1) / (wells[0].N - 1);
                for (int i = 0; i < wells[0].N; i++)
                {
                    tOne.Add(wells[0].Time1 + i * step);
                }
                for (int i = 0; i != wells[0].N; i++)
                {
                    if (tOne[i] == 0.0)
                    {
                        pOne.Add(0 + wells[0].P0);
                    }
                    else
                    {
                        pOne.Add(Pressure(wells[0], wells[0].Q, tOne[i]) + wells[0].P0);
                    }
                }
                pressures = pOne;
                times = tOne;
                pressuresAndTimes.Pressures1= pressures;
                pressuresAndTimes.Times1=times;
                indexes.Add(times.Count);
                wells[0].P = pressures.Last();
            }

            if (wells.Count == 2)
            {
                List<double> tOne = new List<double>(wells[0].N);
                List<double> tTwo = new List<double>(wells[0].N);
                List<double> pOne = new List<double>(wells[0].N + wells[1].N);
                List<double> pTwo = new List<double>(wells[0].N + wells[1].N);
                int counter1 = 0;
                double step1 = (wells[0].Time2 - wells[0].Time1) / (wells[0].N - 1);
                double step2 = (wells[1].Time2 - wells[1].Time1) / (wells[1].N - 1);
                for (int i = 0; i != wells[0].N; i++)
                {
                    tOne.Add(wells[0].Time1 + i * step1);
                }
                for (int i = 0; i != wells[1].N; i++)
                {
                    tTwo.Add(wells[1].Time1 + i * step2);
                }
                times.AddRange(tOne);
                times.AddRange(tTwo);
                for (int i = 0; i != times.Count; i++)
                {
                    if (times[i] == 0.0)
                    {
                        pOne.Add(0.0 + wells[0].P0);
                    }
                    else
                    {
                        pOne.Add(Pressure(wells[0], wells[0].Q, times[i]) + wells[0].P0);

                        if ((times[i] >= wells[1].Time1) && (i >= wells[0].N))
                        {
                            counter1++;
                            {
                                pTwo.Add(Pressure(wells[0], wells[0].Q, times[i])
                                        + Pressure(wells[0], wells[1].Q - wells[0].Q, times[i] - wells[1].Time1) + wells[0].P0);

                            }
                        }

                    }
                }
                List<double> P1f = new List<double>(times.Count-counter1);
                List<double> P1s = new List<double>(counter1);
                List<double> T1f = new List<double>(times.Count - counter1);
                List<double> T1s = new List<double>(counter1);
                for (int i = 0; i != times.Count-counter1; i++)
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
                wells[0].P = P1f.Last();
                indexes.Add(pressures.Count);
                pressures.AddRange(P2new);
                wells[1].P = P2new.Last();
                pressures.RemoveAt(wells[0].N);
                indexes.Add(pressures.Count);
                times.RemoveAt(wells[0].N);

                pressuresAndTimes.Pressures1f=P1f;
                pressuresAndTimes.Times1f    =T1f;
                pressuresAndTimes.Pressures1s=P1s;
                pressuresAndTimes.Times1s    =T1s;
                pressuresAndTimes.Pressures2 =P2new;
                pressuresAndTimes.Times2     =T2new;
            }

            if (wells.Count == 3)
            {
                List<double> tOne = new List<double>(wells[0].N);
                List<double> tTwo = new List<double>(wells[1].N);
                List<double> tThree = new List<double>(wells[2].N);
                List<double> pOne = new List<double>(wells[0].N + wells[1].N + wells[2].N);
                List<double> pTwo = new List<double>(wells[0].N + wells[1].N + wells[2].N);
                List<double> pThree = new List<double>(wells[0].N + wells[1].N + wells[2].N);
                int index1 = 0;
                int index2 = 0;
                bool ind_flag1 = false;
                bool ind_flag2 = false;
                double step1 = (wells[0].Time2 - wells[0].Time1) / (wells[0].N - 1);
                double step2 = (wells[1].Time2 - wells[1].Time1) / (wells[1].N - 1);
                double step3 = (wells[2].Time2 - wells[2].Time1) / (wells[2].N - 1);
                for (int i = 0; i != wells[0].N; i++)
                {
                    tOne.Add(wells[0].Time1 + i * step1);
                }
                for (int i = 0; i != wells[1].N; i++)
                {
                    tTwo.Add(wells[1].Time1 + i * step2);
                }
                for (int i = 0; i != wells[2].N; i++)
                {
                    tThree.Add(wells[2].Time1 + i * step3);
                }
                times.AddRange(tOne);
                times.AddRange(tTwo);
                times.AddRange(tThree);
                for (int i = 0; i != times.Count; i++)
                {
                    if (times[i] == 0.0)
                    {
                        pOne.Add(0.0 + wells[0].P0);
                    }
                    else
                    {
                        pOne.Add(Pressure(wells[0], wells[0].Q, times[i]) + wells[0].P0);
                        if (times[i] >= wells[1].Time1)
                        {
                            if (ind_flag1 == false)
                            {
                                index1 = i;
                                ind_flag1 = true;
                            }

                            pTwo.Add(Pressure(wells[0],wells[0].Q, times[i])
                                + Pressure(wells[1], wells[1].Q - wells[0].Q, times[i] - wells[1].Time1) 
                                + wells[0].P0);
                        }
                        if ((times[i] >= wells[2].Time1) && (i >= wells[1].N))
                        {
                            if (ind_flag2 == false)
                            {
                                index2 = i;
                                ind_flag2 = true;
                            }
                            pThree.Add(Pressure(wells[0], wells[0].Q, times[i])
                                  + Pressure(wells[1], wells[1].Q - wells[0].Q, times[i] - wells[1].Time1)
                                  + Pressure(wells[1], wells[2].Q - wells[1].Q, times[i] - wells[2].Time1)
                                   + wells[0].P0);
                        }
                    }
                }
                

                List<double> P1f = new List<double>(index1+1);
                List<double> P1s = new List<double>(times.Count - (index1 + 1));
                List<double> T1f = new List<double>(index1 + 1);
                List<double> T1s = new List<double>(times.Count - (index1 + 1));

                for (int i = 0; i < index1 + 1; i++)
                {
                    P1f.Add(pOne[i]);
                    T1f.Add(times[i]);
                }
                for (int i = 0; i < times.Count - (index1 + 2); i++)
                {
                    P1s.Add(pOne[i + index1]);
                    T1s.Add(times[i + index1]);
                }

                List<double> T2f= new List<double>((index2+1)-(index1 + 1));
                List<double> P2f= new List<double>((index2+1)-(index1 + 1));
                List<double> T2s= new List<double>((times.Count - 1)-(index2));
                List<double> P2s = new List<double>((times.Count - 1)-(index2));
                for (int i = 0; i < (index2 - index1); i++)
                {
                    T2f.Add(times[i + index1]);
                    P2f.Add(pTwo[i]);
                }
                for (int i = 0; i != (times.Count - 1) - index2; i++)
                {
                    T2s.Add(times[i + index2]);
                    P2s.Add(pTwo[i + index2 - wells[0].N+1]);
                }
                List<double> T3new = new List<double>(times.Count - 1 - index2);
                List<double> P3new = new List<double>(times.Count - 1 - index2);
                for (int i = 0; i != times.Count - (index2 + 1); i++)
                {
                    T3new.Add(times[i + index2]);
                    P3new.Add(pThree[i]);
                }

                
                pressures = P1f;
                wells[0].P = P1f.Last();
                indexes.Add(pressures.Count);
                pressures.AddRange(P2f);
                wells[1].P = P2f.Last();
                pressures.RemoveAt(wells[0].N);                                 
                indexes.Add(pressures.Count);
                pressures.AddRange(P3new);
                wells[2].P = P3new.Last();
                pressures.RemoveAt(wells[0].N + wells[1].N - 1);         
                indexes.Add(pressures.Count);                                   
                times.RemoveAt(wells[0].N);                                     
                times.RemoveAt(wells[0].N + wells[1].N - 1);

                pressuresAndTimes.Pressures1f=P1f;
                pressuresAndTimes.Times1f    =T1f;
                pressuresAndTimes.Pressures1s=P1s;
                pressuresAndTimes.Times1s    =T1s;
                pressuresAndTimes.Pressures2f=P2f;
                pressuresAndTimes.Times2f    =T2f;
                pressuresAndTimes.Pressures2s=P2s;
                pressuresAndTimes.Times2s    =T2s;
                pressuresAndTimes.Pressures3 =P3new;
                pressuresAndTimes.Times3     =T3new;
            }

        }

        #region Prepare slae
        public static void PrepareEqPressures(int count, List<Well> wells, List<int> indexes, out List<double> eqPressures, double P0)
        {
            eqPressures = new List<double>();
            switch(count)
            {
                case 1:
                    for (int i = 0; i < indexes[0]; i++)
                        eqPressures.Add(wells[0].P - P0);
                    break;
                case 2:
                    for (int i = 0; i < indexes[0]; i++)
                        eqPressures.Add(wells[0].P - P0);
                    for (int i = indexes[0]; i < indexes[1]+1; i++)
                        eqPressures.Add(wells[1].P - P0);
                    break;
                case 3:
                    for (int i = 0; i < indexes[0]; i++)
                        eqPressures.Add(wells[0].P - P0);
                    for (int i = indexes[0]; i < indexes[1]+1; i++)
                        eqPressures.Add(wells[1].P - P0);
                    for (int i = indexes[1]; i < indexes[2]+1; i++)
                        eqPressures.Add(wells[2].P - P0);
                    break;
            }
            eqPressures.RemoveAt(0);
        }

        public static void PrepareStaticConsumptions(int count, List<Well> wells, List<int> indexes, List<double> staticConsumptions, List<double> times)
        {
            switch (count)
            {
                case 1:
                    for (int i = 0; i != indexes[0]; i++)
                    {
                        staticConsumptions.Add(wells[0].Q);
                    }
                    break;
                case 2:
                    for (int i = 0; i != indexes[0]; i++)
                    {
                        staticConsumptions.Add(wells[0].Q);
                    }
                    for (int i = indexes[0]; i < indexes[1] + 1; i++)
                    {
                        staticConsumptions.Add(wells[1].Q);
                    }
                    break;
                case 3:
                    for (int i = 0; i != indexes[0]; i++)
                    {
                        staticConsumptions.Add(wells[0].Q);
                    }
                    for (int i = indexes[0]; i < indexes[1] + 1; i++)
                    {
                        staticConsumptions.Add(wells[1].Q);
                    }
                    for (int i = indexes[1]; i < times.Count - 1; i++)
                    {
                        staticConsumptions.Add(wells[2].Q);
                    }
                    break;
            }
        }

        public static void PrepareCoefs(List<double> times, List<Models.Well> wells,  out List<List<double>> coefs)
        {
            coefs = new List<List<double>>(times.Count-1);
            for (int i = 0; i < times.Count-1; i++)
                coefs.Add(new List<double>());  
            int n = 1;
            for (int i = 0; i != times.Count-1; i++)
            {
                int k = 1;
                for (int j = 0; j != times.Count-1; j++)
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
                        coefs[i].Add(((wells[0].Mu * 1.0) / (4.0 * Math.PI * wells[0].K * wells[0].H0)) * (E1 - E2));
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
        public static void GaussSeidel(List<List<double>> A, List<double> B, List<double> X)
        {
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
            double eps = 0.000001;
            double norm = 0;
            for (int i = 0; i < xk.Count; i++)
                norm += (xk[i] - xkp[i]) * (xk[i] - xkp[i]);
            return (Math.Sqrt(norm) < eps);
        }
        #endregion

        public static void GetConsumtions(List<double> times, List<Well> wells, int count, List<double> pressures, 
            List<int> indexes, out List<double> consumptions,  double P0)
        {
            consumptions = new List<double>();
            for (int i = 0; i < times.Count-1; i++)
                consumptions.Add(0);
            List<List<double>> coefs;
            List<double> eqPressures;
            PrepareEqPressures(count, wells, indexes, out eqPressures, P0);
            PrepareCoefs(times, wells, out coefs);
            GaussSeidel(coefs, eqPressures, consumptions);
        }

        public static void GetNextGradientIteration(Gradient gradient, List<Well> gradientWells, List<double> times, 
            List<double> pressures, List<int> indexes, out GradientAndConsumptions gradientAndConsumptions)
        {
            gradientAndConsumptions = new GradientAndConsumptions();

            #region Wells fill
            List<Well> kWells       = new List<Well>();
            List<Well> kappaWells   = new List<Well>();
            List<Well> ksiWells     = new List<Well>();
            List<Well> p0wells      = new List<Well>();
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
                });
            }
            for (int i=0;i<gradientWells.Count;i++)
            {
                kWells[i].K = gradient.ChangedK + gradient.DeltaK;
                kappaWells[i].Kappa = gradient.ChangedKappa + gradient.DeltaKappa;
                ksiWells[i].Ksi = gradient.ChangedKsi + gradient.DeltaKsi;
                p0wells[i].P0 = gradient.ChangedP0 + gradient.DeltaP0;
            }
            #endregion

            #region Qk QkDelta QkappaDelta QksiDelta QP0Delta evaluation
            List<double> Qk;
            List<double> QkDelta;
            List<double> QkappaDelta;
            List<double> QksiDelta;
            List<double> QP0Delta;
            
            GetConsumtions(times, gradientWells, gradientWells.Count, pressures,
                indexes, out Qk, gradientWells[0].P0); // Q_k

            GetConsumtions(times, kWells, gradientWells.Count, pressures,
                indexes, out QkDelta, kWells[0].P0); // k+delta

            GetConsumtions(times, kappaWells, kappaWells.Count, pressures,
                indexes, out QkappaDelta, kappaWells[0].P0); // kappa+delta

            GetConsumtions(times, ksiWells, ksiWells.Count, pressures,
                indexes, out QksiDelta, ksiWells[0].P0); // ksi+delta

            GetConsumtions(times, p0wells, p0wells.Count, pressures,
                indexes, out QP0Delta, p0wells[0].P0); // p0+delta
                                                       // подсчет градиента
            #endregion

            double gradientK =0;
            double gradientKappa = 0;
            double gradientKsi = 0;
            double gradientP0 = 0;
            switch (gradientWells.Count)
            {
                #region case 1
                case 1:
                    gradientK = (Math.Pow((gradientWells[0].Q - QkDelta.Last()), 2) - Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradient.DeltaK;
                    gradientKappa = (Math.Pow((gradientWells[0].Q - QkappaDelta.Last()), 2) - Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradient.DeltaKappa;
                    if (gradient.DeltaKsi == 0)
                    {
                        gradientKsi = 0;
                    }
                    else
                    {
                        gradientKsi = (Math.Pow((gradientWells[0].Q - QksiDelta.Last()), 2) - Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradient.DeltaKsi;
                    }
                    if (gradient.DeltaP0 == 0)
                    {
                        gradientP0 = 0;
                    }
                    else
                    {
                        gradientP0 = (Math.Pow((gradientWells[0].Q - QP0Delta.Last()), 2) - Math.Pow((gradientWells[0].Q - Qk.Last()), 2)) / gradient.DeltaP0;
                    }
                    break;
                #endregion
                #region case 2
                case 2:
                    gradientK = ((Math.Pow((gradientWells[0].Q - QkDelta[indexes[0] - 2]), 2) +
                            Math.Pow((gradientWells[1].Q - QkDelta.Last()), 2))
                            - (Math.Pow((gradientWells[0].Q - Qk[indexes[0] - 2]), 2)
                            + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradient.DeltaK;
                    gradientKappa = ((Math.Pow((gradientWells[0].Q - QkappaDelta[indexes[0] - 2]), 2)
                            + Math.Pow((gradientWells[1].Q - QkappaDelta.Last()), 2))
                            - (Math.Pow((gradientWells[0].Q - Qk[indexes[0] - 2]), 2)
                            + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradient.DeltaKappa;
                    if (gradient.DeltaKsi == 0)
                    {
                        gradientKsi = 0;
                    }
                    else
                    {
                        gradientKsi = ((Math.Pow((gradientWells[0].Q - QksiDelta[indexes[0] - 2]), 2)
                                + Math.Pow((gradientWells[1].Q - QksiDelta.Last()), 2))
                                - (Math.Pow((gradientWells[0].Q - Qk[indexes[0] - 2]), 2)
                                + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradient.DeltaKsi;
                    }
                    if (gradient.DeltaP0 == 0)
                    {
                        gradientP0 = 0;
                    }
                    else
                    {
                        gradientP0 = ((Math.Pow((gradientWells[0].Q - QP0Delta[indexes[0] - 2]), 2)
                                + Math.Pow((gradientWells[1].Q - QP0Delta.Last()), 2))
                                - (Math.Pow((gradientWells[0].Q - Qk[indexes[0] - 2]), 2)
                                + Math.Pow((gradientWells[1].Q - Qk.Last()), 2))) / gradient.DeltaP0;
                    }
                    break;
                #endregion
                #region case 3
                case 3:
                    gradientK = ((Math.Pow((gradientWells[0].Q - QkDelta[indexes[0] - 2]), 2) +
                            Math.Pow((gradientWells[1].Q - QkDelta[indexes[1] - 2]), 2)
                            + Math.Pow((gradientWells[2].Q - QkDelta.Last()), 2))
                            - (Math.Pow((gradientWells[0].Q - Qk[indexes[0] - 2]), 2)
                            +Math.Pow((gradientWells[1].Q - Qk[indexes[1] - 2]), 2)
                            + Math.Pow((gradientWells[2].Q - Qk.Last()), 2))) / gradient.DeltaK;
                    gradientKappa = ((Math.Pow((gradientWells[0].Q - QkappaDelta[indexes[0] - 2]), 2) +
                            Math.Pow((gradientWells[1].Q - QkappaDelta[indexes[1] - 2]), 2)
                            + Math.Pow((gradientWells[2].Q - QkappaDelta.Last()), 2))
                            - (Math.Pow((gradientWells[0].Q - Qk[indexes[0] - 2]), 2)
                            + Math.Pow((gradientWells[1].Q - Qk[indexes[1] - 2]), 2)
                            + Math.Pow((gradientWells[2].Q - Qk.Last()), 2))) / gradient.DeltaKappa;
                    if (gradient.DeltaKsi == 0)
                    {
                        gradientKsi = 0;
                    }
                    else
                    {
                        gradientKsi = ((Math.Pow((gradientWells[0].Q - QksiDelta[indexes[0] - 2]), 2) +
                            Math.Pow((gradientWells[1].Q - QksiDelta[indexes[1] - 2]), 2)
                            + Math.Pow((gradientWells[2].Q - QksiDelta.Last()), 2))
                            - (Math.Pow((gradientWells[0].Q - Qk[indexes[0] - 2]), 2)
                            + Math.Pow((gradientWells[1].Q - Qk[indexes[1] - 2]), 2)
                            + Math.Pow((gradientWells[2].Q - Qk.Last()), 2))) / gradient.DeltaKsi;
                    }
                    if (gradient.DeltaP0 == 0)
                    {
                        gradientP0 = 0;
                    }
                    else
                    {
                        gradientP0 = ((Math.Pow((gradientWells[0].Q - QP0Delta[indexes[0] - 2]), 2) +
                            Math.Pow((gradientWells[1].Q - QP0Delta[indexes[1] - 2]), 2)
                            + Math.Pow((gradientWells[2].Q - QP0Delta.Last()), 2))
                            - (Math.Pow((gradientWells[0].Q - Qk[indexes[0] - 2]), 2)
                            + Math.Pow((gradientWells[1].Q - Qk[indexes[1] - 2]), 2)
                            + Math.Pow((gradientWells[2].Q - Qk.Last()), 2))) / gradient.DeltaP0;
                    }
                    break;
                    #endregion
            }
            Gradient nextGradient = new Gradient
            {
                Lambda = gradient.Lambda,
                GradientK =gradientK,
                GradientKappa = gradientKappa,
                GradientKsi = gradientKsi,
                GradientP0 = gradientP0
            };
            // подсчет корректного шага из условия колчиество символов (k|kappa|ksi + delta)+grad(k|kappa|ksi)

            int i1 = (gradient.UsedK ?? false) ? 1 : 0;
            int i2 = (gradient.UsedKappa ?? false) ? 1 : 0;
            int i3 = (gradient.UsedKsi ?? false) ? 1 : 0;
            int i4 = (gradient.UsedP0 ?? false) ? 1 : 0;
            int kNum = 0;
            int kappaNum = 0;
            int ksiNum = 0;
            int p0Num = 0;
            if ((bool)gradient.UsedK)
                {
                    int count1 = 0;
                    int count2 = 0;
                    if (Math.Abs(gradient.ChangedK + gradient.DeltaK) < 1 && Math.Abs(gradient.ChangedK + gradient.DeltaK) != 0)
                    {
                        double tempVal = Math.Abs(gradient.ChangedK + gradient.DeltaK);
                        while (tempVal < 1)
                        {
                            count1--;
                            tempVal *= 10;
                        }
                    }
                    if (Math.Abs(gradient.ChangedK + gradient.DeltaK) > 1 && Math.Abs(gradient.ChangedK + gradient.DeltaK) != 0)
                    {
                        double tempVal = Math.Abs(gradient.ChangedK + gradient.DeltaK);
                        while (tempVal > 1)
                        {
                            count1++;
                            tempVal /= 10;
                        }
                    }

                    if (Math.Abs(gradientK) < 1 && Math.Abs(gradientK) != 0)
                    {
                        double tempVal = Math.Abs(gradientK);
                        while (tempVal < 1)
                        {
                            count2--;
                            tempVal *= 10;
                        }
                    }
                    if (Math.Abs(gradientK) > 1 && Math.Abs(gradientK) != 0)
                    {
                        double tempVal = Math.Abs(gradientK);
                        while (tempVal > 1)
                        {
                            count2++;
                            tempVal /= 10;
                        }
                    }
                    kNum = count1 - count2;
                }
            if ((bool)gradient.UsedKappa)
                {
                    int count1 = 0;
                    int count2 = 0;
                    if (Math.Abs(gradient.ChangedKappa + gradient.DeltaKappa) < 1 && Math.Abs(gradient.ChangedKappa + gradient.DeltaKappa) != 0)
                    {
                        double tempVal = Math.Abs(gradient.ChangedKappa + gradient.DeltaKappa);
                        while (tempVal < 1)
                        {
                            count1--;
                            tempVal *= 10;
                        }
                    }
                    if (Math.Abs(gradient.ChangedKappa + gradient.DeltaKappa) > 1 && Math.Abs(gradient.ChangedKappa + gradient.DeltaKappa) != 0)
                    {
                        double tempVal = Math.Abs(gradient.ChangedKappa + gradient.DeltaKappa);
                        while (tempVal > 1)
                        {
                            count1++;
                            tempVal /= 10;
                        }
                    }

                    if (Math.Abs(gradientKappa) < 1 && Math.Abs(gradientKappa) != 0)
                    {
                        double tempVal = Math.Abs(gradientKappa);
                        while (tempVal < 1)
                        {
                            count2--;
                            tempVal *= 10;
                        }
                    }
                    if (Math.Abs(gradientKappa) > 1 && Math.Abs(gradientKappa) != 0)
                    {
                        double tempVal = Math.Abs(gradientKappa);
                        while (tempVal > 1)
                        {
                            count2++;
                            tempVal /= 10;
                        }
                    }
                    kappaNum = count1 - count2;
                }
            if ((bool)gradient.UsedKsi)
                {
                    int count1 = 0;
                    int count2 = 0;
                    if (Math.Abs(gradient.ChangedKsi + gradient.DeltaKsi) < 1 && Math.Abs(gradient.ChangedKsi + gradient.DeltaKsi) != 0)
                    {
                        double tempVal = Math.Abs(gradient.ChangedKsi + gradient.DeltaKsi);
                        while (tempVal < 1)
                        {
                            count1--;
                            tempVal *= 10;
                        }
                    }
                    if (Math.Abs(gradient.ChangedKsi + gradient.DeltaKsi) > 1 && Math.Abs(gradient.ChangedKsi + gradient.DeltaKsi) != 0)
                    {
                        double tempVal = Math.Abs(gradient.ChangedKsi + gradient.DeltaKsi);
                        while (tempVal > 1)
                        {
                            count1++;
                            tempVal /= 10;
                        }
                    }
                    if (Math.Abs(gradientKsi) < 1 && Math.Abs(gradientKsi) != 0)
                    {
                        double tempVal = Math.Abs(gradientKsi);
                        while (tempVal < 1)
                        {
                            count2--;
                            tempVal *= 10;
                        }
                    }
                    if (Math.Abs(gradientKsi) > 1 && Math.Abs(gradientKsi) != 0)
                    {
                        double tempVal = Math.Abs(gradientKsi);
                        while (tempVal > 1)
                        {
                            count2++;
                            tempVal /= 10;
                        }
                    }
                    ksiNum = count1 - count2;
                }
            if ((bool)gradient.UsedP0)
                {
                    int count1 = 0;
                    int count2 = 0;
                    if (Math.Abs(gradient.ChangedP0 + gradient.DeltaP0) < 1 && Math.Abs(gradient.ChangedP0 + gradient.DeltaP0) != 0)
                    {
                        double tempVal = Math.Abs(gradient.ChangedP0 + gradient.DeltaP0);
                        while (tempVal < 1)
                        {
                            count1--;
                            tempVal *= 10;
                        }
                    }
                    if (Math.Abs(gradient.ChangedP0 + gradient.DeltaP0) > 1 && Math.Abs(gradient.ChangedP0 + gradient.DeltaP0) != 0)
                    {
                        double tempVal = Math.Abs(gradient.ChangedP0 + gradient.DeltaP0);
                        while (tempVal > 1)
                        {
                            count1++;
                            tempVal /= 10;
                        }
                    }
                    if (Math.Abs(gradientP0) < 1 && Math.Abs(gradientP0) != 0)
                    {
                        double tempVal = Math.Abs(gradientP0);
                        while (tempVal < 1)
                        {
                            count2--;
                            tempVal *= 10;
                        }
                    }
                    if (Math.Abs(gradientP0) > 1 && Math.Abs(gradientP0) != 0)
                    {
                        double tempVal = Math.Abs(gradientP0);
                        while (tempVal > 1)
                        {
                            count2++;
                            tempVal /= 10;
                        }
                    }
                    p0Num = count1 - count2;
                }
            int maxDegree = 0;
            bool b1 = false;
            bool b2 = false;
            bool b3 = false;
            bool b4 = false;
            if (maxDegree < Math.Abs(kNum))
                {
                    b1 = true;
                    b2 = b3 = false;
                    maxDegree = Math.Abs(kNum);
                }
            if (maxDegree < Math.Abs(kappaNum))
                {
                    b2 = true;
                    b1 = b3 = false;
                    maxDegree = Math.Abs(kappaNum);
                }
            if (maxDegree < Math.Abs(ksiNum))
                {
                    b3 = true;
                    b2 = b1 = false;
                    maxDegree = Math.Abs(ksiNum);
                }
            if (maxDegree < Math.Abs(p0Num))
                {
                    b4 = true;
                    b3 = b2 = b1 = false;
                    maxDegree = Math.Abs(p0Num);
                }
            if (b1 == true)
            {
                nextGradient.Lambda *= Math.Pow(10, kNum);
            }
            if (b2 == true)
            {
                nextGradient.Lambda *= Math.Pow(10, kappaNum);
            }
            if (b3 == true)
            {
                nextGradient.Lambda *= Math.Pow(10, ksiNum);
            }
            if (b4 == true)
            {
                nextGradient.Lambda *= Math.Pow(10, p0Num);
            }

            double kNext = gradient.ChangedK - i1 * nextGradient.Lambda * gradientK;
            double kappaNext = gradient.ChangedKappa - i2 * nextGradient.Lambda * gradientKappa;
            double ksiNext = gradient.ChangedKsi - i3 * nextGradient.Lambda * gradientKsi;
            double p0Next = gradient.ChangedP0 - i4 * nextGradient.Lambda * gradientP0;
            if ((kNext > 0) && (kappaNext > 0) && (ksiNext >= 0) && (p0Next >= 0))
            {
                nextGradient.ChangedK = kNext;
                nextGradient.ChangedKappa = kappaNext;
                nextGradient.ChangedKsi = ksiNext;
                nextGradient.ChangedP0 = p0Next;
                //stack.push_front(*grK1);
                List<Well> Qk1wells = new List<Well>();
                Qk1wells.AddRange(gradientWells);
                for (int i =0;i<Qk1wells.Count;i++)
                {
                    Qk1wells[i].K = nextGradient.ChangedK;
                    Qk1wells[i].Kappa = nextGradient.ChangedKappa;
                    Qk1wells[i].Ksi = nextGradient.ChangedKsi;
                    Qk1wells[i].P0 = nextGradient.ChangedP0;
                }
                List<double> Qk1;
                GetConsumtions(times, Qk1wells, gradientWells.Count, pressures,
                    indexes, out Qk1, Qk1wells[0].P0); // Q_k+1

                double Fmin=0;
                switch (gradientWells.Count)
                {
                    case 1:
                        //                  (Q1 - Q_{k+1})^2
                        Fmin = Math.Pow((gradientWells[0].Q - Qk1.Last()), 2);
                        Fmin = Math.Sqrt(Fmin / (Math.Pow(gradientWells[0].Q, 2)));
                        break;
                    case 2:
                        Fmin = Math.Pow((gradientWells[0].Q - Qk1[indexes[0] - 2]), 2) + Math.Pow((gradientWells[1].Q - Qk1.Last()), 2);
                        Fmin = Math.Sqrt(Fmin / (Math.Pow(gradientWells[0].Q, 2) + Math.Pow(gradientWells[1].Q, 2)));
                        break;
                    case 3:
                        Fmin = Math.Pow((gradientWells[0].Q - Qk1[indexes[0] - 2]), 2)
                                + Math.Pow((gradientWells[1].Q - Qk1[indexes[1] - 2]), 2)
                                + Math.Pow((gradientWells[2].Q - Qk1.Last()), 2);
                        Fmin = Math.Sqrt(Fmin / (Math.Pow(gradientWells[0].Q, 2) + Math.Pow(gradientWells[1].Q, 2) + Math.Pow(gradientWells[2].Q, 2)));
                        break;

                }

                nextGradient.F = Fmin;
                ConsumptionsAndTimes consumptionsAndTimes = new ConsumptionsAndTimes { Times = times, Consumptions = Qk1 };
                gradientAndConsumptions.ConsumptionsAndTimes = consumptionsAndTimes;
                gradientAndConsumptions.Gradient = nextGradient;                
            }
        }

        //private int DegreeEvaluation(bool? usedProjection, )
        //{
        //    return 0;
        //}

        private int CorrectNextStepEvaluation()
        {
            return 0;
        }
    }
}
