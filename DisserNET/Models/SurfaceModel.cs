using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DisserNET.Models
{
    public class SurfaceModel : INotifyPropertyChanged
    {
        // hardcoded as fuck but I dont give a shit
        public string K_path_Q { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\K_Q.txt";
        public string Kappa_path_Q { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\Kappa_Q.txt";
        public string P0_path_Q { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\P0_Q.txt";
        public string Fmin_path_Q { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\Fmin_Q.txt";
        public string Report_file_name_Q { get; set; } = "Report_Q";

        public string K_path_P { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\K_P.txt";
        public string Kappa_path_P { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\Kappa_P.txt";
        public string P0_path_P { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\P0_P.txt";
        public string Fmin_path_P { get; set; } = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\Fmin_P.txt";
        public string Report_file_name_P { get; set; } = "Report_P";

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
