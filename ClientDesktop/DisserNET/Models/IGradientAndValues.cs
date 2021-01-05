using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Models
{
    public interface IGradientAndValues<TGrad, TValues> 
        where TGrad : Gradient 
        where TValues : ValuesAndTimes
    {
        TGrad Grad { get; set; }
        TValues ValuesAndTimes { get; set; }
    }
}
