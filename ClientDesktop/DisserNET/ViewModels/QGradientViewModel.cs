using DisserNET.Commands;
using DisserNET.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OxyPlot;
using System.Collections.Specialized;

namespace DisserNET.ViewModels
{
    public class QGradientViewModel : INotifyPropertyChanged
    {
        private QGradient _selectedGradient;
        public QGradient SelectedGradient
        {
            get  => _selectedGradient; 
            set
            {
                _selectedGradient = value;
                OnPropertyChanged("SelectedGradient");
            }
        }

        #region Commands
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

        private ICommand _saveQCommand;
        public ICommand QSave
        {
            get
            {
                if (_saveQCommand == null)
                {
                    _saveQCommand = new SaveQCommand(this);
                }
                return _saveQCommand;
            }
        }
        #endregion

        public ObservableCollection<QGradientAndConsumptions> GradientsAndConsumptions;


        public IList<DataPoint> activeConsumptions;
        public IList<DataPoint> ActiveConsumptions
        {
            get => activeConsumptions;
            set
            {
                activeConsumptions = value;
                OnPropertyChanged("ActiveConsumptions");
            }
        }

        public IList<DataPoint> staticConsumptions;
        public IList<DataPoint> StaticConsumptions
        {
            get => staticConsumptions;
            set
            {
                staticConsumptions = value;
                OnPropertyChanged("StaticConsumptions");
            }
        }

        public ObservableCollection<QGradient> Gradients;

        public WellsList wellsList;

        public bool IsFirstTimeGradientClicked;

        public QGradientViewModel()
        {
            Gradients = new ObservableCollection<QGradient>();
            GradientsAndConsumptions = new ObservableCollection<QGradientAndConsumptions>();

            GradientsAndConsumptions.CollectionChanged += GradientAndConsumptionsChanged;
        }

        private void GradientAndConsumptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var grAndCons = sender as IEnumerable<QGradientAndConsumptions>;
            var lastGrAndCons = grAndCons.LastOrDefault();
            if ((lastGrAndCons is not null) && (lastGrAndCons?.ValuesAndTimes is not null || lastGrAndCons?.Grad is not null))
            {
                ActiveConsumptions = lastGrAndCons?.ValuesAndTimes.ToDataPoints(false);
                StaticConsumptions = lastGrAndCons?.ValuesAndTimes.ToDataPoints(true);
            }
        }

        // TO DO : refactor this
        public void ConsumptionsCalculated(QGradientAndConsumptions qGradientAndConsumptions)
        {
            if (GradientsAndConsumptions.Count != 0)
                GradientsAndConsumptions.Clear();

            GradientsAndConsumptions.Add(qGradientAndConsumptions);
            Gradients.Add(qGradientAndConsumptions.Grad);
            SelectedGradient = Gradients.Last();
        }

        public void CleanUp()
        {
            GradientsAndConsumptions?.Clear();
            Gradients?.Clear();
            ActiveConsumptions?.Clear();
            StaticConsumptions?.Clear();
        }
        public void WellsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<Well> w = (sender as IEnumerable<Well>).ToList();
            wellsList = new WellsList(w);
        }

        #region Property changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
