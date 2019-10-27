using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputationalServer.Models
{
    public class WellsList
    {
        [JsonProperty("Wells")]
        public List<Well> Wells { get; set; }
        [JsonProperty("Indexes")]
        public List<int> Indexes { get; set; }
    }
}