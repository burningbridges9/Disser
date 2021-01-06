using System.Windows;
using DisserNET.ViewModels;

namespace DisserNET
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            this.DataContext = mainViewModel;
            
            // stupid but works, some day will be binded in usual way in view ... TO DO
            this.QGradientClc.gradientList.ItemsSource = mainViewModel.QGradientViewModel.Gradients;
            this.PGradientClc.gradientList.ItemsSource = mainViewModel.PGradientViewModel.Gradients;
            this.MHView.acceptedList.ItemsSource = mainViewModel.MetropolisHastingsViewModel.AcceptedValues;
        }
    }
}
