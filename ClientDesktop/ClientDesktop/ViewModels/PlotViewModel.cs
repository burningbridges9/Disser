using ClientDesktop.Models;
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

        public void PlotTimeConsumptions(ConsumptionsAndTimes consumptionsAndTimes)
        {
            MyModel.Series.Clear();
            MyModel.InvalidatePlot(true);
            var model = new PlotModel { LegendSymbolLength = 24 };
            model.Series.Add(new LineSeries
            {
                Color = OxyColors.SkyBlue,
                MarkerType = MarkerType.None,
                MarkerStrokeThickness = 1.5
            });
            foreach (var pt in consumptionsAndTimes.Consumptions.Zip(consumptionsAndTimes.Times, Tuple.Create))
            {
                (model.Series[0] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * 24.0 * 3600.0));
            }
            if (consumptionsAndTimes.StaticConsumptions != null)
            {
                model.Series.Add(new LineSeries
                {
                    Color = OxyColors.Red,
                    MarkerType = MarkerType.None,
                    MarkerStrokeThickness = 1.5
                });
                foreach (var pt in consumptionsAndTimes.StaticConsumptions.Zip(consumptionsAndTimes.Times, Tuple.Create))
                {
                    (model.Series[1] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * 24.0 * 3600.0));
                }
            }
            MyModel = model;
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
