﻿using System;
using System.ComponentModel;
using System.Windows.Data;

namespace VolumeWatcher.ViewModel.Converter
{
    abstract class ReadOnlyConverter : IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter,
                             System.Globalization.CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
