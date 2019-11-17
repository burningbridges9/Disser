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
        public static WellViewModel wellViewModel;
        public PressureCalcLayout()
        {
            wellViewModel = new WellViewModel();
            DataContext = wellViewModel;
            InitializeComponent();
        }

        private void SubmitAdd_Click(object sender, RoutedEventArgs e)
        {
            Well well = new Well
            {
                Q = 1.0 / (24.0 * 3600.0) * Convert.ToDouble(TextBoxQ.Text),
                P = Math.Pow(10.0, 6) * Convert.ToDouble(TextBoxP.Text),
                P0 = Math.Pow(10.0, 6) * Convert.ToDouble(TextBoxP0.Text),
                Time1 = 3600.0 * Convert.ToDouble(TextBoxT1.Text),
                Time2 = 3600.0 * Convert.ToDouble(TextBoxT2.Text),
                H0 = Convert.ToDouble(TextBoxH0.Text),
                Mu = Math.Pow(10.0, -3) * Convert.ToDouble(TextBoxMu.Text),
                Rw = Convert.ToDouble(TextBoxRw.Text),
                K = Math.Pow(10.0, -15) * Convert.ToDouble(TextBoxK.Text),
                Kappa = (1.0 / 3600.0) * Convert.ToDouble(TextBoxKappa.Text),
                Rs = Convert.ToDouble(TextBoxRs.Text),
                Ksi = Convert.ToDouble(TextBoxKsi.Text),
                N = Convert.ToInt32(TextBoxN.Text),
            };
            TextBoxQ.Text = (Convert.ToDouble(TextBoxQ.Text) + 5).ToString();
            TextBoxP.Text = (Convert.ToDouble(TextBoxP.Text) + 5).ToString();
            //TextBoxP0.Text = "";
            TextBoxT1.Text = (Convert.ToDouble(TextBoxT1.Text) + 5).ToString();
            TextBoxT2.Text = (Convert.ToDouble(TextBoxT2.Text) + 5).ToString();  
            //MainWindow.wellViewModel.Add(well);
        }

        private void SubmitDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.wellViewModel.Wells.Clear();
        }
    }
}