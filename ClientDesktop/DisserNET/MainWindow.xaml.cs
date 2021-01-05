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
            this.QGradientClc.gradientList.ItemsSource = mainViewModel.QGradientViewModel.Gradients; // stupid but works, some day will be binded in view ... TO DO
        }
    }
}
