using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.Models
{
    public class PGradientAndWellsList
    {
        [JsonProperty("Gradient")]
        public PGradient Gradient { get; set; }
        [JsonProperty("WellsList")]
        public WellsList WellsList { get; set; }
    }
}
