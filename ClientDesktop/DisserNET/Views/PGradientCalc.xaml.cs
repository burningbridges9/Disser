using DisserNET.ViewModels;
using System.Windows.Controls;

namespace DisserNET.Views
{
    /// <summary>
    /// Interaction logic for GradientCalc.xaml
    /// </summary>
    public partial class PGradientCalc : UserControl
    {
        public PGradientViewModel gradientViewModel;
        public PGradientCalc()
        {
            gradientViewModel = new PGradientViewModel();
            DataContext = gradientViewModel;
            InitializeComponent();
            gradientList.ItemsSource = gradientViewModel.Gradients;
        }

    }
}
