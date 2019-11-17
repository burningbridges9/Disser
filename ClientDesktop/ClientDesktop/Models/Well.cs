using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.Models
{
    public class Well : INotifyPropertyChanged
    {
        [JsonIgnore]
        private double _Q;
        [JsonProperty("Q")]
        public double Q
        {
            get { return _Q; }
            set
            {
                _Q = value;
                OnPropertyChanged("Q");
            }
        }

        [JsonIgnore]
        private double _P;
        [JsonProperty("P")]
        public double P
        {
            get { return _P; }
            set
            {
                _P = value;
                OnPropertyChanged("P");
            }
        }

        [JsonIgnore]
        private double _P0;
        [JsonProperty("P0")]
        public double P0
        {
            get { return _P0; }
            set
            {
                _P0 = value;
                OnPropertyChanged("P0");
            }
        }

        [JsonIgnore]
        private double _Time1;
        [JsonProperty("Time1")]
        public double Time1
        {
            get { return _Time1; }
            set
            {
                _Time1 = value;
                OnPropertyChanged("Time1");
            }
        }


        [JsonIgnore]
        private double _Time2;
        [JsonProperty("Time2")]
        public double Time2
        {
            get { return _Time2; }
            set
            {
                _Time2 = value;
                OnPropertyChanged("Time2");
            }
        }


        [JsonIgnore]
        private double _H0;
        [JsonProperty("H0")]
        public double H0
        {
            get { return _H0; }
            set
            {
                _H0 = value;
                OnPropertyChanged("H0");
            }
        }


        [JsonIgnore]
        private double _Mu;
        [JsonProperty("Mu")]
        public double Mu
        {
            get { return _Mu; }
            set
            {
                _Mu = value;
                OnPropertyChanged("Mu");
            }
        }

        [JsonIgnore]
        private double _Rw;
        [JsonProperty("Rw")]
        public double Rw
        {
            get { return _Rw; }
            set
            {
                _Rw = value;
                OnPropertyChanged("Rw");
            }
        }

        [JsonIgnore]
        private double _K;
        [JsonProperty("K")]
        public double K
        {
            get { return _K; }
            set
            {
                _K = value;
                OnPropertyChanged("K");
            }
        }

        [JsonIgnore]
        private double _Kappa;
        [JsonProperty("Kappa")]
        public double Kappa
        {
            get { return _Kappa; }
            set
            {
                _Kappa = value;
                OnPropertyChanged("Kappa");
            }
        }

        [JsonIgnore]
        private double _Rs;
        [JsonProperty("Rs")]
        public double Rs
        {
            get { return _Rs; }
            set
            {
                _Rs = value;
                OnPropertyChanged("Rs");
            }
        }

        [JsonIgnore]
        private double _Ksi;
        [JsonProperty("Ksi")]
        public double Ksi
        {
            get { return _Ksi; }
            set
            {
                _Ksi = value;
                OnPropertyChanged("Ksi");
            }
        }

        [JsonIgnore]
        private int _N;
        [JsonProperty("N")]
        public int N
        {
            get { return _N; }
            set
            {
                _N = value;
                OnPropertyChanged("N");
            }
        }

        [JsonIgnore]
        private double _CalculatedP;
        [JsonProperty("CalculatedP")]
        public double CalculatedP
        {
            get { return _CalculatedP; }
            set
            {
                _CalculatedP = value;
                OnPropertyChanged("CalculatedP");
            }
        }

        [JsonIgnore]
        private double _CalculatedQ;
        [JsonProperty("CalculatedQ")]
        public double CalculatedQ 
        {
            get { return _CalculatedQ; }
            set
            {
                _CalculatedQ = value;
                OnPropertyChanged("CalculatedQ");
            }
        }

        [JsonProperty("Mode")]
        public Mode Mode { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
