using ClientDesktop.Layouts;
using ClientDesktop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public PressuresAndTimes PressuresAndTimes;
        public ConsumptionsAndTimes ConsumptionsAndTimes;
        public WellViewModel WellViewModel;
        public PlotViewModel plotViewModel;
        public PlotViewModel PlotViewModel
        {
            get { return plotViewModel; }
            set
            {
                plotViewModel = value;
                OnPropertyChanged("PlotViewModel");
            }
        }
        public QGradientViewModel QGradientViewModel;
        public PGradientViewModel PGradientViewModel;
        public MainViewModel(WellViewModel wellViewModel, QGradientViewModel qGradientViewModel, PGradientViewModel pGradientViewModel)
        {
            this.WellViewModel = wellViewModel;
            this.QGradientViewModel = qGradientViewModel;
            this.PGradientViewModel = pGradientViewModel;
            this.PlotViewModel = new PlotViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
