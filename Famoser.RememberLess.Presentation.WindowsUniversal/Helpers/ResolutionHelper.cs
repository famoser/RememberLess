using Windows.UI.Xaml;

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Helpers
{
    public class ResolutionHelper
    {
        public double WidthOfDevice => Window.Current.Bounds.Width;

        public double HeightOfDevice => Window.Current.Bounds.Height;
    }
}
