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
    public class QGradientViewModel : INotifyPropertyChanged
    {
        private QGradient _selectedGradient;
        public QGradient SelectedGradient
        {
            get { return _selectedGradient; }
            set
            {
                _selectedGradient = value;
                OnPropertyChanged("SelectedGradient");
            }
        }

        private ICommand _nextStepCommand;
        public ICommand NextStep
        {
            get
            {
                if (_nextStepCommand == null)
                {
                    _nextStepCommand = new NextStepCommand(this);
                }
                return _nextStepCommand;
            }
        }

        private ICommand _previousStepCommand;
        public ICommand PreviousStep
        {
            get
            {
                if (_previousStepCommand == null)
                {
                    _previousStepCommand = new PreviousStepCommand(this);
                }
                return _previousStepCommand;
            }
        }

        public List<QGradientAndConsumptions> GradientsAndConsumptions;

        public ObservableCollection<Gradient> Gradients;

        public bool IsFirstTimeGradientClicked;

        public QGradientViewModel()
        {
            Gradients = new ObservableCollection<Gradient>();
            GradientsAndConsumptions = new List<QGradientAndConsumptions>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
