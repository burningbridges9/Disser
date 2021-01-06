using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DisserNET.Models
{
    public class ValuesAndTimes { }
    public class ConsumptionsAndTimes : ValuesAndTimes
    {
        public List<double> Consumptions { get; set; }
        public List<double> Times { get; set; }
        public List<double> StaticConsumptions { get; set; }

        public List<DataPoint> ToDataPoints(bool staticConsumptions)
        {
            var dataPoints = new List<DataPoint>();

            if (staticConsumptions && StaticConsumptions is not null)
                dataPoints = zip(StaticConsumptions, Times);
            else if (Consumptions is not null)
                dataPoints = zip(Consumptions, Times);
            return dataPoints;

            List<DataPoint> zip(List<double> c, List<double> t)
            {
                var dataPoints = new List<DataPoint>();
                c.Zip(t, Tuple.Create).ToList().ForEach(z => dataPoints.Add(new DataPoint(z.Item2 / 3600.0, z.Item1 * 24.0 * 3600.0)));
                return dataPoints;
            }
        }
    }
}
