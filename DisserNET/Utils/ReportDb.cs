using Microsoft.Extensions.Configuration;
using System;
using DisserNET.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DisserNET.Utils
{
    public class ReportDb
    {
        private readonly string dailyFolderFormat = "report_{0:yyyy'-'MM'-'dd}";
        private readonly string experimentFolderFormat =
            "exp_{0:yyyy'-'MM'-'dd'-'mm};k[{1}-{2}-{3}-{4}],kap[{5}-{6}-{7}-{8}],ksi[{9}-{10}-{11}-{12}],p0[{13}-{14}-{15}-{16}],WC[{17}],S_0[{18}],M[{19}],AL[{20}]";
        private readonly string root;
        private readonly string mhParamsNameAndExt = "MH_params.json";
        private readonly string mhAcceptedNameAndExt = "MH_results.json";
        private readonly string KValsNameAndExt = "K.txt";
        private readonly string KappaValsNameAndExt = "Kappa.txt";
        private readonly string KsiValsNameAndExt = "Ksi.txt";
        private readonly string P0ValsNameAndExt = "P0.txt";
        private readonly string FminValsNameAndExt = "Fmin.txt";

        public ReportDb(IConfiguration configuration)
        {
            root = configuration.GetValue<string>("AppSettings:MHReportsPath")?.ToString() ?? "C:\\Users\\Rustam\\Desktopfsfsfs\\Master\\MHReports";
        }

        public void WriteMHInfo(MetropolisHastings mh, List<AcceptedValueMH> acceptedValues)
        {
            var dir = Path.Combine(root, string.Format(dailyFolderFormat, DateTime.Now));
            CheckCreated(dir);

            var expFolderName = string.Format(experimentFolderFormat, DateTime.Now,
                mh.MinK, mh.MaxK, mh.StepK, mh.IncludedK,
                mh.MinKappa, mh.MaxKappa, mh.StepKappa, mh.IncludedKappa,
                mh.MinKsi, mh.MaxKsi, mh.StepKsi, mh.IncludedKsi,
                mh.MinP0, mh.MaxP0, mh.StepP0, mh.IncludedP0,
                mh.WalksCount, mh.S_0, mh.Mode, mh.SelectLogic);

            var expFolderDir = Path.Combine(dir, expFolderName);
            CheckCreated(expFolderDir);
            WrapAndWriteJson<MetropolisHastings>(mh, expFolderDir, mhParamsNameAndExt);
            WrapAndWriteJson<AcceptedValueMH>(acceptedValues.LastOrDefault(), expFolderDir, mhAcceptedNameAndExt);

            if (mh.IncludedK)
                WriteValues(acceptedValues.Select(a => a.K).ToList(), Math.Pow(10.0, 15), expFolderDir, KValsNameAndExt);
            if (mh.IncludedKappa)
                WriteValues(acceptedValues.Select(a => a.Kappa).ToList(), 3600.0, expFolderDir, KappaValsNameAndExt);
            if (mh.IncludedKsi)
                WriteValues(acceptedValues.Select(a => a.Ksi).ToList(), 1, expFolderDir, KsiValsNameAndExt);
            if (mh.IncludedP0)
                WriteValues(acceptedValues.Select(a => a.P0).ToList(), Math.Pow(10.0, -6), expFolderDir, P0ValsNameAndExt);
            WriteValues(acceptedValues.Select(a => a.Fmin).ToList(), 1, expFolderDir, FminValsNameAndExt);
        }

        private void WrapAndWriteJson<T>(T mh, string dir, string nameAndExt) where T : class
        {
            var json = JsonConvert.SerializeObject(mh, Formatting.Indented);
            using StreamWriter sw = new StreamWriter(Path.Combine(dir, nameAndExt), false, Encoding.Default);
            sw.Write(json);
        }

        private void WriteValues(List<double> v, double c, string dir, string nameAndExt)
        {
            using StreamWriter sw = new StreamWriter(Path.Combine(dir, nameAndExt), false, Encoding.Default);
            v.ForEach(val => sw.Write(val * c + " "));
        }



        void CheckCreated(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
