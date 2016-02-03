using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string QrCode
        {
            get { return "RememberLessUser: " + UserIdentification; }
        }

        private string _userIdentification;

        public string UserIdentification
        {
            get { return _userIdentification; }
        }

        private string _newQrCode;

        public string NewQrCode
        {
            get { return _newQrCode; }
            set
            {
                if (Set(ref _newQrCode, value) && _newQrCode.Length > 6)
                {
                    CheckIfGuidExists();
                }
            }
        }

        private async void CheckIfGuidExists()
        {

        }
    }
}
