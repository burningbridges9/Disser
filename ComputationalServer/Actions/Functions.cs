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

        public static void GetTimesAndPressures(List<Models.Well> wells, out List<double> times, 
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
                indexes.Add(pressures.Count);
                pressures.AddRange(P2new);
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
                indexes.Add(pressures.Count);
                pressures.AddRange(P2f);
                pressures.RemoveAt(wells[0].N);                                 
                indexes.Add(pressures.Count);
                pressures.AddRange(P3new);
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
        public static void PrepareEqPressures(int count, List<double> pressures, List<int> indexes, out List<double> eqPressures, double P0)
        {
            eqPressures = new List<double>();
            switch(count)
            {
                case 1:
                    for (int i = 0; i != pressures.Count; i++)
                        eqPressures.Add(pressures[pressures.Count - 1] - P0);
                    break;
                case 2:
                    for (int i = 0; i != indexes[0]; i++)
                        eqPressures.Add(pressures[indexes[0] - 1] - P0);
                    for (int i = indexes[0]; i != pressures.Count; i++)
                        eqPressures.Add(pressures[indexes[1] - 1] - P0);
                    break;
                case 3:
                    for (int i = 0; i != indexes[0]; i++)
                        eqPressures.Add(pressures[indexes[0] - 1] - P0);
                    for (int i = indexes[0]; i != indexes[1]; i++)
                        eqPressures.Add(pressures[indexes[1] - 1] - P0);
                    for (int i = indexes[1]; i != indexes[2]; i++)
                        eqPressures.Add(pressures[indexes[2] - 1] - P0);
                    break;
            }
            eqPressures.RemoveAt(0);
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

        public static void GetConsumtions(List<double> times, List<Models.Well> wells, int count, List<double> pressures, 
            List<int> indexes, out List<double> consumptions, out List<double> staticConsumptions, double P0)
        {
            consumptions = new List<double>();
            staticConsumptions = new List<double>();
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
            for (int i = 0; i < times.Count-1; i++)
                consumptions.Add(0);
            List<List<double>> coefs;
            List<double> eqPressures;
            PrepareEqPressures(count, pressures, indexes, out eqPressures, P0);
            PrepareCoefs(times, wells, out coefs);
            GaussSeidel(coefs, eqPressures, consumptions);
        }
    }
}
