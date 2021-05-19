using DisserNET.Calculs.Helpers;
using DisserNET.Models;
using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DisserNET.Calculs
{
    public partial class Functions
    {
        private static Object lockObj = new Object();

        private static int _seedCount = 0;
        private static ThreadLocal<Random> _tlRng = new ThreadLocal<Random>(() => new Random(GenerateSeed()));
        private static int GenerateSeed()
        {
            // note the usage of Interlocked, remember that in a shared context we can't just "_seedCount++"
            return (int)((DateTime.Now.Ticks << 4) + (Interlocked.Increment(ref _seedCount)));
        }

        static List<Well> GetWells()
        {
            List<Well> wells = new List<Well>();
            for (int i = 1; i <= 3; i++)
            {
                Well well = new Well
                {
                    Q = 1.0 / (24.0 * 3600.0) * Convert.ToDouble(25) * i,
                    P = Math.Pow(10.0, 6) * Convert.ToDouble(25) * i,
                    P0 = Math.Pow(10.0, 6) * Convert.ToDouble(15),
                    Time1 = 3600.0 * Convert.ToDouble(10) * (i - 1),
                    Time2 = 3600.0 * Convert.ToDouble(10) * i,
                    H0 = Convert.ToDouble(5),
                    Mu = Math.Pow(10.0, -3) * Convert.ToDouble(1), // 1- water, 5 - oil
                    Rw = Convert.ToDouble(0.1),
                    K = Math.Pow(10.0, -15) * Convert.ToDouble(30),
                    Kappa = (1.0 / 3600.0) * Convert.ToDouble(300), // 300- water, 75 - oil
                    Rs = Convert.ToDouble(0.5),
                    Ksi = Convert.ToDouble(5),
                    N = Convert.ToInt32(50),
                };
                wells.Add(well);
            }
            return wells;
        }

        public static async Task<List<AcceptedValueMH>> ParallelMetropolisHastingsAlgorithm(WellsList wellsListCurrent, MetropolisHastings modelMH, int threadsNumber, Mode mode = Mode.Direct)
        {
            #region Old
            var tasks = new List<Task<List<AcceptedValueMH>>>();
            var l = new List<AcceptedValueMH>();
            for (int i = 0; i < threadsNumber; i++)
            {
                tasks.Add(Task<List<AcceptedValueMH>>.Factory.StartNew(
                    () =>
                    {
                        WellsList wellsList = new WellsList(GetWells());
                        List<AcceptedValueMH> list = new List<AcceptedValueMH>();
                        if (mode == Mode.Direct)
                            Functions.MetropolisHastingsAlgorithmForConsumptions(wellsList, modelMH, list, mode);
                        else
                            Functions.MetropolisHastingsAlgorithmForPressures(wellsList, modelMH, list, mode);
                        return list;
                    }, TaskCreationOptions.None));
            }
            await Task.WhenAll(tasks.AsParallel().Select(async task => await task));
            var results = new List<AcceptedValueMH>();
            tasks.ForEach(x => results.AddRange(x.Result));
            return results;
            #endregion

            #region dich
            //MetropolisParallelObject[] metropolisParallelObjects = new MetropolisParallelObject[threadsNumber];
            //System.Random rng = new CryptoRandomSource(threadSafe: true);
            //List<AcceptedValueMH> list = new List<AcceptedValueMH>();
            //Thread[] threads = new Thread[threadsNumber];
            //for (int i = 0; i < threadsNumber; i++)
            //{
            //    metropolisParallelObjects[i] = new MetropolisParallelObject()
            //    {
            //        mode = mode,
            //        ModelMH = modelMH,
            //        WellsListCurrent = new WellsList(GetWells()),
            //        rng = rng,
            //    };
            //    threads[i] = new Thread(new ParameterizedThreadStart(Functions.ParallelMetropolisHastingsAlgorithm));
            //    threads[i].Start(metropolisParallelObjects[i]);
            //}
            //AutoResetEvent.WaitAll(new WaitHandle[] { waitHandler });
            ////for (int i = 0; i < threadsNumber; i++)
            ////    threads[i].Join();
            //foreach (var o in metropolisParallelObjects)
            //{
            //    list.AddRange(o.AcceptedValues);
            //}
            //return list;
            #endregion

        }

        public static void ParallelMetropolisHastingsAlgorithm(object obj)
        {
            MetropolisParallelObject metropolisParallelObject = obj as MetropolisParallelObject;
            WellsList wellsListCurrent = metropolisParallelObject.WellsListCurrent;
            MetropolisHastings modelMH = metropolisParallelObject.ModelMH;
            Mode mode = metropolisParallelObject.mode;
            wellsListCurrent.Wells.ForEach(x => x.Mode = mode);
            List<AcceptedValueMH> acceptedValueMHs = metropolisParallelObject.AcceptedValues;
            if (mode == Mode.Reverse)
                MetropolisHastingsAlgorithmForPressures(wellsListCurrent, modelMH, acceptedValueMHs);
            else
                MetropolisHastingsAlgorithmForConsumptions(wellsListCurrent, modelMH, acceptedValueMHs);
        }

    }
}
