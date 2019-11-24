using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.Models
{
    public class Gradient : INotifyPropertyChanged
    {
        [JsonIgnore]
        private double _ChangedK;
        [JsonProperty("ChangedK")]
        public double ChangedK
        {
            get { return _ChangedK; }
            set
            {
                _ChangedK = value;
                OnPropertyChanged("ChangedK");
            }
        }

        [JsonIgnore]
        private double _ChangedKappa;
        [JsonProperty("ChangedKappa")]
        public double ChangedKappa
        {
            get { return _ChangedKappa; }
            set
            {
                _ChangedKappa = value;
                OnPropertyChanged("ChangedKappa");
            }
        }

        [JsonIgnore]
        private double _ChangedKsi;
        [JsonProperty("ChangedKsi")]
        public double ChangedKsi
        {
            get { return _ChangedKsi; }
            set
            {
                _ChangedKsi = value;
                OnPropertyChanged("ChangedKsi");
            }
        }

        [JsonIgnore]
        private double _ChangedP0;
        [JsonProperty("ChangedP0")]
        public double ChangedP0
        {
            get { return _ChangedP0; }
            set
            {
                _ChangedP0 = value;
                OnPropertyChanged("ChangedP0");
            }
        }

        [JsonIgnore]
        private double _DeltaK;
        [JsonProperty("DeltaK")]
        public double DeltaK
        {
            get { return _DeltaK; }
            set
            {
                _DeltaK = value;
                OnPropertyChanged("DeltaK");
            }
        }

        [JsonIgnore]
        private double _DeltaKappa;
        [JsonProperty("DeltaKappa")]
        public double DeltaKappa
        {
            get { return _DeltaKappa; }
            set
            {
                _DeltaKappa = value;
                OnPropertyChanged("DeltaKappa");
            }
        }

        [JsonIgnore]
        private double _DeltaKsi;
        [JsonProperty("DeltaKsi")]
        public double DeltaKsi
        {
            get { return _DeltaKsi; }
            set
            {
                _DeltaKsi = value;
                OnPropertyChanged("DeltaKsi");
            }
        }

        [JsonIgnore]
        private double _DeltaP0;
        [JsonProperty("DeltaP0")]
        public double DeltaP0
        {
            get { return _DeltaP0; }
            set
            {
                _DeltaP0 = value;
                OnPropertyChanged("DeltaP0");
            }
        }

        [JsonIgnore]
        private double _GradientK;
        [JsonProperty("GradientK")]
        public double GradientK
        {
            get { return _GradientK; }
            set
            {
                _GradientK = value;
                OnPropertyChanged("GradientK");
            }
        }

        [JsonIgnore]
        private double _GradientKappa;
        [JsonProperty("GradientKappa")]
        public double GradientKappa
        {
            get { return _GradientKappa; }
            set
            {
                _GradientKappa = value;
                OnPropertyChanged("GradientKappa");
            }
        }

        [JsonIgnore]
        private double _GradientKsi;
        [JsonProperty("GradientKsi")]
        public double GradientKsi
        {
            get { return _GradientKsi; }
            set
            {
                _GradientKsi = value;
                OnPropertyChanged("GradientKsi");
            }
        }

        [JsonIgnore]
        private double _GradientP0;
        [JsonProperty("GradientP0")]
        public double GradientP0
        {
            get { return _GradientP0; }
            set
            {
                _GradientP0 = value;
                OnPropertyChanged("GradientP0");
            }
        }

        [JsonIgnore]
        private bool? _UsedK;
        [JsonProperty("UsedK")]
        public bool? UsedK
        {
            get { return _UsedK; }
            set
            {
                _UsedK = value;
                OnPropertyChanged("UsedK");
            }
        }

        [JsonIgnore]
        private bool? _UsedKappa;
        [JsonProperty("UsedKappa")]
        public bool? UsedKappa
        {
            get { return _UsedKappa; }
            set
            {
                _UsedKappa = value;
                OnPropertyChanged("UsedKappa");
            }
        }

        [JsonIgnore]
        private bool? _UsedKsi;
        [JsonProperty("UsedKsi")]
        public bool? UsedKsi
        {
            get { return _UsedKsi; }
            set
            {
                _UsedKsi = value;
                OnPropertyChanged("UsedKsi");
            }
        }

        [JsonIgnore]
        private bool? _UsedP0;
        [JsonProperty("UsedP0")]
        public bool? UsedP0
        {
            get { return _UsedP0; }
            set
            {
                _UsedP0 = value;
                OnPropertyChanged("UsedP0");
            }
        }

        [JsonIgnore]
        private double _Lambda;
        [JsonProperty("Lambda")]
        public double Lambda
        {
            get { return _Lambda; }
            set
            {
                _Lambda = value;
                OnPropertyChanged("Lambda");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
