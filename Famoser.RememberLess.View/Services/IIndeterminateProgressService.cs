using Famoser.RememberLess.View.Enums;

namespace Famoser.RememberLess.View.Services
{
    public interface IIndeterminateProgressService
    {
        void ShowProgress(ProgressKeys key);
        void HideProgress(ProgressKeys key);
    }
}
