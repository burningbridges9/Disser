namespace DisserNET.Models
{
    public class PGradient : Gradient
    {
        private double _FminP;
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
