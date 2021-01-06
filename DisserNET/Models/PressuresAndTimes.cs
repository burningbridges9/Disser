using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DisserNET.Models
{
    public class PressuresAndTimes : ValuesAndTimes
    {
        #region if one debit
        public List<double> Pressures1 { get; set; }
        public List<double> Times1 { get; set; }
        #endregion
        #region if two debits
        public List<double> Pressures1f { get; set; }
        public List<double> Times1f { get; set; }
        public List<double> Pressures1s { get; set; }
        public List<double> Times1s { get; set; }
        public List<double> Pressures2 { get; set; }
        public List<double> Times2 { get; set; }
        #endregion
        #region if three debits
        public List<double> Pressures2f { get; set; }
        public List<double> Times2f { get; set; }
        public List<double> Pressures2s { get; set; }
        public List<double> Times2s { get; set; }
        public List<double> Pressures3 { get; set; }
        public List<double> Times3 { get; set; }
        #endregion
        public List<double> StaticPressures { get; set; }


        public List<DataPoint> ToDataPoints(List<double> p, List<double> t, bool staticVal = false)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach (var pt in p.Zip(t, Tuple.Create))
            {
                dataPoints.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
            }
            return dataPoints;
        }
    }
}
