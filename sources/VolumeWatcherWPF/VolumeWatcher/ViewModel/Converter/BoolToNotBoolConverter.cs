using System;
using System.Windows.Data;

namespace VolumeWatcher.ViewModel.Converter
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    class BoolToNotBoolConverter : IValueConverter
    {
        /// <summary>
        /// bool to !bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>bool</returns>
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            var result = (bool)value;
            
            return !result;
        }

        public object ConvertBack(object value, Type targetType,
                        object parameter, System.Globalization.CultureInfo culture)
        {
            var result = (bool)value;

            return !result;
        }
    }
}

