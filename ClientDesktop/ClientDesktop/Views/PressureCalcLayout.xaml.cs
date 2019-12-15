using ClientDesktop.Models;
using ClientDesktop.Utils;
using ClientDesktop.ViewModels;
using OxyPlot;
using OxyPlot.Series;
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

namespace ClientDesktop.Layouts
{
    /// <summary>
    /// Interaction logic for PressureCalcLayout.xaml
    /// </summary>
    public partial class PressureCalcLayout : UserControl
    {
        public WellViewModel wellViewModel;
        public PressureCalcLayout()
        {
            wellViewModel = new WellViewModel();
            DataContext = wellViewModel;
            InitializeComponent();
            wellsList.ItemsSource = wellViewModel.Wells;
        }
    }
}