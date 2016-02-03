using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Famoser.RememberLess.Presentation.WindowsUniversal.Enums;
using Famoser.RememberLess.View.Enums;
using GalaSoft.MvvmLight.Messaging;
using ZXing;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Famoser.RememberLess.Presentation.WindowsUniversal.UserControls.ConnectPage
{
    public sealed partial class TakeImage : UserControl
    {
        public TakeImage()
        {
            this.InitializeComponent();
            Messenger.Default.Register<LocalMessages>(this, EvaluateMessages);
            Start();
        }

        private void EvaluateMessages(LocalMessages obj)
        {
            if (obj == LocalMessages.StartCamera)
                Start();
            else if (obj == LocalMessages.StopCamera)
                Stop();
        }

        private MediaCapture _mediaCapture;
        private bool _stopRequested;
        private bool _isRunning;
        private async void Start()
        {
            if(_isRunning)
                return;
            _isRunning = true;
            _stopRequested = false;

            // Find all available webcams
            DeviceInformationCollection webcamList = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the proper webcam (default one)
            DeviceInformation backWebcam = (from webcam in webcamList
                                            where webcam.IsEnabled
                                            select webcam).FirstOrDefault();

            // Initializing MediaCapture
            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
            {
                VideoDeviceId = backWebcam.Id,
                AudioDeviceId = "",
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.VideoPreview
            });

            // Set the source of CaptureElement to MediaCapture
            CaptureElement.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();

            var imgProp = new ImageEncodingProperties { Subtype = "BMP", Width = 600, Height = 800 };
            var bcReader = new BarcodeReader();

            while (!_stopRequested)
            {
                var stream = new InMemoryRandomAccessStream();
                await _mediaCapture.CapturePhotoToStreamAsync(imgProp, stream);

                stream.Seek(0);
                var wbm = new WriteableBitmap(600, 800);
                await wbm.SetSourceAsync(stream);

                var result = bcReader.Decode(wbm);

                if (result != null)
                {
                    Messenger.Default.Send(result.Text, Messages.QrCodeScanned);
                    var msgbox = new MessageDialog(result.Text);
                    await msgbox.ShowAsync();
                }
            }
            _isRunning = false;
        }

        private async void Stop()
        {
            _stopRequested = true;
            await Task.Delay(1000);
            await _mediaCapture.StopPreviewAsync();
        }
    }
}
