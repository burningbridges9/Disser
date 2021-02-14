using DisserNET.Commands;
using DisserNET.Models;
using DisserNET.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DisserNET.ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        private ReportModel reportModel;
        public ReportModel ReportModel { get => reportModel; set { reportModel = value; OnPropertyChanged(prop: "ReportModel"); } }
        
        public readonly ReportDb reportDb;
        public string BaseReportDir => reportDb.BaseDir;
        public ReportViewModel(ReportDb reportDb)
        {
            this.reportDb = reportDb;
        }


        //public void Save() => reportDb.WriteMHInfo(MetropolisHastings, AcceptedValues.ToList());

        #region Commands and Prop changed
        private ICommand _selectFolder;
        public ICommand SelectFolder
        {
            get
            {
                if (_selectFolder == null)
                {
                    _selectFolder = new SelectFolderCommand(this);
                }
                return _selectFolder;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
