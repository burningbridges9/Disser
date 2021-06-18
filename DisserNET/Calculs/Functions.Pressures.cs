using DisserNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Calculs
{
    public partial class Functions
    {
        public static double Pressure(Well W, double q, double t) => t == 0.0 ? 0 : (q * W.Mu) / (4.0 * Math.PI * W.H0 * W.K) * (W.Ksi + IntegralCalculator.EIntegral(Math.Pow(W.Rs, 2) / (4.0 * W.Kappa * t)));

        public static PressuresAndTimes GetPressures(WellsList wellsList, bool prepareStatic = true)
        {
            PressuresAndTimes pressuresAndTimes = Functions.GetTimesAndPressures(wellsList);
            List<double> staticPressures = new List<double>();
            if (prepareStatic)
            {
                Functions.PrepareStaticPressures(wellsList, staticPressures);
                if (wellsList.Wells[0].Mode == Mode.Reverse)
                    pressuresAndTimes.StaticPressures = staticPressures;
            }
            wellsList.Wells[0].CalculatedP = pressuresAndTimes.Pressures1f.Last();
            wellsList.Wells[1].CalculatedP = pressuresAndTimes.Pressures2f.Last();
            wellsList.Wells[2].CalculatedP = pressuresAndTimes.Pressures3.Last();
            return pressuresAndTimes;
        }

        public static PressuresAndTimes GetTimesAndPressures(WellsList wells)
        {
            List<double> times = GetTimes(wells.Wells, true);
            List<double> pressures = new List<double>();
            var pressuresAndTimes = new PressuresAndTimes();
            #region Unused

            //if (wells.Wells.Count == 1)
            //{
            //    List<double> tOne = new List<double>(wells.Wells[0].N);
            //    List<double> pOne = new List<double>(wells.Wells[0].N);
            //    double step = (wells.Wells[0].Time2 - wells.Wells[0].Time1) / (wells.Wells[0].N - 1);
            //    for (int i = 0; i < wells.Wells[0].N; i++)
            //    {
            //        tOne.Add(wells.Wells[0].Time1 + i * step);
            //    }
            //    for (int i = 0; i != wells.Wells[0].N; i++)
            //    {
            //        if (tOne[i] == 0.0)
            //        {
            //            pOne.Add(0 + wells.Wells[0].P0);
            //        }
            //        else
            //        {
            //            pOne.Add(Pressure(wells.Wells[0], wells.Wells[0].Q, tOne[i]) + wells.Wells[0].P0);
            //        }
            //    }
            //    pressures = pOne;
            //    times = tOne;
            //    pressuresAndTimes.Pressures1 = pressures;
            //    pressuresAndTimes.Times1 = times;
            //    //indexes.Add(times.Count);
            //    wells.Wells[0].P = pressures.Last();
            //}

            //if (wells.Wells.Count == 2)
            //{
            //    List<double> tOne = new List<double>();
            //    List<double> tTwo = new List<double>();
            //    List<double> pOne = new List<double>();
            //    List<double> pTwo = new List<double>();
            //    int counter1 = 0;
            //    double step1 = (wells.Wells[0].Time2 - wells.Wells[0].Time1) / (wells.Wells[0].N - 1);
            //    double step2 = (wells.Wells[1].Time2 - wells.Wells[1].Time1) / (wells.Wells[1].N - 1);
            //    for (int i = 0; i != wells.Wells[0].N; i++)
            //    {
            //        tOne.Add(wells.Wells[0].Time1 + i * step1);
            //    }
            //    for (int i = 0; i != wells.Wells[1].N; i++)
            //    {
            //        tTwo.Add(wells.Wells[1].Time1 + i * step2);
            //    }
            //    times.AddRange(tOne);
            //    times.AddRange(tTwo);
            //    for (int i = 0; i != times.Count; i++)
            //    {
            //        if (times[i] == 0.0)
            //        {
            //            pOne.Add(0.0 + wells.Wells[0].P0);
            //        }
            //        else
            //        {
            //            pOne.Add(Pressure(wells.Wells[0], wells.Wells[0].Q, times[i]) + wells.Wells[0].P0);

            //            if ((times[i] >= wells.Wells[1].Time1) && (i >= wells.Wells[0].N))
            //            {
            //                counter1++;
            //                {
            //                    pTwo.Add(Pressure(wells.Wells[0], wells.Wells[0].Q, times[i])
            //                           + Pressure(wells.Wells[0], wells.Wells[1].Q - wells.Wells[0].Q, times[i] - wells.Wells[1].Time1) + wells.Wells[0].P0);

            //                }
            //            }

            //        }
            //    }
            //    List<double> P1f = new List<double>();
            //    List<double> P1s = new List<double>();
            //    List<double> T1f = new List<double>();
            //    List<double> T1s = new List<double>();
            //    for (int i = 0; i != times.Count - counter1; i++)
            //    {
            //        P1f.Add(pOne[i]);
            //        T1f.Add(times[i]);
            //    }
            //    for (int i = 0; i != counter1; i++)
            //    {
            //        P1s.Add(pOne[times.Count - counter1 + i]);
            //        T1s.Add(times[times.Count - counter1 + i]);
            //    }

            //    List<double> P2new = new List<double>(counter1);
            //    List<double> T2new = new List<double>(counter1);
            //    for (int i = 0; i < counter1; i++)
            //    {
            //        T2new.Add(times[times.Count - counter1 + i]);
            //        P2new.Add(pTwo[i]);
            //    }

            //    // T2new to P1S
            //    pressures.AddRange(P1f);
            //    wells.Wells[0].P = P1f.Last();
            //    //indexes.Add(pressures.Count);
            //    pressures.AddRange(P2new);
            //    wells.Wells[1].P = P2new.Last();
            //    pressures.RemoveAt(wells.Wells[0].N);
            //    // indexes.Add(pressures.Count);
            //    times.RemoveAt(wells.Wells[0].N);

            //    pressuresAndTimes.Pressures1f = P1f;
            //    pressuresAndTimes.Times1f = T1f;
            //    pressuresAndTimes.Pressures1s = P1s;
            //    pressuresAndTimes.Times1s = T1s;
            //    pressuresAndTimes.Pressures2 = P2new;
            //    pressuresAndTimes.Times2 = T2new;
            //}

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
                    tTwo.Add(wells.Wells[1].Time1 + i * step2);
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
            return pressuresAndTimes;
        }

        public static List<double> GetTimes(List<Well> wells, bool cameFromPressure)
        {
            var times = new double[wells[0].N * wells.Count];
            double step1, step2, step3;
            switch (wells.Count)
            {
                #region Unused

                //case 1:
                //    step1 = (wells[0].Time2 - wells[0].Time1) / (wells[0].N - 1);
                //    for (int i = 0; i < wells[0].N; i++)
                //    {
                //        times.Add(wells[0].Time1 + i * step1);
                //    }
                //    break;
                //case 2:
                //    step1 = (wells[0].Time2 - wells[0].Time1) / (wells[0].N - 1);
                //    step2 = (wells[1].Time2 - wells[1].Time1) / (wells[1].N - 1);
                //    for (int i = 0; i != wells[0].N; i++)
                //    {
                //        times.Add(wells[0].Time1 + i * step1);
                //    }
                //    for (int i = 0; i != wells[1].N; i++)
                //    {
                //        times.Add(wells[1].Time1 + i * step2);
                //    }
                //    if (wells[0].Mode == Mode.Reverse)
                //    {
                //        times.RemoveAt(wells[0].N);
                //    }
                //    break;
                #endregion
                case 3:
                    step1 = (wells[0].Time2 - wells[0].Time1) / (wells[0].N - 1);
                    step2 = (wells[1].Time2 - wells[1].Time1) / (wells[1].N - 1);
                    step3 = (wells[2].Time2 - wells[2].Time1) / (wells[2].N - 1);
                    for (int i = 0; i != wells[0].N; i++)
                    {
                        times[i] = (wells[0].Time1 + i * step1);
                        times[i + wells[0].N] = (wells[1].Time1 + i * step2);
                        times[i + wells[0].N * 2] = (wells[2].Time1 + i * step3);
                    }
                    if (!cameFromPressure)
                    {
                        var tmpTimes = times.ToList();
                        tmpTimes.RemoveAt(wells[0].N);
                        tmpTimes.RemoveAt(wells[0].N + wells[1].N - 1);
                        return tmpTimes;
                    }
                    break;
            }

            return times.ToList();
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

    }
}
