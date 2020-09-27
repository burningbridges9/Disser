using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HydrodynamicStudies.Models
{
    public class Well : INotifyPropertyChanged
    {
        private double _Q;
        public double Q
        {
            get { return _Q; }
            set
            {
                _Q = value;
                OnPropertyChanged("Q");
            }
        }

        private double _P;
        public double P
        {
            get { return _P; }
            set
            {
                _P = value;
                OnPropertyChanged("P");
            }
        }

        private double _P0;
        public double P0
        {
            get { return _P0; }
            set
            {
                _P0 = value;
                OnPropertyChanged("P0");
            }
        }

        private double _Time1;
        public double Time1
        {
            get { return _Time1; }
            set
            {
                _Time1 = value;
                OnPropertyChanged("Time1");
            }
        }


        private double _Time2;
        public double Time2
        {
            get { return _Time2; }
            set
            {
                _Time2 = value;
                OnPropertyChanged("Time2");
            }
        }


        private double _H0;
        public double H0
        {
            get { return _H0; }
            set
            {
                _H0 = value;
                OnPropertyChanged("H0");
            }
        }


        private double _Mu;
        public double Mu
        {
            get { return _Mu; }
            set
            {
                _Mu = value;
                OnPropertyChanged("Mu");
            }
        }

        private double _Rw;
        public double Rw
        {
            get { return _Rw; }
            set
            {
                _Rw = value;
                OnPropertyChanged("Rw");
            }
        }

        private double _K;
        public double K
        {
            get { return _K; }
            set
            {
                _K = value;
                OnPropertyChanged("K");
            }
        }

        private double _Kappa;
        public double Kappa
        {
            get { return _Kappa; }
            set
            {
                _Kappa = value;
                OnPropertyChanged("Kappa");
            }
        }

        private double _Rs;
        public double Rs
        {
            get { return _Rs; }
            set
            {
                _Rs = value;
                OnPropertyChanged("Rs");
            }
        }

        private double _Ksi;
        public double Ksi
        {
            get { return _Ksi; }
            set
            {
                _Ksi = value;
                OnPropertyChanged("Ksi");
            }
        }

        private int _N;
        public int N
        {
            get { return _N; }
            set
            {
                _N = value;
                OnPropertyChanged("N");
            }
        }

        private double _CalculatedP;
        public double CalculatedP
        {
            get { return _CalculatedP; }
            set
            {
                _CalculatedP = value;
                ReCalculate();
                OnPropertyChanged("CalculatedP");
            }
        }

        private double _CalculatedQ;
        public double CalculatedQ 
        {
            get { return _CalculatedQ; }
            set
            {
                _CalculatedQ = value;
                ReCalculate();
                OnPropertyChanged("CalculatedQ");
            }
        }

        private double _CalcMP;
        public double CalcMP
        {
            get { return _CalcMP; }
            set
            {
                _CalcMP = value;
                OnPropertyChanged("CalcMP");
            }
        }

        private double _CalcMQ;
        public double CalcMQ
        {
            get { return _CalcMQ; }
            set
            {
                _CalcMQ = value;
                OnPropertyChanged("CalcMQ");
            }
        }

        public Mode Mode { get; set; }

        private void ReCalculate()
        {
            CalcMQ = CalculatedQ * (24.0 * 3600.0);
            CalcMP = Math.Pow(10.0, -6) * CalculatedP;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
