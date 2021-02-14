using DisserNET.Calculs;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DisserNET.Models
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
        private SelectLogic selectLogic;
        private Mode mode;
        private MoveLogic moveLogic;
        private int nk;
        private int nkappa;
        private int nksi;
        private int np0;
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
        public SelectLogic SelectLogic { get => selectLogic; set { selectLogic = value; OnPropertyChanged("SelectLogic"); } }
        public MoveLogic MoveLogic { get => moveLogic; set { moveLogic = value; OnPropertyChanged("MoveLogic"); } }

        public Mode Mode { get => mode; set { mode = value; OnPropertyChanged("Mode"); } }

        public double S_0 { get => s_0; set { s_0 = value; OnPropertyChanged("S_0"); } }
        public double C { get => c; set { c = value; OnPropertyChanged("C"); } }



        public int NK { get => nk; set { nk = value; OnPropertyChanged("NK"); } }
        public int NKappa { get => nkappa; set { nkappa = value; OnPropertyChanged("NKappa"); } }
        public int NKsi { get => nksi; set { nksi = value; OnPropertyChanged("NK"); } }
        public int NP0 { get => np0; set { np0 = value; OnPropertyChanged("NP0"); } }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public MetropolisHastings GetNormalized()
        {
            var kmin = Converter.ConvertBack(this.MinK, ValueToConvert.K);
            var kmax = Converter.ConvertBack(this.MaxK, ValueToConvert.K);
            var kappaMin = Converter.ConvertBack(this.MinKappa, ValueToConvert.Kappa);
            var kappaMax = Converter.ConvertBack(this.MaxKappa, ValueToConvert.Kappa);
            var ksiMin = Converter.ConvertBack(this.MinKsi, ValueToConvert.Ksi);
            var ksiMax = Converter.ConvertBack(this.MaxKsi, ValueToConvert.Ksi);
            var pMin = Converter.ConvertBack(this.MinP0, ValueToConvert.P);
            var pMax = Converter.ConvertBack(this.MaxP0, ValueToConvert.P);
            var sk = Converter.ConvertBack(this.StepK, ValueToConvert.K);
            var skappa = Converter.ConvertBack(this.StepKappa, ValueToConvert.Kappa);
            var sksi = Converter.ConvertBack(this.StepKsi, ValueToConvert.Ksi);
            var sp = Converter.ConvertBack(this.StepP0, ValueToConvert.P);
            var nK= (int)((this.MaxK - this.MinK)/StepK);
            var nKappa = (int)((this.MaxKappa - this.MinKappa) / StepKappa);
            var nKsi = StepKsi != 0 ? (int)((this.MaxKsi - this.MinKsi) / StepKsi) : 0;
            var nP0 = (int)((this.MaxP0 - this.MinP0) / StepP0);
            var a = new MetropolisHastings()
            {

                IncludedK = this.IncludedK,
                IncludedKappa = this.IncludedKappa,
                IncludedP0 = this.IncludedP0,
                IncludedKsi = this.IncludedKsi,

                MinK = kmin,
                MinKappa = kappaMin,
                MinP0 = pMin,
                MinKsi = ksiMin,

                MaxK = kmax,
                MaxKappa = kappaMax,
                MaxKsi = ksiMax,
                MaxP0 = pMax,

                StepK = sk,
                StepKappa=skappa,
                StepKsi = sksi,
                StepP0 = sp,


                NK = nK,
                NKappa = nKappa,
                NKsi = nKsi,
                NP0 = nP0,

                Ns = this.Ns,
                S_0 = this.S_0,
                C = this.C,
                Mode = this.Mode,
                MoveLogic = this.MoveLogic,
                WalksCount = this.WalksCount,
                SelectLogic = this.SelectLogic,
            };
            return a;
        }
    }

    public enum SelectLogic
    {
        BasedOnAccepted,
        BasedOnWalks,
        AcceptAll,
    }

    public enum MoveLogic
    {
        Cyclic,
        StickToBorder,
        StepBack,
        Reject,
    }
}
