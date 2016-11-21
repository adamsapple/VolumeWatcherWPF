using System;
using System.Diagnostics;

using System.ComponentModel;

using System.Windows.Media;
using System.Windows.Data;

using Audio.CoreAudio;
using Moral.Util;

namespace VolumeWatcher.ViewModel.Converter
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(MMDevice), typeof(ImageSource))]
    class MMDeviceToIconConverter : ReadOnlyConverter
    {
        /// <summary>
        /// Convert MMDevice to IconPath(string) 
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
                return null;
            }

            var device = (MMDevice)value;
            try
            {
                if (device?.State != EDeviceState.Active)
                {
                    return null;
                }
                var isLarge = false;
                if (parameter != null)
                {
                    isLarge = ((string)parameter).ToUpper().Equals("LARGE");
                }

                ImageSource img = WindowsUtil.GetIconFromEXEDLL2(device.IconPath, isLarge).ToImageSource();

                return img;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);

                return null;
            }
        }
    }
}
