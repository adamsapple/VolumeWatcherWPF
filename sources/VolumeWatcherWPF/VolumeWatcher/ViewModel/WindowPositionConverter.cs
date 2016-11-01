using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using VolumeWatcher.Enumrate;

namespace VolumeWatcher.ViewModel
{
    public class WindowPositionConverter : System.Windows.Data.IValueConverter
    {
        /// <summary>
        /// Convert Enum to bool 
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">language</param>
        /// <returns>bool</returns>
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            return value == parameter || Enum.GetName(typeof(EWindowPosition), value).Equals(parameter);
        }

        /// <summary>
        /// Convert bool to Enum 
        /// </summary>
        /// <param name="value">bool value</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">language</param>
        /// <returns>Enum</returns>
        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            var boolean = (bool)(value as bool?);
            // true→falseの変化は無視する
            if (!boolean)
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }
            
            // ConverterParameterをEnumに変換して返す
            return Enum.Parse(targetType, parameter.ToString());
        }
    }
}
