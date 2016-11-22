//class MMDeviceToDescriptionConverter

using System;
using System.ComponentModel;
using System.Windows.Data;

using Audio.CoreAudio;


namespace VolumeWatcher.ViewModel.Converter
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(MMDevice), typeof(string))]
    class MMDeviceToDescriptionConverter : ReadOnlyConverter
    {
        /// <summary>
        /// Convert MMDevice to Description(string) 
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">language</param>
        /// <returns>bool</returns>
        public override object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var device = value as MMDevice;

            return device?.DeviceDescription;
        }
    }
}


