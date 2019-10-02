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
        public static void GetTimesAndPressures(List<Models.Well> wells, out List<double> times, out List<double> pressures, out List<double> indexes)
        {
            times = new List<double>();
            pressures= new List<double>();
            indexes = new List<double>();
            if (wells.Count == 1)
            {
                List<double> tOne = new List<double>(wells[0].N);
                List<double> pOne = new List<double>(wells[0].N);
                int index = 0;
                bool flag = false;
                double step = (wells[0].Time2- wells[0].Time1) / (wells[0].N - 1);
                for (int i = 0; i < wells[0].N; i++)
                {
                    tOne.Add(wells[0].Time1 + i * step);
                }
                for (int i = 0; i != wells[0].N; i++)
                {
                    if (tOne[i] == 0.0)
                    {
                        pOne.Add(0);
                    }
                    else
                    {
                        pOne.Add(Pressure(wells[0], wells[0].Q, tOne[i]));
                    }
                }
                pressures = pOne;
                times = tOne;               
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
                        pOne.Add(0.0);
                    }
                    else
                    {
                        pOne.Add(Pressure(wells[0], wells[0].Q, times[i]));

                        if ((times[i] >= wells[1].Time1) && (i >= wells[0].N))
                        {
                            counter1++;
                            {
                                pTwo.Add(Pressure(wells[0], wells[0].Q, times[i])
                                        + Pressure(wells[0], wells[1].Q - wells[0].Q, times[i] - wells[1].Time1));

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
                        pOne.Add(0.0);
                    }
                    else
                    {
                        pOne.Add(Pressure(wells[0], wells[0].Q, times[i]));
                        if (times[i] >= wells[1].Time1)
                        {
                            if (ind_flag1 == false)
                            {
                                index1 = i;
                                ind_flag1 = true;
                            }

                            pTwo.Add(Pressure(wells[0],wells[0].Q, times[i])
                                + Pressure(wells[1], wells[1].Q - wells[0].Q, times[i] - wells[1].Time1));
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
                                  + Pressure(wells[1], wells[2].Q - wells[1].Q, times[i] - wells[2].Time1));

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
                    T2f[i] = times[i + index1];
                    P2f[i] = pTwo[i + index1];
                }
                for (int i = 0; i != (times.Count - 1) - index2; i++)
                {
                    T2s[i] = times[i + index2];
                    P2s[i] = pTwo[i + index2];
                }

                List<double> T3new = new List<double>(times.Count - 1 - index2);
                List<double> P3new = new List<double>(times.Count - 1 - index2);
                for (int i = 0; i != times.Count - (index2 + 1); i++)
                {
                    T3new[i] = times[i + index2];
                    P3new[i] = pThree[i + index2];
                }

                
                pressures = P1f;                                                       
                indexes.Add(pressures.Count);
                pressures.AddRange(P2f);
                pressures.RemoveAt(wells[0].N);                                 
                indexes.Add(pressures.Count);
                pressures.AddRange(P3new);
                pressures.RemoveAt(wells[0].N + wells[1].N - 1);         
                indexes.Add(pressures.size());                                   
                times.RemoveAt(wells[0].N);                                     
                times.RemoveAt(wells[0].N + wells[1].N - 1);             
                
            }

        }
    }
}
