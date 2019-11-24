using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.Models
{
    public class QGradient : Gradient
    {
        [JsonIgnore]
        private double _FminQ;
        [JsonProperty("FminQ")]
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
