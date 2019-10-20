using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.ViewModels
{
    public class PlotViewModel : INotifyPropertyChanged
    {
        //public PlotViewModel()
        //{
        //    Title = "O";
        //    PressureTime1 = new List<DataPoint>();
        //    PressureTime1f = new List<DataPoint>();
        //    PressureTime1s = new List<DataPoint>();
        //    PressureTime2 = new List<DataPoint>();
        //    PressureTime2f = new List<DataPoint>();
        //    PressureTime2s = new List<DataPoint>();
        //    PressureTime3 = new List<DataPoint>();
        //}
        //private string title;
        //public string Title
        //{
        //    get { return title; }
        //    set
        //    {
        //        title = value;
        //        OnPropertyChanged("Title");
        //    }
        //}

        //private IList<DataPoint> pressuretime1;      
        //public IList<DataPoint> PressureTime1
        //{
        //    get { return pressuretime1; }
        //    set
        //    {
        //        pressuretime1 = value;
        //        OnPropertyChanged("PressureTime1");
        //    }
        //}

        //private IList<DataPoint> pressuretime1f;
        //public IList<DataPoint> PressureTime1f
        //{
        //    get { return pressuretime1f; }
        //    set
        //    {
        //        pressuretime1f = value;
        //        OnPropertyChanged("PressureTime1f");
        //    }
        //}

        //private IList<DataPoint> pressuretime1s;
        //public IList<DataPoint> PressureTime1s
        //{
        //    get { return pressuretime1s; }
        //    set
        //    {
        //        pressuretime1s = value;
        //        OnPropertyChanged("PressureTime1s");
        //    }
        //}

        //private IList<DataPoint> pressuretime2;
        //public IList<DataPoint> PressureTime2
        //{
        //    get { return pressuretime2; }
        //    set
        //    {
        //        pressuretime2 = value;
        //        OnPropertyChanged("PressureTime2");
        //    }
        //}

        //private IList<DataPoint> pressuretime2f;
        //public IList<DataPoint> PressureTime2f
        //{
        //    get { return pressuretime2f; }
        //    set
        //    {
        //        pressuretime2f = value;
        //        OnPropertyChanged("PressureTime2f");
        //    }
        //}

        //private IList<DataPoint> pressuretime2s;
        //public IList<DataPoint> PressureTime2s
        //{
        //    get { return pressuretime2s; }
        //    set
        //    {
        //        pressuretime2s = value;
        //        OnPropertyChanged("PressureTime2s");
        //    }
        //}

        //private IList<DataPoint> pressuretime3;
        //public IList<DataPoint> PressureTime3
        //{
        //    get { return pressuretime3; }
        //    set
        //    {
        //        pressuretime3 = value;
        //        OnPropertyChanged("PressureTime3");
        //    }
        //}


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public PlotViewModel()
        {
            this.MyModel = new PlotModel();
            var s1 = new LineSeries
            {
                Color = OxyColors.SkyBlue,
                MarkerType = MarkerType.None,
                //MarkerSize = 3,
                //MarkerStroke = OxyColors.White,
                //MarkerFill = OxyColors.SkyBlue,
                MarkerStrokeThickness = 1.5
            };
            var s3 = new LineSeries
            {
                Color = OxyColors.Black,
                //MarkerType = MarkerType.Cross,
                //MarkerSize = 3,
                //MarkerStroke = OxyColors.White,
                //MarkerFill = OxyColors.SkyBlue,
                MarkerStrokeThickness = 1.5
            };
            var s2 = new LineSeries
            {
                Color = OxyColors.AliceBlue,
                MarkerType = MarkerType.Cross,
                MarkerSize = 1.5,
                MarkerStroke = OxyColors.White,
                MarkerFill = OxyColors.AliceBlue,
                MarkerStrokeThickness = 1.5
            };
            
            MyModel.Series.Add(s1);

            MyModel.Series.Add(s2);
            MyModel.Series.Add(s3);
            MyModel.InvalidatePlot(true);
        }
        private PlotModel myModel { get; set; }
        public PlotModel MyModel
        {
            get { return myModel; }
            set
            {
                myModel = value;
                OnPropertyChanged("MyModel");
            }
        }

    }
}
