using DisserNET.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DisserNET.Views
{
    /// <summary>
    /// Логика взаимодействия для SurfaceCalc.xaml
    /// </summary>
    public partial class SurfaceCalc : UserControl
    {
        public SurfaceViewModel surfaceViewModel;

        public SurfaceCalc()
        {
            surfaceViewModel = new SurfaceViewModel();
            DataContext = surfaceViewModel;
            InitializeComponent();
        }
    }
}
