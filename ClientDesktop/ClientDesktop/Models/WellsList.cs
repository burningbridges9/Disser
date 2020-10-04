using System.Collections.Generic;

namespace HydrodynamicStudies.Models
{
    public class WellsList
    {
        public List<Well> Wells { get; set; }
        public List<int> Indexes { get; set; }
        public WellsList()
        {
        }

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

        public void Clear()
        {
            Wells.Clear();
            Indexes.Clear();
        }
    }
}
