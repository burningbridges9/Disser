using ClientDesktop.Commands;
using ClientDesktop.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ClientDesktop.ViewModels
{
    public class PGradientViewModel : INotifyPropertyChanged
    {
        private PGradient _selectedGradient;
        public PGradient SelectedGradient
        {
            get { return _selectedGradient; }
            set
            {
                _selectedGradient = value;
                OnPropertyChanged("SelectedGradient");
            }
        }

        private ICommand _nextStepPCommand;
        public ICommand NextStep
        {
            get
            {
                if (_nextStepPCommand == null)
                {
                    _nextStepPCommand = new NextStepPCommand(this);
                }
                return _nextStepPCommand;
            }
        }

        private ICommand _previousStepPCommand;
        public ICommand PreviousStep
        {
            get
            {
                if (_previousStepPCommand == null)
                {
                    _previousStepPCommand = new PreviousStepPCommand(this);
                }
                return _previousStepPCommand;
            }
        }

        private ICommand _savePCommand;
        public ICommand PSave
        {
            get
            {
                if (_savePCommand == null)
                {
                    _savePCommand = new SavePCommand(this);
                }
                return _savePCommand;
            }
        }

        public List<PGradientAndPressures> PGradientAndPressures;

        public ObservableCollection<Gradient> Gradients;

        public bool IsFirstTimeGradientClicked;

        public PGradientViewModel()
        {
            Gradients = new ObservableCollection<Gradient>();
            PGradientAndPressures = new List<PGradientAndPressures>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}