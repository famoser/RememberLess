﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = (int) value;
            if (val > 0)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
