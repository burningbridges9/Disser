using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Calculs.Helpers
{
    public class HCalc
    {
        private int variableRecieved = 0;
        public double NextH(double h, double w)
        {
            var retValue = h * (w - 0.5);

            return retValue;
        }

        public double NextH(double h, double w, double d)
        {
            var retValue = h * (w - 0.5);

            if (variableRecieved == 0)
            {
                retValue *= 1 + Math.Sign(0.5 - d);
            }
            else if (variableRecieved == 1)
            {
                retValue *= 1 + Math.Sign(d - 0.5);
            }

            this.variableRecieved++;
            return retValue;
        }

        public double NextH(double h, double w, int variablesCount = 3)
        {
            var retValue = h * (w - 0.5);
            var mult = 0;
            if (variablesCount == 3)
            {
                mult = variableRecieved switch
                {
                    0 => (w >= 0 && w < 1.0 / 3.0) ? 1 : 0,
                    1 => (w >= 1.0 / 3.0 && w < 2.0 / 3.0) ? 1 : 0,
                    2 => (w >= 2.0 / 3.0 && w <= 1) ? 1 : 0,
                    _ => 0
                };
            }
            else if (variablesCount == 4)
            {
                mult = variableRecieved switch
                {
                    0 => (w >= 0 && w < 0.25) ? 1 : 0,
                    1 => (w >= 0.25 && w < 0.5) ? 1 : 0,
                    2 => (w >= 0.5 && w <= 0.75) ? 1 : 0,
                    3 => (w >= 0.75 && w <= 1) ? 1 : 0,
                    _ => 0
                };
            }
            this.variableRecieved++;
            return retValue * mult;
        }

        public double NextH(double h, double w, double d, double g)
        {
            var retValue = h * (w - 0.5);

            switch (variableRecieved)
            {
                case 0:
                    retValue *= (1 + Math.Sign(0.5 - d)) * (1 + Math.Sign(g - 0.5));
                    break;
                case 1:
                    retValue *= (1 + Math.Sign(d - 0.5)) * (1 + Math.Sign(0.5 - g));
                    break;
                case 2:
                    retValue *= (1 + Math.Sign(d - 0.5)) * (1 + Math.Sign(g - 0.5));
                    break;
            }

            this.variableRecieved++;
            return retValue;
        }

        // to be approved
        public double NextH(double h, double w, double d, double g, double v)
        {
            var retValue = h * (w - 0.5);

            switch (variableRecieved)
            {
                case 0:
                    retValue *= (1 + Math.Sign(0.5 - d)) * (1 + Math.Sign(g - 0.5));
                    break;
                case 1:
                    retValue *= (1 + Math.Sign(d - 0.5)) * (1 + Math.Sign(0.5 - g));
                    break;
                case 2:
                    retValue *= (1 + Math.Sign(d - 0.5)) * (1 + Math.Sign(g - 0.5));
                    break;
                case 3:
                    retValue *= (1 + Math.Sign(d - 0.5)) * (1 + Math.Sign(g - 0.5));
                    break;
            }

            this.variableRecieved++;
            return retValue;
        }
    }
}
