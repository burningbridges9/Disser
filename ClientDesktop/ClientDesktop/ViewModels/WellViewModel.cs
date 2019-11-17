using ClientDesktop.Commands;
using ClientDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientDesktop.ViewModels
{
    public class WellViewModel :INotifyPropertyChanged
    {
        private Well _selectedWell;

        private ICommand _addWellCommand;
        public ICommand Add
        {
            get
            {
                if (_addWellCommand == null)
                {
                    _addWellCommand = new AddWellCommand(this);
                }
                return _addWellCommand;
            }
        }

        private ICommand _removeLastWellCommand;
        public ICommand RemoveLastWellCommand
        {
            get
            {
                if (_removeLastWellCommand == null)
                {
                    _removeLastWellCommand = new RemoveLastWellCommand(this);
                }
                return _removeLastWellCommand;
            }
        }

        private ICommand _deleteAllWellCommand;
        public ICommand DeleteAllWellCommand
        {
            get
            {
                if (_deleteAllWellCommand == null)
                {
                    _deleteAllWellCommand = new DeleteAllWellCommand(this);
                }
                return _deleteAllWellCommand;
            }
        }

        public List<Well> Wells { get; set; }
        public Well SelectedWell
        {
            get { return _selectedWell; }
            set
            {
                _selectedWell = value;
                OnPropertyChanged("SelectedWell");
            }
        }

        public WellViewModel()
        {
            Wells = new List<Well>();
        }
        //public void Add(Models.Well well)
        //{
        //    Wells.Add(well);
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
