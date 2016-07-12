using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Famoser.RememberLess.Business.Models;

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Converters.MainPage
{
    public class SymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var note = value as NoteModel;
            if (note != null && note.IsCompleted)
                return Symbol.Remove;
            return Symbol.Accept;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
