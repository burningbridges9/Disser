using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicStudies.Models
{
    public class QGradient : Gradient
    {
        private double _FminQ;
        public double FminQ
        {
            get { return _FminQ; }
            set
            {
                _FminQ = value;
                OnPropertyChanged("FminQ");
            }
        }
    }
}
