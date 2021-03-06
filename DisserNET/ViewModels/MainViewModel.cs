﻿using DisserNET.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DisserNET.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public WellViewModel WellViewModel { get; set; }
        public QGradientViewModel QGradientViewModel { get; set; }
        public PGradientViewModel PGradientViewModel { get; set; }
        public SurfaceViewModel SurfaceViewModel { get; set; }
        public MetropolisHastingsViewModel MetropolisHastingsViewModel { get; }
        public ReportViewModel ReportViewModel { get; }

        public MainViewModel(WellViewModel wellViewModel, QGradientViewModel qGradientViewModel, 
            PGradientViewModel pGradientViewModel, SurfaceViewModel surfaceViewModel,
            MetropolisHastingsViewModel metropolisHastingsViewModel, ReportViewModel reportViewModel)
        {
            this.WellViewModel = wellViewModel;
            this.QGradientViewModel = qGradientViewModel;
            this.PGradientViewModel = pGradientViewModel;
            this.SurfaceViewModel = surfaceViewModel;
            this.MetropolisHastingsViewModel = metropolisHastingsViewModel;
            this.ReportViewModel = reportViewModel;
            BindPostActionsToCommands();
        }

        private void BindPostActionsToCommands()
        {
            this.WellViewModel.Wells.CollectionChanged += this.PGradientViewModel.WellsChanged;
            (this.WellViewModel.CalculatePressures as CalculatePressuresCommand).CommandExecuted += this.PGradientViewModel.PressuresCalculated;
            (this.WellViewModel.Clear as ClearCommand).CommandExecuted += this.PGradientViewModel.CleanUp;

            this.WellViewModel.Wells.CollectionChanged += this.QGradientViewModel.WellsChanged;
            (this.WellViewModel.CalculateConsumptions as CalculateConsumptionsCommand).CommandExecuted += this.QGradientViewModel.ConsumptionsCalculated;
            (this.WellViewModel.Clear as ClearCommand).CommandExecuted += this.QGradientViewModel.CleanUp;

            this.WellViewModel.Wells.CollectionChanged += this.MetropolisHastingsViewModel.WellsChanged;
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
