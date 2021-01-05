using DisserNET.Models;
using DisserNET.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Linq;
using System;

namespace DisserNET.ViewModels
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

        public WellsList wellsList;


        public ObservableCollection<PGradientAndPressures> PGradientAndPressures;

        public ObservableCollection<PGradient> Gradients;

        public bool IsFirstTimeGradientClicked; 
        private ChartDataRepository chartDataRepository;
        public ChartDataRepository ChartDataRepository
        {
            get => chartDataRepository;
            set
            {
                chartDataRepository = value;
                OnPropertyChanged("ChartDataRepository");
            }
        }

        public PGradientViewModel()
        {
            Gradients = new ObservableCollection<PGradient>();
            PGradientAndPressures = new ObservableCollection<PGradientAndPressures>();
            ChartDataRepository = new ChartDataRepository();
            PGradientAndPressures.CollectionChanged += PGradientAndPressuresChanged;
        }

        private void PGradientAndPressuresChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var grAndPres = sender as IEnumerable<PGradientAndPressures>;
            var lastGrAndPres = grAndPres.LastOrDefault();
            if ((lastGrAndPres is not null) && (lastGrAndPres?.ValuesAndTimes is not null || lastGrAndPres?.Grad is not null))
            {
                SetupPressurePerTimeDatas(lastGrAndPres.ValuesAndTimes);
            }
        }

        public void PressuresCalculated(PGradientAndPressures pGradientAndPressures)
        {
            if (PGradientAndPressures.Count != 0)
                PGradientAndPressures.Clear();

            PGradientAndPressures.Add(pGradientAndPressures);
            Gradients.Add(pGradientAndPressures.Grad);
            SelectedGradient = pGradientAndPressures.Grad; //Gradients.Last(); ??
        }

        public void CleanUp()
        {
            PGradientAndPressures?.Clear();
            Gradients?.Clear();
        }


        public void WellsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<Well> w = (sender as IEnumerable<Well>).ToList();
            wellsList = new WellsList(w);
        }


        internal void SetupPressurePerTimeDatas(PressuresAndTimes pressuresAndTimes)
        {
            switch (wellsList.Wells.Count)
            {
                case 1:
                    ChartDataRepository.Pressures1Times1 = pressuresAndTimes.ToDataPoints(pressuresAndTimes.Pressures1, pressuresAndTimes.Times1);
                    break;
                case 2:
                    ChartDataRepository.Pressures1fTimes1f = pressuresAndTimes.ToDataPoints(pressuresAndTimes.Pressures1f, pressuresAndTimes.Times1f);
                    ChartDataRepository.Pressures1sTimes1s = pressuresAndTimes.ToDataPoints(pressuresAndTimes.Pressures1s, pressuresAndTimes.Times1s);
                    ChartDataRepository.Pressures2Times2 = pressuresAndTimes.ToDataPoints(pressuresAndTimes.Pressures2, pressuresAndTimes.Times2);
                    break;
                case 3:
                    ChartDataRepository.Pressures1fTimes1f = pressuresAndTimes.ToDataPoints(pressuresAndTimes.Pressures1f, pressuresAndTimes.Times1f);
                    ChartDataRepository.Pressures1sTimes1s = pressuresAndTimes.ToDataPoints(pressuresAndTimes.Pressures1s, pressuresAndTimes.Times1s);
                    ChartDataRepository.Pressures2fTimes2f = pressuresAndTimes.ToDataPoints(pressuresAndTimes.Pressures2f, pressuresAndTimes.Times2f);
                    ChartDataRepository.Pressures2sTimes2s = pressuresAndTimes.ToDataPoints(pressuresAndTimes.Pressures2s, pressuresAndTimes.Times2s);
                    ChartDataRepository.Pressures3Times3 = pressuresAndTimes.ToDataPoints(pressuresAndTimes.Pressures3, pressuresAndTimes.Times3);
                    break;
            }
            if (pressuresAndTimes.StaticPressures != null)
            {
                switch (wellsList.Wells.Count)
                {
                    // TO DO : add other cases
                    case 3:
                        ChartDataRepository.StaticPressuresTimes = pressuresAndTimes.ToDataPoints(pressuresAndTimes.StaticPressures, pressuresAndTimes.Times1f.Concat(pressuresAndTimes.Times1s).ToList());
                        break;
                }
            }
        }

        #region Commands
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

        #endregion
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}