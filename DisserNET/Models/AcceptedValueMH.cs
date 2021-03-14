using DisserNET.Calculs;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DisserNET.Models
{
    public class AcceptedValueMH : IWellFeaturesInclude, IWell, INotifyPropertyChanged
    {
        private int acceptedCount;
        private double probabilityDensity;
        private double fmin;
        private double k;
        private double kappa;
        private double ksi;
        private double p0;
        private bool includedK;
        private bool includedKappa;
        private bool includedP0;
        private bool includedKsi;

        public int AcceptedCount { get => acceptedCount; set { acceptedCount = value; OnPropertyChanged(prop: "AcceptedCount"); } }
        public double ProbabilityDensity { get => probabilityDensity; set { probabilityDensity = value; OnPropertyChanged(prop: "ProbabilityDensity"); } }
        public double Fmin { get => fmin; set { fmin = value; OnPropertyChanged(prop: "Fmin"); } }
        public double K { get => k; set { k = value; OnPropertyChanged(prop: "K"); } }
        public double Kappa { get => kappa; set { kappa = value; OnPropertyChanged(prop: "Kappa"); } }
        public double Ksi { get => ksi; set { ksi = value; OnPropertyChanged(prop: "Ksi"); } }
        public double P0 { get => p0; set { p0 = value; OnPropertyChanged(prop: "P0"); } }
        public bool IncludedK { get => includedK; set { includedK = value; OnPropertyChanged(prop: "IncludedK"); } }
        public bool IncludedKappa { get => includedKappa; set { includedKappa = value; OnPropertyChanged(prop: "IncludedKappa"); } }
        public bool IncludedP0 { get => includedP0; set { includedP0 = value; OnPropertyChanged(prop: "IncludedP0"); } }
        public bool IncludedKsi { get => includedKsi; set { includedKsi = value; OnPropertyChanged(prop: "IncludedKsi"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public AcceptedValueMH GetNormalized()
        {
            var k = Converter.ConvertBack(this.K, ValueType.K);
            var kappa = Converter.ConvertBack(this.Kappa, ValueType.Kappa);
            var ksi = Converter.ConvertBack(this.Ksi, ValueType.Ksi);
            var p = Converter.ConvertBack(this.P0, ValueType.P);
            var a = new AcceptedValueMH()
            {
                AcceptedCount = this.AcceptedCount,
                ProbabilityDensity = this.ProbabilityDensity,

                Fmin = this.Fmin,
                IncludedK = this.IncludedK,
                IncludedKappa = this.IncludedKappa,
                IncludedP0 = this.IncludedP0,
                IncludedKsi = this.IncludedKsi,
                K = k,
                Kappa = kappa,
                Ksi = ksi,
                P0 = p,
            };
            return a;
        }
    }
}
