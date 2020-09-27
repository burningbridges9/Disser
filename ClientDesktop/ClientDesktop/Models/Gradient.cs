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
        private double _ChangedK;
        public double ChangedK
        {
            get { return _ChangedK; }
            set
            {
                _ChangedK = value;
                OnPropertyChanged("ChangedK");
            }
        }

        private double _ChangedKappa;
        public double ChangedKappa
        {
            get { return _ChangedKappa; }
            set
            {
                _ChangedKappa = value;
                OnPropertyChanged("ChangedKappa");
            }
        }

        private double _ChangedKsi;
        public double ChangedKsi
        {
            get { return _ChangedKsi; }
            set
            {
                _ChangedKsi = value;
                OnPropertyChanged("ChangedKsi");
            }
        }

        private double _ChangedP0;
        public double ChangedP0
        {
            get { return _ChangedP0; }
            set
            {
                _ChangedP0 = value;
                OnPropertyChanged("ChangedP0");
            }
        }

        private double _DeltaK;
        public double DeltaK
        {
            get { return _DeltaK; }
            set
            {
                _DeltaK = value;
                OnPropertyChanged("DeltaK");
            }
        }

        private double _DeltaKappa;
        public double DeltaKappa
        {
            get { return _DeltaKappa; }
            set
            {
                _DeltaKappa = value;
                OnPropertyChanged("DeltaKappa");
            }
        }

        private double _DeltaKsi;
        public double DeltaKsi
        {
            get { return _DeltaKsi; }
            set
            {
                _DeltaKsi = value;
                OnPropertyChanged("DeltaKsi");
            }
        }

        private double _DeltaP0;
        public double DeltaP0
        {
            get { return _DeltaP0; }
            set
            {
                _DeltaP0 = value;
                OnPropertyChanged("DeltaP0");
            }
        }

        private double _GradientK;
        public double GradientK
        {
            get { return _GradientK; }
            set
            {
                _GradientK = value;
                OnPropertyChanged("GradientK");
            }
        }

        private double _GradientKappa;
        public double GradientKappa
        {
            get { return _GradientKappa; }
            set
            {
                _GradientKappa = value;
                OnPropertyChanged("GradientKappa");
            }
        }

        private double _GradientKsi;
        public double GradientKsi
        {
            get { return _GradientKsi; }
            set
            {
                _GradientKsi = value;
                OnPropertyChanged("GradientKsi");
            }
        }

        private double _GradientP0;
        public double GradientP0
        {
            get { return _GradientP0; }
            set
            {
                _GradientP0 = value;
                OnPropertyChanged("GradientP0");
            }
        }

        private bool? _UsedK;
        public bool? UsedK
        {
            get { return _UsedK; }
            set
            {
                _UsedK = value;
                OnPropertyChanged("UsedK");
            }
        }

        private bool? _UsedKappa;
        public bool? UsedKappa
        {
            get { return _UsedKappa; }
            set
            {
                _UsedKappa = value;
                OnPropertyChanged("UsedKappa");
            }
        }

        private bool? _UsedKsi;
        public bool? UsedKsi
        {
            get { return _UsedKsi; }
            set
            {
                _UsedKsi = value;
                OnPropertyChanged("UsedKsi");
            }
        }

        private bool? _UsedP0;
        public bool? UsedP0
        {
            get { return _UsedP0; }
            set
            {
                _UsedP0 = value;
                OnPropertyChanged("UsedP0");
            }
        }

        private double _Lambda;
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
