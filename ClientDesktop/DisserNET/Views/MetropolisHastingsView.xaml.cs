using DisserNET.ViewModels;
using System.Windows.Controls;

namespace DisserNET.Views
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