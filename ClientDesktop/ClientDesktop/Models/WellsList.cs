using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.Models
{
    public class WellsList
    {
        [JsonProperty("Wells")]
        public List<Well> Wells { get; set; }
        [JsonProperty("Indexes")]
        public List<int> Indexes { get; set; }

        public WellsList(List<Well> Wells)
        {
            this.Wells = Wells;
            Indexes = new List<int>();
            for (int i = 0; i < Wells.Count; i++)
            {
                int tempVal = 0;
                for (int j = 0; j <= i; j++)
                {
                    tempVal += Wells[j].N;
                }
                Indexes.Add(tempVal-i);
            }
        }

    }
}
