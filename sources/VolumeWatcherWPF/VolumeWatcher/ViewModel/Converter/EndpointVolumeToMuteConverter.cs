using System;
using System.ComponentModel;
using System.Windows.Data;

using Audio.CoreAudio;


namespace VolumeWatcher.ViewModel.Converter
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(AudioEndpointVolume), typeof(bool))]
    class EndpointVolumeToMuteConverter : IValueConverter
    {
        /// <summary>
        /// Convert AudioEndpointVolume to mute(bool) 
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
                return true;
            }

            var endpoint = (AudioEndpointVolume)value;
            
            return endpoint.Mute;
        }

        /// <summary>
        /// Convert set Mute.
        /// throw Exception
        /// </summary>
        /// <param name="value">bool value</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">language</param>
        /// <returns>Enum</returns>
        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
