namespace DisserNET.Models
{
    public class GradientAndWellsList<T> where T : Gradient
    {
        public T Gradient { get; set; }
        public WellsList WellsList { get; set; }
    }
}
