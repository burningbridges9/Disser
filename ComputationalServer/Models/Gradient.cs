using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputationalServer.Models
{
    public class Gradient
    {
        public double ChangedK     {get;set;}
        public double ChangedKappa {get;set;}
        public double ChangedKsi   {get;set;}
        public double Lambda      {get;set;}
        public double DeltaK       {get;set;}
        public double DeltaKappa   {get;set;}
        public double DeltaKsi     {get;set;}
        public double F            {get;set;}
    }
}
