using HydrodynamicStudies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicStudies.Calculs
{
    /// <summary>
    /// nam tihs laer emos was I
    /// </summary>
    public class RealMagic
    {
        public static PressuresAndTimes GetPressures(WellsList wellsList)
        {
            PressuresAndTimes pressuresAndTimes = Functions.GetTimesAndPressures(wellsList);
            List<double> staticPressures = new List<double>();
            Functions.PrepareStaticPressures(wellsList, staticPressures);
            if (wellsList.Wells[0].Mode == Mode.Reverse)
                pressuresAndTimes.StaticPressures = staticPressures;
            return pressuresAndTimes;
        }

        public static ConsumptionsAndTimes GetConsumptions(WellsList wellsList)
        {
            ConsumptionsAndTimes consumptionsAndTimes = new ConsumptionsAndTimes();
            var consumptions = Functions.GetConsumtions(wellsList);
            List<double> staticConsumptions = new List<double>();
            Functions.PrepareStaticConsumptions(wellsList, staticConsumptions);
            consumptionsAndTimes.Times = Functions.GetTimes(wellsList.Wells, false);
            consumptionsAndTimes.Consumptions = consumptions;
            if (wellsList.Wells[0].Mode == Mode.Direct)
                consumptionsAndTimes.StaticConsumptions = staticConsumptions;
            return consumptionsAndTimes;
        }

        public static QGradientAndConsumptions QGradientMethod(GradientAndWellsList<QGradient> gradientAndWellsList)
        {
            List<Well> gradientWells = new List<Well>();
            foreach (var v in gradientAndWellsList.WellsList.Wells)
                gradientWells.Add(new Well
                {
                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,
                });
            for (int i = 0; i < gradientWells.Count; i++)
            {
                gradientWells[i].K = gradientAndWellsList.Gradient.ChangedK;
                gradientWells[i].Kappa = gradientAndWellsList.Gradient.ChangedKappa;
                gradientWells[i].Ksi = gradientAndWellsList.Gradient.ChangedKsi;
                gradientWells[i].P0 = gradientAndWellsList.Gradient.ChangedP0;
            }
            QGradientAndConsumptions gradientAndConsumptions = new QGradientAndConsumptions() { QGradient = gradientAndWellsList.Gradient };
            Functions.GetNextGradientIteration(gradientAndWellsList, gradientWells, out gradientAndConsumptions);
            if (gradientAndConsumptions.ConsumptionsAndTimes != null)
            {
                List<double> staticConsumptions = new List<double>();
                Functions.PrepareStaticConsumptions(gradientAndWellsList.WellsList, staticConsumptions);
                gradientAndConsumptions.ConsumptionsAndTimes.StaticConsumptions = staticConsumptions;
            }
            return gradientAndConsumptions;
        }

        public static PGradientAndPressures PGradientMethod(GradientAndWellsList<PGradient> gradientAndWellsList)
        {
            List<Well> gradientWells = new List<Well>();
            foreach (var v in gradientAndWellsList.WellsList.Wells)
                gradientWells.Add(new Well
                {
                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                    Mode = v.Mode,
                    CalculatedP = v.CalculatedP,
                    CalculatedQ = v.CalculatedQ,
                });
            for (int i = 0; i < gradientWells.Count; i++)
            {
                gradientWells[i].K = gradientAndWellsList.Gradient.ChangedK;
                gradientWells[i].Kappa = gradientAndWellsList.Gradient.ChangedKappa;
                gradientWells[i].Ksi = gradientAndWellsList.Gradient.ChangedKsi;
                gradientWells[i].P0 = gradientAndWellsList.Gradient.ChangedP0;
            }
            PGradientAndPressures pGradientAndPressures = new PGradientAndPressures() { PGradient = gradientAndWellsList.Gradient };
            Functions.GetNextPGradientIteration(gradientAndWellsList, gradientWells, out pGradientAndPressures);
            if (pGradientAndPressures.PressuresAndTimes != null)
            {
                List<double> staticConsumptions = new List<double>();
                Functions.PrepareStaticPressures(gradientAndWellsList.WellsList, staticConsumptions);
                pGradientAndPressures.PressuresAndTimes.StaticPressures = staticConsumptions;
            }
            return pGradientAndPressures;
        }
    }
}
