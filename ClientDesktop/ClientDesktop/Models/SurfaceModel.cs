using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.Models
{
    public class SurfaceModel : INotifyPropertyChanged
    {
        private int n;
        private double kLeft;
        private double kRight;
        private double kappaLeft;
        private double kappaRight;
        private double p0Left;
        private double p0Right;

        public int N
        {
            get => n;
            set
            {
                n = value;
                OnPropertyChanged("N");
            }
        }
        public double KLeft
        {
            get => kLeft;
            set
            {
                kLeft = value;
                OnPropertyChanged("KLeft");
            }
        }
        public double KRight
        {
            get => kRight;
            set
            {
                kRight = value;
                OnPropertyChanged("KRight");
            }
        }
        public double KappaLeft
        {
            get => kappaLeft;
            set
            {
                kappaLeft = value;
                OnPropertyChanged("KappaLeft");
            }
        }
        public double KappaRight
        {
            get => kappaRight;
            set
            {
                kappaRight = value;
                OnPropertyChanged("KappaRight");
            }
        }
        public double P0Left
        {
            get => p0Left;
            set
            {
                p0Left = value;
                OnPropertyChanged("P0Left");
            }
        }
        public double P0Right
        {
            get => p0Right;
            set
            {
                p0Right = value;
                OnPropertyChanged("P0Right");
            }
        }

        // hardcoded as fuck but I dont give a shit
        public string K_path_Q { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\K_Q.txt";
        public string Kappa_path_Q { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\Kappa_Q.txt";
        public string P0_path_Q { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\P0_Q.txt";
        public string Fmin_path_Q { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\Fmin_Q.txt";
        public string Report_file_name_Q { get; set; } = "Report_Q";

        public string K_path_P { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\K_P.txt";
        public string Kappa_path_P { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\Kappa_P.txt";
        public string P0_path_P { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\P0_P.txt";
        public string Fmin_path_P { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\Fmin_P.txt";
        public string Report_file_name_P { get; set; } = "Report_P";

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
