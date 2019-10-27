using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputationalServer.Models
{
    public class GradientAndWellsList
    {
        [JsonProperty("Gradient")]
        public Gradient Gradient { get; set; }
        [JsonProperty("WellsList")]
        public WellsList WellsList { get; set; }
    }
}
