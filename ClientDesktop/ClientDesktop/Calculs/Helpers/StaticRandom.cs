using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace HydrodynamicStudies.Calculs.Helpers
{
    public static class StaticRandom
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new SystemRandomSource(Interlocked.Increment(ref seed)));

        public static double Rand()
        {
            return random.Value.NextDouble();
        }
    }
}
