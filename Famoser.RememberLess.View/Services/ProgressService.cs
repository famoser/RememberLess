using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.RememberLess.View.Enums;
using Famoser.RememberLess.View.ViewModel;
using GalaSoft.MvvmLight.Ioc;

namespace Famoser.RememberLess.View.Services
{
    public class ProgressService : IProgressService
    {
        private readonly ProgressViewModel _viewModel;
        public ProgressService()
        {
            _viewModel = SimpleIoc.Default.GetInstance<ProgressViewModel>();
        }

        public void ShowProgress(ProgressKeys key)
        {
            _viewModel.SetProgressState(key, true);
        }

        public void HideProgress(ProgressKeys key)
        {
            _viewModel.SetProgressState(key, false);
        }
    }
}
