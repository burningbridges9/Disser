using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputationalServer.Actions
{
    public class IntegralCalculator
    {
        public static double PolyApproxExpIntegral1(double arg) //1 < ARG < INF
        {
            double a1, a2, a3, a4, b1, b2, b3, b4, sum1, sum2;
            a1 = 8.5733287401;
            a2 = 18.0590169730;
            a3 = 8.6347608925;
            a4 = 0.2677737343;
            b1 = 9.5733223454;
            b2 = 25.6329561486;
            b3 = 21.0996530827;
            b4 = 3.9584969228;
            sum1 = Math.Pow(arg, 4) + a1 * Math.Pow(arg, 3) + a2 * Math.Pow(arg, 2) + a3 * arg + a4;
            sum2 = Math.Pow(arg, 4) + b1 * Math.Pow(arg, 3) + b2 * Math.Pow(arg, 2) + b3 * arg + b4;
            return (sum1 / (sum2 * arg * Math.Pow(Math.E, arg)));
        }

        public static double PolyApproxExpIntegral2(double arg) // 0 < ARG < 1
        {
            double a0, a1, a2, a3, a4, a5, sum, E;
            a0 = -0.57721566;
            a1 = 0.99999193;
            a2 = -0.24991055;
            a3 = 0.05519968;
            a4 = -0.00976004;
            a5 = 0.00107857;
            sum = a0 + a1 * arg + a2 * Math.Pow(arg, 2) + a3 * Math.Pow(arg, 3) + a4 * Math.Pow(arg, 4) + a5 * Math.Pow(arg, 5);
            E = sum - Math.Log(arg);
            return E;
        }

        public static double EIntegral(double arg)
        {
            double E = 0;
            if (arg > 1)
            {
                E = PolyApproxExpIntegral1(arg);
            }
            if (arg < 1)
            {
                E = PolyApproxExpIntegral2(arg);
            }
            return E;
        }
    }
}
