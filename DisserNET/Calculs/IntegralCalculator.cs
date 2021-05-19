using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Calculs
{
    public class IntegralCalculator
    {
        class PolyApproxExpIntegral1Consts
        {
           public const double a1 = 8.5733287401;
           public const double a2 = 18.0590169730;
           public const double a3 = 8.6347608925;
           public const double a4 = 0.2677737343;
           public const double b1 = 9.5733223454;
           public const double b2 = 25.6329561486;
           public const double b3 = 21.0996530827;
           public const double b4 = 3.9584969228;
        }
        public static double PolyApproxExpIntegral1(double arg) //1 < ARG < INF
        {
            double sum1 = Math.Pow(arg, 4) + PolyApproxExpIntegral1Consts.a1 * Math.Pow(arg, 3) 
                + PolyApproxExpIntegral1Consts.a2 * Math.Pow(arg, 2) 
                + PolyApproxExpIntegral1Consts.a3 * arg
                + PolyApproxExpIntegral1Consts.a4;
            double sum2 = Math.Pow(arg, 4) + PolyApproxExpIntegral1Consts.b1 * Math.Pow(arg, 3) 
                + PolyApproxExpIntegral1Consts.b2 * Math.Pow(arg, 2) 
                + PolyApproxExpIntegral1Consts.b3 * arg 
                + PolyApproxExpIntegral1Consts.b4;
            return (sum1 / (sum2 * arg * Math.Pow(Math.E, arg)));
        }

        class PolyApproxExpIntegral2Consts
        {
            public const double a0 = -0.57721566;
            public const double a1 = 0.99999193;
            public const double a2 = -0.24991055;
            public const double a3 = 0.05519968;
            public const double a4 = -0.00976004;
            public const double a5 = 0.00107857;
        }
        public static double PolyApproxExpIntegral2(double arg) // 0 < ARG < 1
        {
            double sum, E;
            sum = PolyApproxExpIntegral2Consts.a0 + PolyApproxExpIntegral2Consts.a1 * arg 
                + PolyApproxExpIntegral2Consts.a2 * Math.Pow(arg, 2) 
                + PolyApproxExpIntegral2Consts.a3 * Math.Pow(arg, 3) 
                + PolyApproxExpIntegral2Consts.a4 * Math.Pow(arg, 4) 
                + PolyApproxExpIntegral2Consts.a5 * Math.Pow(arg, 5);
            E = sum - Math.Log(arg);
            return E;
        }

        public static double EIntegral(double arg) => arg > 1 ? PolyApproxExpIntegral1(arg) : PolyApproxExpIntegral2(arg);
    }
}
