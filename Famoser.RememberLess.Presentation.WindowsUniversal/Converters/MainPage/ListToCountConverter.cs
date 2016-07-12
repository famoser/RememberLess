using System;
using System.Collections;
using Windows.UI.Xaml.Data;

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Converters.MainPage
{
    public class ListToCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = value as IList;
            return list?.Count ?? 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
