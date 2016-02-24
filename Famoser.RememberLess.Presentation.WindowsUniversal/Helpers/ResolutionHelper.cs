using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Helpers
{
    public class ResolutionHelper
    {
        public double WidthOfDevice => Window.Current.Bounds.Width;

        public double HeightOfDevice => Window.Current.Bounds.Height;
    }
}
