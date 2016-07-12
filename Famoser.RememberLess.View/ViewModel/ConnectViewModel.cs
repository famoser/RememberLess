using System;
using Famoser.RememberLess.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Famoser.RememberLess.View.ViewModel
{
    public class ConnectViewModel : ViewModelBase
    {

        public ConnectViewModel()
        {
            if (IsInDesignMode)
            {
                _userIdentification = Guid.NewGuid().ToString().Trim('{', '}');
            }
            else
            {
                _userIdentification = Guid.NewGuid().ToString().Trim('{', '}');
            }

            Messenger.Default.Register<string>(this, Messages.QrCodeScanned, EvaluateMessage);
        }

        private void EvaluateMessage(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {

            }
        }

        public string QrCode => "RememberLessUser: " + UserIdentification;

        private readonly string _userIdentification;

        public string UserIdentification => _userIdentification;

        private string _newQrCode;

        public string NewQrCode
        {
            get { return _newQrCode; }
            set
            {
                if (Set(ref _newQrCode, value) && _newQrCode.Length > 6)
                {
                }
            }
        }
    }
}
