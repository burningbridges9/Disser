namespace HydrodynamicStudies.Models
{
    public class MetropolisHastings : IWellFeaturesInclude
    {
        public int WalksCount { get; set; }
        public int M
        {
            get => (IncludedK ? 1 : 0) + (IncludedKappa ? 1 : 0) + (IncludedKsi ? 1 : 0) + (IncludedP0 ? 1 : 0);
        }
        public int Ns { get; set; }
        public double MaxK { get; set; }
        public double MaxKappa { get; set; }
        public double MaxP0 { get; set; }
        public double MaxKsi { get; set; }
        public double MinK { get; set; }
        public double MinKappa { get; set; }
        public double MinP0 { get; set; }
        public double MinKsi { get; set; }
        public double StepK { get; set; }
        public double StepKappa { get; set; }
        public double StepP0 { get; set; }
        public double StepKsi { get; set; }
        public bool IncludedK { get; set; }
        public bool IncludedKappa { get; set; }
        public bool IncludedP0 { get; set; }
        public bool IncludedKsi { get; set; }

        public double S_0 { get; set; }
        public double C { get; set; }
    }
}
