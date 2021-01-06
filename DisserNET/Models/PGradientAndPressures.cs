namespace DisserNET.Models
{
    public class PGradientAndPressures : IGradientAndValues<PGradient, PressuresAndTimes>
    {
        public PGradient Grad { get; set; }
        public PressuresAndTimes ValuesAndTimes { get; set; }
    }
}
