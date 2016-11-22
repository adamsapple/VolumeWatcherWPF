using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;

namespace VolumeWatcher.ViewModel.Converter
{
   /// <summary>
   /// 
   /// </summary>
    [ValueConversion(typeof(bool), typeof(Brush))]
    class BoolToBrushConverter : ReadOnlyConverter
    {
        /// <summary>
        /// Convert MMDevice to name(string) 
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">language</param>
        /// <returns>bool</returns>
        public override object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            var b = false;
            if (value != null)
            {
                b = (bool)value;
            }

            

            return b?Brushes.Gray:Brushes.Black;
        }
    }
}
