using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputationalServer.Models
{
    public class PressuresAndTimes
    {
        #region if one debit
        public List<double> Pressures1 { get; set; }
        public List<double> Times1 { get; set; }
        #endregion
        #region if two debits
        public List<double> Pressures1f { get; set; }
        public List<double> Times1f{ get; set; }
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
    }
}
