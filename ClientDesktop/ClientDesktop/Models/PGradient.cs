using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.Models
{
    public class PGradient : Gradient
    {
        [JsonIgnore]
        private double _FminP;
        [JsonProperty("FminP")]
        public double FminP
        {
            get { return _FminP; }
            set
            {
                _FminP = value;
                OnPropertyChanged("FminP");
            }
        }
    }
}
