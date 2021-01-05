using DisserNET.Calculs;
using DisserNET.Models;
using DisserNET.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Commands
{
    public class StartMHCommand : MetropolisHastingsViewCommand
    {
        public StartMHCommand(MetropolisHastingsViewModel vm) : base(vm)
        {

        }

        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            //var wl = new WellsList(MainWindow.MainViewModell.WellViewModel.Wells.ToList());
            //var mode = wl.Wells.First().Mode;

            //var result = Functions.MetropolisHastingsAlgorithmForConsumptions(wl, metropolisHastingsViewModel.MetropolisHastings, mode);
            //WriteToFile(result.ToList(), 2);
        }


        static void WriteToFile(List<AcceptedValueMH> accepteds, int values)
        {
            var writePath1 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\K_Q1.txt";
            var writePath2 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\Kappa_Q1.txt";
            var writePathProb = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\Probability_Q1.txt";
            var writePathObj = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Metropolis\Acc1.txt";
            switch (values)
            {
                case 1:
                    using (StreamWriter sw1 = new StreamWriter(writePath1, false, Encoding.Default))
                    using (StreamWriter sw2 = new StreamWriter(writePathProb, false, Encoding.Default))
                    {
                        foreach (var a in accepteds)
                        {
                            sw1.Write(a.K * Math.Pow(10.0, 15) + " ");
                            sw2.Write(a.ProbabilityDensity + " ");
                        }
                    }
                    break;
                case 2:
                    using (StreamWriter sw1 = new StreamWriter(writePath1, false, Encoding.Default))
                    using (StreamWriter sw2 = new StreamWriter(writePathProb, false, Encoding.Default))
                    using (StreamWriter sw3 = new StreamWriter(writePath2, false, Encoding.Default))
                    {
                        foreach (var a in accepteds)
                        {
                            sw1.Write(a.K * Math.Pow(10.0, 15) + " ");
                            sw2.Write(a.Fmin + " ");
                            sw3.Write(a.Kappa * 3600.0 + " ");
                        }
                    }

                    var json = JsonConvert.SerializeObject(accepteds, Formatting.Indented);
                    using (StreamWriter sw = new StreamWriter(writePathObj, false, Encoding.Default))
                    {
                        sw.Write(json);
                    }
                    break;
                default:
                    break;
            }

        }
    }
}
