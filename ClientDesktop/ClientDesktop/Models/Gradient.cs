using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.Models
{
    public class Gradient
    {
        public double ChangedK { get; set; }
        public double ChangedKappa { get; set; }
        public double ChangedKsi { get; set; }
        public double ChangedP0 { get; set; }
        public double DeltaK { get; set; }
        public double DeltaKappa { get; set; }
        public double DeltaKsi { get; set; }
        public double DeltaP0 { get; set; }
        public double GradientK { get; set; }
        public double GradientKappa { get; set; }
        public double GradientKsi { get; set; }
        public double GradientP0 { get; set; }
        public bool? UsedK { get; set; }
        public bool? UsedKappa { get; set; }
        public bool? UsedKsi { get; set; }
        public bool? UsedP0 { get; set; }
        public double Lambda { get; set; }
        
        public double FminQ { get; set; }
        //{
        //    get
        //    {
        //        return _F;
        //    }

        //    set
        //    {
        //        if (value == _F)
        //            return;

        //        _F = value;
        //        OnPropertyChanged("_F");
        //    }

        //}
        //[JsonIgnore]
        //private double _F;
        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
