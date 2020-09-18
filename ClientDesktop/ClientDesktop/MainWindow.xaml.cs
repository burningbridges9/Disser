using OxyPlot;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClientDesktop.ViewModels;
using ClientDesktop.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Windows.Media;
using ClientDesktop.Layouts;
using System.IO;

namespace ClientDesktop
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
