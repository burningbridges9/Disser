using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Models
{
    public class ChartDataRepository : INotifyPropertyChanged
    {
        #region Fields
        private List<DataPoint> pressures1Times1;
        private List<DataPoint> pressures1fTimes1f;
        private List<DataPoint> pressures1sTimes1s;
        private List<DataPoint> pressures2Times2;
        private List<DataPoint> pressures2fTimes2f;
        private List<DataPoint> pressures2sTimes2s;
        private List<DataPoint> pressures3Times3;
        private List<DataPoint> staticPressuresTimes;
        private List<DataPoint> consumptionsTimes;
        private List<DataPoint> staticConsumptionsTimes;
        private string labelForPressureCalculation = "Давления P = P(t)";
        private string labelForConsumptionCalculation = "Расходы Q = Q(t)";
        private ShowMode wellViewShowMode = ShowMode.Pressures;

        #endregion

        #region Pressures
        #region if one debit
        public List<DataPoint> Pressures1Times1
        {
            get => pressures1Times1;
            set
            {
                pressures1Times1 = value;
                OnPropertyChanged("Pressures1Times1");
            }
        }
        #endregion
        #region if two debits
        public List<DataPoint> Pressures1fTimes1f
        {
            get => pressures1fTimes1f;
            set
            {
                pressures1fTimes1f = value;
                OnPropertyChanged("Pressures1fTimes1f");
            }
        }
        public List<DataPoint> Pressures1sTimes1s
        {
            get => pressures1sTimes1s;
            set
            {
                pressures1sTimes1s = value;
                OnPropertyChanged("Pressures1sTimes1s");
            }
        }
        public List<DataPoint> Pressures2Times2
        {
            get => pressures2Times2;
            set
            {
                pressures2Times2 = value;
                OnPropertyChanged("Pressures2Times2");
            }
        }
        #endregion
        #region if three debits
        public List<DataPoint> Pressures2fTimes2f
        {
            get => pressures2fTimes2f;
            set
            {
                pressures2fTimes2f = value;
                OnPropertyChanged("Pressures2fTimes2f");
            }
        }
        public List<DataPoint> Pressures2sTimes2s
        {
            get => pressures2sTimes2s;
            set
            {
                pressures2sTimes2s = value;
                OnPropertyChanged("Pressures2sTimes2s");
            }
        }
        public List<DataPoint> Pressures3Times3
        {
            get => pressures3Times3;
            set
            {
                pressures3Times3 = value;
                OnPropertyChanged("Pressures3Times3");
            }
        }
        #endregion
        public List<DataPoint> StaticPressuresTimes
        {
            get => staticPressuresTimes;
            set
            {
                staticPressuresTimes = value;
                OnPropertyChanged("StaticPressuresTimes");
            }
        }
        #endregion

        #region Consumptions
        public List<DataPoint> ConsumptionsTimes
        {
            get => consumptionsTimes;
            set
            {
                consumptionsTimes = value;
                OnPropertyChanged("ConsumptionsTimes");
            }
        }
        public List<DataPoint> StaticConsumptionsTimes
        {
            get => staticConsumptionsTimes;
            set
            {
                staticConsumptionsTimes = value;
                OnPropertyChanged("StaticConsumptionsTimes");
            }
        }
        #endregion

        #region Labels and Legends
        public string LabelForPressureCalculation
        {
            get => labelForPressureCalculation;
            set
            {
                labelForPressureCalculation = value;
                OnPropertyChanged("LabelForPressureCalculation");
            }
        }
        public string LabelForConsumptionCalculation
        {
            get => labelForConsumptionCalculation;
            set
            {
                labelForPressureCalculation = value;
                OnPropertyChanged("LabelForConsumptionCalculation");
            }
        }

        // TO DO : Add all shit from from PlotViewModel
        #endregion

        #region Show mode on WellView
        public ShowMode WellViewShowMode
        {
            get => wellViewShowMode;
            set
            {
                wellViewShowMode = value;
                OnPropertyChanged("WellViewShowMode");
            }
        }
        #endregion

        public void Reset()
        {
            StaticPressuresTimes = new List<DataPoint>();
            StaticConsumptionsTimes = new List<DataPoint>();
            ConsumptionsTimes = new List<DataPoint>();
            Pressures1fTimes1f = new List<DataPoint>();
            Pressures1sTimes1s = new List<DataPoint>();
            Pressures1Times1 = new List<DataPoint>();
            Pressures2fTimes2f = new List<DataPoint>();
            Pressures2sTimes2s = new List<DataPoint>();
            Pressures2Times2 = new List<DataPoint>();
            Pressures3Times3 = new List<DataPoint>();
        }

        #region Property changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }

    public enum ShowMode
    {
        Pressures = 1,
        Consumptions = 2,
    }
}
