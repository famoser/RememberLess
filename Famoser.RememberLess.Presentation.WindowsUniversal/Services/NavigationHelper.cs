using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.RememberLess.View.Enums;
using GalaSoft.MvvmLight.Views;

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Services
{
    public class NavigationHelper
    {
        public static INavigationService CreateNavigationService()
        {
            var navigationService = new CustomNavigationService();

            navigationService.Implementation.Configure(PageKeys.MainPage.ToString(), typeof(Pages.MainPage));
            navigationService.Implementation.Configure(PageKeys.ConnectPage.ToString(), typeof(Pages.ConnectPage));
            navigationService.Implementation.Configure(PageKeys.NotePage.ToString(), typeof(Pages.NotePage));

            return navigationService;
        }
    }
}
