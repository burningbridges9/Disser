using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Calculs
{
    public enum ValueToConvert
    {
        K,
        Kappa,
        Ksi,
        P,
    }
    public class Converter
    {
        /// <summary>
        /// Used to convert values to system C
        /// </summary>
        /// <param name="val"></param>
        /// <param name="valueToConvert"></param>
        /// <returns></returns>
        public static double Convert(double val, ValueToConvert valueToConvert)
        {
            switch (valueToConvert)
            {
                case ValueToConvert.K:
                    return val * Math.Pow(10.0, -15);
                case ValueToConvert.Kappa:
                    return val * (1.0 / 3600.0);
                case ValueToConvert.Ksi:
                    return val;
                case ValueToConvert.P:
                    return val * Math.Pow(10.0, 6);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Used to convert values from system C
        /// </summary>
        /// <param name="val"></param>
        /// <param name="valueToConvert"></param>
        /// <returns></returns>
        public static double ConvertBack(double val, ValueToConvert valueToConvert)
        {
            switch (valueToConvert)
            {
                case ValueToConvert.K:
                    return val * Math.Pow(10.0, 15);
                case ValueToConvert.Kappa:
                    return val * 3600.0;
                case ValueToConvert.Ksi:
                    return val;
                case ValueToConvert.P:
                    return val * Math.Pow(10.0, -6);
                default:
                    return 0;
            }
        }
    }
}
