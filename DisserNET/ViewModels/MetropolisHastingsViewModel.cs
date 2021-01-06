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
    public class MetropolisHastingsViewModel : INotifyPropertyChanged
    {
        private MetropolisHastings metropolisHastings;
        public MetropolisHastings MetropolisHastings { get => metropolisHastings; set { metropolisHastings = value; OnPropertyChanged(prop: "MetropolisHastings"); } }
        private ObservableCollection<AcceptedValueMH> acceptedValues;
        public ObservableCollection<AcceptedValueMH> AcceptedValues { get => acceptedValues; set { acceptedValues = value; OnPropertyChanged(prop: "AcceptedValues"); } }

        public readonly ReportDb reportDb;
        public WellsList WellsList { get; private set; }
        public Mode Mode => WellsList?.Wells?.FirstOrDefault()?.Mode ?? 0;
        public MetropolisHastingsViewModel(ReportDb reportDb)
        {
            this.reportDb = reportDb;
            AcceptedValues = new ObservableCollection<AcceptedValueMH>();
        }

        public void WellsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<Well> w = (sender as IEnumerable<Well>).ToList();
            WellsList = new WellsList(w);
        }

        public void Save() => reportDb.WriteMHInfo(MetropolisHastings, AcceptedValues.ToList());

        #region Commands and Prop changed
        private ICommand _addMHCommand;
        public ICommand Add
        {
            get
            {
                if (_addMHCommand == null)
                {
                    _addMHCommand = new AddMHCommand(this);
                }
                return _addMHCommand;
            }
        }

        private ICommand _startMHCommand;

        public ICommand Start
        {
            get
            {
                if (_startMHCommand == null)
                {
                    _startMHCommand = new StartMHCommand(this);
                }
                return _startMHCommand;
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
