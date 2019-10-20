using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.ViewModels
{
    public class WellViewModel
    {
        public List<Models.Well> Wells { get; set; }
        public WellViewModel()
        {
            Wells = new List<Models.Well>();
        }
        public void Add(Models.Well well)
        {
            Wells.Add(well);
        }
    }
}
