using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Models
{
    public class ReportModel : INotifyPropertyChanged
    {
        private MetropolisHastings metropolisHastings;
        private string folderPath;
        private string comment;
        private AcceptedValueMH acceptedValueMH;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public MetropolisHastings MetropolisHastings { get => metropolisHastings; set { metropolisHastings = value; OnPropertyChanged("MetropolisHastings"); } }
        public AcceptedValueMH AcceptedValueMH { get => acceptedValueMH; set { acceptedValueMH = value; OnPropertyChanged("AcceptedValueMH"); } }
        public string FolderPath { get => folderPath; set { folderPath = value; OnPropertyChanged("FolderPath"); } }
        public string Comment { get => comment; set { comment = value; OnPropertyChanged("Comment"); } }

        public ReportModel(string pathToParams, string pathToResults, string pathToComment)
        {
            using (StreamReader srp = new StreamReader(pathToParams))
            using (StreamReader srr = new StreamReader(pathToResults))
            {
                var jsonP = srp.ReadToEnd();
                var jsonR = srr.ReadToEnd();
                MetropolisHastings = JsonConvert.DeserializeObject<MetropolisHastings>(jsonP);
                AcceptedValueMH = JsonConvert.DeserializeObject<AcceptedValueMH>(jsonR);
            }
            if (pathToComment == null || !File.Exists(pathToComment))
            {
                Comment = string.Empty;
            }
            else
            {
                using var sr = new StreamReader(pathToComment);
                Comment = sr.ReadToEnd();
            }
        }
    }
}
