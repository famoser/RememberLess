using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Converters.MainPage
{
    public class CountToVisibilityConverter
    : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = value as IList;
            return list?.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
