using HydrodynamicStudies.ViewModels;
using System.Windows.Controls;

namespace HydrodynamicStudies.Views
{
    /// <summary>
    /// Interaction logic for MetropolisHastingsView.xaml
    /// </summary>
    public partial class MetropolisHastingsView : UserControl
    {
        public MetropolisHastingsViewModel metropolisHastingsViewModel;
        public MetropolisHastingsView()
        {
            metropolisHastingsViewModel = new MetropolisHastingsViewModel();
            DataContext = metropolisHastingsViewModel;
            InitializeComponent();
        }
    }
}