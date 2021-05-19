using DisserNET.Commands;
using DisserNET.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DisserNET.ViewModels
{
    public class WellViewModel : INotifyPropertyChanged
    {
        private PressuresAndTimes pressuresAndTimes;
        public PressuresAndTimes PressuresAndTimes
        {
            get => pressuresAndTimes;
            set
            {
                pressuresAndTimes = value;
                OnPropertyChanged("PressuresAndTimes");
            }
        }

        private ConsumptionsAndTimes consumptionsAndTimes;
        public ConsumptionsAndTimes ConsumptionsAndTimes
        {
            get => consumptionsAndTimes;
            set
            {
                consumptionsAndTimes = value;
                OnPropertyChanged("ConsumptionsAndTimes");
            }
        }

        private Well selectedWell;
        public Well SelectedWell
        {
            get => selectedWell;
            set
            {
                selectedWell = value;
                OnPropertyChanged("SelectedWell");
            }
        }

        public ObservableCollection<Well> Wells { get; set; }

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

        internal void SetupPressurePerTimeDatas()
        {
            ChartDataRepository.WellViewShowMode = ShowMode.Pressures;
            switch (Wells.Count)
            {
                case 1:
                    ChartDataRepository.Pressures1Times1 = PressuresAndTimes.ToDataPoints(PressuresAndTimes.Pressures1, PressuresAndTimes.Times1);
                    break;
                case 2:
                    ChartDataRepository.Pressures1fTimes1f = PressuresAndTimes.ToDataPoints(PressuresAndTimes.Pressures1f, PressuresAndTimes.Times1f);
                    ChartDataRepository.Pressures1sTimes1s = PressuresAndTimes.ToDataPoints(PressuresAndTimes.Pressures1s, PressuresAndTimes.Times1s);
                    ChartDataRepository.Pressures2Times2 = PressuresAndTimes.ToDataPoints(PressuresAndTimes.Pressures2, PressuresAndTimes.Times2);
                    break;
                case 3:
                    ChartDataRepository.Pressures1fTimes1f = PressuresAndTimes.ToDataPoints(PressuresAndTimes.Pressures1f, PressuresAndTimes.Times1f);
                    ChartDataRepository.Pressures1sTimes1s = PressuresAndTimes.ToDataPoints(PressuresAndTimes.Pressures1s, PressuresAndTimes.Times1s);
                    ChartDataRepository.Pressures2fTimes2f = PressuresAndTimes.ToDataPoints(PressuresAndTimes.Pressures2f, PressuresAndTimes.Times2f);
                    ChartDataRepository.Pressures2sTimes2s = PressuresAndTimes.ToDataPoints(PressuresAndTimes.Pressures2s, PressuresAndTimes.Times2s);
                    ChartDataRepository.Pressures3Times3 = PressuresAndTimes.ToDataPoints(PressuresAndTimes.Pressures3, PressuresAndTimes.Times3);
                    break;
            }
            if (pressuresAndTimes.StaticPressures != null)
            {
                switch (Wells.Count) 
                {
                    // TO DO : add other cases
                    case 3:
                        ChartDataRepository.StaticPressuresTimes = PressuresAndTimes.ToDataPoints(PressuresAndTimes.StaticPressures, PressuresAndTimes.Times1f.Concat(PressuresAndTimes.Times1s).ToList());
                        break;
                }
            }
        }

        internal void SetupConsumptionsPerTimeDatas()
        {
            ChartDataRepository.WellViewShowMode = ShowMode.Consumptions;
            ChartDataRepository.ConsumptionsTimes = ConsumptionsAndTimes.ToDataPoints(staticConsumptions: false);
            if (consumptionsAndTimes.StaticConsumptions != null)
            {
                //ChartDataRepository.StaticConsumptionsTimes = ConsumptionsAndTimes.ToDataPoints(staticConsumptions: true);
            }
        }

        public WellViewModel()
        {
            Wells = new ObservableCollection<Well>();
            ChartDataRepository = new ChartDataRepository();
            CalculatePressures = new CalculatePressuresCommand(this);
            CalculateConsumptions = new CalculateConsumptionsCommand(this);
            Clear = new ClearCommand(this);
        }

        #region Commands
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

        private ICommand _addAutoWellCommand;
        public ICommand AddAuto
        {
            get
            {
                if (_addAutoWellCommand == null)
                {
                    _addAutoWellCommand = new AddAutoWellCommand(this);
                }
                return _addAutoWellCommand;
            }
        }

        private ICommand _CalculatePressures;
        public ICommand CalculatePressures
        {
            get
            {
                return _CalculatePressures;
            }
            private set
            {
                _CalculatePressures = value;
            }
        }

        private ICommand _CalculateConsumptions;
        public ICommand CalculateConsumptions
        {
            get
            {
                return _CalculateConsumptions;
            }
            private set
            {
                _CalculateConsumptions = value;
            }
        }

        private ICommand _Clear;
        public ICommand Clear
        {
            get
            {
                return _Clear;
            }
            private set
            {
                _Clear = value;
            }
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
