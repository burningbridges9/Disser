using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputationalServer.Models
{
    public class Well
    {
        [JsonProperty("Q")]
        public double Q { get; set; }
        [JsonProperty("P")]
        public double P { get; set; }
        [JsonProperty("P0")]
        public double P0 { get; set; }
        [JsonProperty("Time1")]
        public double Time1 { get; set; }
        [JsonProperty("Time2")]
        public double Time2 { get; set; }
        [JsonProperty("H0")]
        public double H0 { get; set; }
        [JsonProperty("Mu")]
        public double Mu { get; set; }
        [JsonProperty("Rw")]
        public double Rw { get; set; }
        [JsonProperty("K")]
        public double K { get; set; }
        [JsonProperty("Kappa")]
        public double Kappa { get; set; }
        [JsonProperty("Rs")]
        public double Rs { get; set; }
        [JsonProperty("Ksi")]
        public double Ksi { get; set; }
        [JsonProperty("N")]
        public int N { get; set; }
        [JsonProperty("Mode")]
        public Mode Mode { get; set; }
    }
}
