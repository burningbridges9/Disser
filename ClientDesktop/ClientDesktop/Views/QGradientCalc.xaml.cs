﻿using HydrodynamicStudies.Models;
using HydrodynamicStudies.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace HydrodynamicStudies.Views
{
    /// <summary>
    /// Interaction logic for GradientCalc.xaml
    /// </summary>
    public partial class QGradientCalc : UserControl
    {
        public QGradientViewModel gradientViewModel;
        public QGradientCalc()
        {
            gradientViewModel = new QGradientViewModel();
            DataContext = gradientViewModel;
            InitializeComponent();
            gradientList.ItemsSource = gradientViewModel.Gradients;
        }

    }
}
