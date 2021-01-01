using System.ComponentModel;

namespace DisserNET.Models
{
    public class AcceptedValueMH : IWellFeaturesInclude, IWell, INotifyPropertyChanged
    {
        public int AcceptedCount { get; set; }
        public double ProbabilityDensity { get; set; }
        public double Fmin { get; set; }
        public double K { get; set; }
        public double Kappa { get; set; }
        public double Ksi { get; set; }
        public double P0 { get; set; }
        public bool IncludedK { get; set; }
        public bool IncludedKappa { get; set; }
        public bool IncludedP0 { get; set; }
        public bool IncludedKsi { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
