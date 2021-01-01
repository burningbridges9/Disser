using DisserNET.Commands;
using DisserNET.Models;
using System;
using System.Collections.Generic;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


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
    }
}
