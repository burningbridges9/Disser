using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputationalServer.Models
{
    public class QGradientAndWellsList
    {
        [JsonProperty("Gradient")]
        public QGradient Gradient { get; set; }
        [JsonProperty("WellsList")]
        public WellsList WellsList { get; set; }
    }
}
