using DisserNET.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DisserNET.ValueConverters
{
    public class AddWellConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PlotShowModeConverter : IMultiValueConverter
    {
        /// <summary>
        /// first param for plot name
        /// second for current mode
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var plotName = values[0].ToString();
            var currentModeStr = values[1].ToString();
            if (Enum.TryParse(typeof(ShowMode), currentModeStr, out var showModeObj))
            {
                ShowMode showMode = (ShowMode)showModeObj;
                if (plotName.StartsWith('C'))
                {
                    return showMode == ShowMode.Consumptions ? Visibility.Visible : Visibility.Hidden;
                }
                if (plotName.StartsWith('P'))
                {
                    return showMode == ShowMode.Pressures ? Visibility.Visible : Visibility.Hidden;
                }
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => new object[] { };
    }
}
