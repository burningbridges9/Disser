using ClientDesktop.Commands;
using ClientDesktop.Layouts;
using ClientDesktop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientDesktop.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public PressuresAndTimes PressuresAndTimes { get; set; }
        public ConsumptionsAndTimes ConsumptionsAndTimes { get; set; }
        public WellViewModel WellViewModel { get; set; }
        public PlotViewModel plotViewModel { get; set; }
        public PlotViewModel PlotViewModel
        {
            get { return plotViewModel; }
            set
            {
                plotViewModel = value;
                OnPropertyChanged("PlotViewModel");
            }
        }
        public QGradientViewModel QGradientViewModel { get; set; }
        public PGradientViewModel PGradientViewModel { get; set; }
        public MainViewModel(WellViewModel wellViewModel, QGradientViewModel qGradientViewModel, PGradientViewModel pGradientViewModel)
        {
            this.WellViewModel = wellViewModel;
            this.QGradientViewModel = qGradientViewModel;
            this.PGradientViewModel = pGradientViewModel;
            this.PlotViewModel = new PlotViewModel();
        }

        #region Commands

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

        private ICommand _Surface;
        public ICommand Surface
        {
            get
            {
                return _Surface ?? new SurfaceCommand(this);
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
