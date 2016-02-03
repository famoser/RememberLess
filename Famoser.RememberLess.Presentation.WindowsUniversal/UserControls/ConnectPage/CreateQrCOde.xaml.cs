using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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
