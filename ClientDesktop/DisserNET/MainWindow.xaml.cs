using System.Windows;
using DisserNET.ViewModels;

namespace DisserNET
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainViewModel MainViewModel { get; set; }
        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
            MainViewModel = new MainViewModel(Addition.wellViewModel, QGradientClc.gradientViewModel, PGradientClc.gradientViewModel, SurfaceClc.surfaceViewModel);
            this.DataContext = MainViewModel;
        }
    }
}
