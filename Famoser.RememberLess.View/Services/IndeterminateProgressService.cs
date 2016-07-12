using Famoser.RememberLess.View.Enums;
using Famoser.RememberLess.View.ViewModel;
using GalaSoft.MvvmLight.Ioc;

namespace Famoser.RememberLess.View.Services
{
    public class IndeterminateProgressService : IIndeterminateProgressService
    {
        private readonly ProgressViewModel _viewModel;
        public IndeterminateProgressService()
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
