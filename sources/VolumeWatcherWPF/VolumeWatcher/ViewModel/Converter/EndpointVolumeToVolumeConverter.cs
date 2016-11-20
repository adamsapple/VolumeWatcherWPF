using System;
using System.ComponentModel;
using System.Windows.Data;

using Audio.CoreAudio;

namespace VolumeWatcher.ViewModel.Converter
{
    [ValueConversion(typeof(AudioEndpointVolume), typeof(int))]
    class EndpointVolumeToVolumeConverter : IValueConverter
    {
        /// <summary>
        /// Convert AudioEndpointVolume to volumescalar(int) 
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
                return 0;
            }

            var endpoint = (AudioEndpointVolume)value;
            var vol      = endpoint.MasterVolumeLevelScalar;

            return (int)Math.Round(vol * 100);
        }

        /// <summary>
        /// Convert set Volume.
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
