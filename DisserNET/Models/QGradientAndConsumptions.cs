namespace DisserNET.Models
{
    public class QGradientAndConsumptions : IGradientAndValues<QGradient, ConsumptionsAndTimes>
    {
        public QGradient Grad { get; set; }
        public ConsumptionsAndTimes ValuesAndTimes { get; set; }
    }
}
