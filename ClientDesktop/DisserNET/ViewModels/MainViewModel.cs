using DisserNET.Commands;
using DisserNET.Views;
using DisserNET.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Specialized;

namespace DisserNET.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public WellViewModel WellViewModel { get; set; }
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
        public QGradientViewModel QGradientViewModel { get; set; }
        public PGradientViewModel PGradientViewModel { get; set; }
        public SurfaceViewModel SurfaceViewModel { get; set; }
        public MainViewModel(WellViewModel wellViewModel, QGradientViewModel qGradientViewModel, PGradientViewModel pGradientViewModel, SurfaceViewModel surfaceViewModel)
        {
            this.WellViewModel = wellViewModel;
            this.QGradientViewModel = qGradientViewModel;
            this.PGradientViewModel = pGradientViewModel;
            this.PlotViewModel = new PlotViewModel();
            PlotViewModel.wellViewModel = wellViewModel;
            this.SurfaceViewModel = surfaceViewModel;

            BindPostActionsToCommands();
        }

        private void BindPostActionsToCommands()
        {
            this.WellViewModel.Wells.CollectionChanged += this.PGradientViewModel.WellsChanged;
            (this.WellViewModel.CalculatePressures as CalculatePressuresCommand).NotifyExecuted += this.PGradientViewModel.PressuresCalculated;
            (this.WellViewModel.Clear as ClearCommand).NotifyExecuted += this.PGradientViewModel.CleanUp;

            this.WellViewModel.Wells.CollectionChanged += this.QGradientViewModel.WellsChanged;
            (this.WellViewModel.CalculateConsumptions as CalculateConsumptionsCommand).NotifyExecuted += this.QGradientViewModel.ConsumptionsCalculated;
            (this.WellViewModel.Clear as ClearCommand).NotifyExecuted += this.QGradientViewModel.CleanUp;
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
