using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HydrodynamicStudies.Models
{
    public class MetropolisHastings : IWellFeaturesInclude, INotifyPropertyChanged
    {
        #region fields
        private int walksCount;
        private int ns;
        private double maxK;
        private double maxKappa;
        private double maxP0;
        private double maxKsi;
        private double minK;
        private double minKappa;
        private double minP0;
        private double minKsi;
        private double stepK;
        private double stepKappa;
        private double stepP0;
        private double stepKsi;
        private bool includedK;
        private bool includedKappa;
        private bool includedP0;
        private bool includedKsi;
        private double s_0;
        private double c;
        #endregion

        public int WalksCount { get => walksCount; set { walksCount = value; OnPropertyChanged("WalksCount"); } }
        public int M
        {
            get => (IncludedK ? 1 : 0) + (IncludedKappa ? 1 : 0) + (IncludedKsi ? 1 : 0) + (IncludedP0 ? 1 : 0);
        }
        public int Ns { get => ns; set { ns = value; OnPropertyChanged("Ns"); } }
        public double MaxK { get => maxK; set { maxK = value; OnPropertyChanged("MaxK"); } }
        public double MaxKappa { get => maxKappa; set { maxKappa = value; OnPropertyChanged("MaxKappa"); } }
        public double MaxP0 { get => maxP0; set { maxP0 = value; OnPropertyChanged("MaxP0"); } }
        public double MaxKsi { get => maxKsi; set { maxKsi = value; OnPropertyChanged("MaxKsi"); } }
        public double MinK { get => minK; set { minK = value; OnPropertyChanged("MinK"); } }
        public double MinKappa { get => minKappa; set { minKappa = value; OnPropertyChanged("MinKappa"); } }
        public double MinP0 { get => minP0; set { minP0 = value; OnPropertyChanged("MinP0"); } }
        public double MinKsi { get => minKsi; set { minKsi = value; OnPropertyChanged("MinKsi"); } }
        public double StepK { get => stepK; set { stepK = value; OnPropertyChanged("StepK"); } }
        public double StepKappa { get => stepKappa; set { stepKappa = value; OnPropertyChanged("StepKappa"); } }
        public double StepP0 { get => stepP0; set { stepP0 = value; OnPropertyChanged("StepP0"); } }
        public double StepKsi { get => stepKsi; set { stepKsi = value; OnPropertyChanged("StepKsi"); } }
        public bool IncludedK { get => includedK; set { includedK = value; OnPropertyChanged("IncludedK"); } }
        public bool IncludedKappa { get => includedKappa; set { includedKappa = value; OnPropertyChanged("IncludedKappa"); } }
        public bool IncludedP0 { get => includedP0; set { includedP0 = value; OnPropertyChanged("IncludedP0"); } }
        public bool IncludedKsi { get => includedKsi; set { includedKsi = value; OnPropertyChanged("IncludedKsi"); } }

        public double S_0 { get => s_0; set { s_0 = value; OnPropertyChanged("S_0"); } }
        public double C { get => c; set { c = value; OnPropertyChanged("C"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
