using ClientDesktop.Commands;
using ClientDesktop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientDesktop.ViewModels
{
    public class SurfaceViewModel : INotifyPropertyChanged
    {
        public SurfaceViewModel()
        {
            SurfaceModel = new SurfaceModel();
        }

        private SurfaceModel surfaceModel;

        public SurfaceModel SurfaceModel
        {
            get => surfaceModel;
            set
            {
                surfaceModel = value;
                OnPropertyChanged("SurfaceModel");
            }
        }

        private ICommand _FminQ_Surface;
        public ICommand FminQ_Surface
        {
            get
            {
                return _FminQ_Surface ?? new FminQ_SurfaceCommand(this);
            }
        }

        private ICommand _FminP_Surface;
        public ICommand FminP_Surface
        {
            get
            {
                return _FminP_Surface ?? new FminP_SurfaceCommand(this);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
