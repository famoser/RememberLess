﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Famoser.RememberLess.Business.Models;

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Converters.MainPage
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //SomeApprooveOrange
            var note = value as NoteModel;
            if (note != null && note.IsCompleted)
                return Application.Current.Resources["SomeApprooveOrange"] as SolidColorBrush;
            return Application.Current.Resources["ApprooveGreen"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
