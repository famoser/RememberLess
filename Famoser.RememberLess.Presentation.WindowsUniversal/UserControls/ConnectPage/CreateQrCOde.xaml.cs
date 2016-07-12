using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TCD.Device.Camera.Barcodes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Famoser.RememberLess.Presentation.WindowsUniversal.UserControls.ConnectPage
{
    public sealed partial class CreateQrCode : UserControl
    {
        public CreateQrCode()
        {
            this.InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var qrcode = DataContext as string;
            if (qrcode != null)
            {
                var width = ActualWidth > 0 ? (int)ActualWidth : 200;
                var source = await Encoder.GenerateQRCodeAsync(qrcode, width);
                QrImage.Source = source;
            }
        }
    }
}
