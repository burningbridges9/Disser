using System.Windows;
using DisserNET.ViewModels;

namespace DisserNET
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly MainViewModel MainViewModel;

        public static MainViewModel MainViewModell;
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            this.MainViewModel = mainViewModel;//new MainViewModel(Addition.wellViewModel, QGradientClc.gradientViewModel, PGradientClc.gradientViewModel, 
            //SurfaceClc.surfaceViewModel);
            this.DataContext = mainViewModel;
        }
    }
}
