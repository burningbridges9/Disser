using ClientDesktop.Models;
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

namespace ClientDesktop.Layouts
{
    /// <summary>
    /// Interaction logic for GradientCalc.xaml
    /// </summary>
    public partial class GradientCalc : UserControl
    {
        
        public Gradient GradientToShow { get; set; }
        //{
        //    get
        //    {
        //        return _GradientToShow;
        //    }

        //    set
        //    {
        //        if (value == _GradientToShow)
        //            return;

        //        _GradientToShow = value;
        //        OnPropertyChanged("GradientToShow");
        //    }

        //}
        //private Gradient _GradientToShow;
        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public GradientCalc()
        {
            InitializeComponent();
            //DataContext = GradientToShow;
        }

    }
}
