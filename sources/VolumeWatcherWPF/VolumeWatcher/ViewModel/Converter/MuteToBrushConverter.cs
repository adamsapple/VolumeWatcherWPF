﻿using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;

namespace VolumeWatcher.ViewModel.Converter
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Brush))]
    class MuteToBrushConverter : ReadOnlyConverter
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
            var mute = false;
            var result = Brushes.DarkGray;
            if (value != null)
            {
                mute = (bool)value;
            }
            if (mute)
            {
                result =  Brushes.DarkGray;
            }
            else
            {
                var argbStr = (string)parameter;
                
                var color = argbStr.GetColorFromARGBString();
                result = new SolidColorBrush(color);
            }

            //Debug.WriteLine(result);

            return result;
        }
    }

    static class StringExtention
    {
        public static Color GetColorFromARGBString(this string str) {

            var a = Byte.Parse(str.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            var r = Byte.Parse(str.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            var g = Byte.Parse(str.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            var b = Byte.Parse(str.Substring(7, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

            var color = Color.FromArgb(a, r, g, b);
            
            return color;
        }
    }
}
