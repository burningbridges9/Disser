using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DisserNET.Models
{
    public class ConsumptionsAndTimes
    {
        public List<double> Consumptions { get; set; }
        public List<double> Times { get; set; }
        public List<double> StaticConsumptions { get; set; }

        public List<DataPoint> ToDataPoints(bool staticConsumptions)
        {
            var dataPoints = new List<DataPoint>();
            if (staticConsumptions)
            {
                foreach (var pt in StaticConsumptions.Zip(Times, Tuple.Create))
                {
                    dataPoints.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * 24.0 * 3600.0));
                }
            }
            else
            {
                foreach (var pt in Consumptions.Zip(Times, Tuple.Create))
                {
                    dataPoints.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * 24.0 * 3600.0));
                }
            }
            return dataPoints;
        }
    }
}
