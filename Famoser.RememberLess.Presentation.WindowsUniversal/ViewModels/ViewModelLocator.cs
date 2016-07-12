using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.UniversalWindows.Platform;
using Famoser.RememberLess.Presentation.WindowsUniversal.Pages;
using Famoser.RememberLess.Presentation.WindowsUniversal.Services.Mocks;
using Famoser.RememberLess.View.Enums;
using Famoser.RememberLess.View.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using StorageService = Famoser.RememberLess.Presentation.WindowsUniversal.Services.StorageService;

namespace Famoser.RememberLess.Presentation.WindowsUniversal.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public partial class ViewModelLocator : BaseViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Create design time view services and models
            SimpleIoc.Default.Register<IStorageService, StorageService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();

            var navigationService = new HistoryNavigationServices();
            navigationService.Configure(PageKeys.MainPage.ToString(), typeof(MainPage));
            navigationService.Configure(PageKeys.ConnectPage.ToString(), typeof(ConnectPage));
            navigationService.Configure(PageKeys.NotePage.ToString(), typeof(NotePage));
            SimpleIoc.Default.Register(() => navigationService);
        }
    }
}
