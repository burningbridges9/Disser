using DisserNET.Commands;
using DisserNET.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                // add to chart ..
            }
        }

        internal void SetupConsumptionsPerTimeDatas()
        {
            ChartDataRepository.WellViewShowMode = ShowMode.Consumptions;
            ChartDataRepository.ConsumptionsTimes = ConsumptionsAndTimes.ToDataPoints(staticConsumptions: false);
            if (consumptionsAndTimes.StaticConsumptions != null)
            {
                ChartDataRepository.StaticConsumptionsTimes = ConsumptionsAndTimes.ToDataPoints(staticConsumptions: true);
            }
        }

        public WellViewModel()
        {
            Wells = new ObservableCollection<Well>();
            ChartDataRepository = new ChartDataRepository();
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
                return _CalculatePressures ?? new CalculatePressuresCommand(this);
            }
        }

        private ICommand _CalculateConsumptions;
        public ICommand CalculateConsumptions
        {
            get
            {
                return _CalculateConsumptions ?? new CalculateConsumptionsCommand(this);
            }
        }

        private ICommand _Clear;
        public ICommand Clear
        {
            get
            {
                return _Clear ?? new ClearCommand(this);
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
