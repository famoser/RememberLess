using System;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Famoser.RememberLess.View.Enums;
using Famoser.RememberLess.View.ViewModel;
using GalaSoft.MvvmLight.Messaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private MainViewModel ViewModel => DataContext as MainViewModel;

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ViewModel.RefreshCommand.CanExecute(null))
                ViewModel.RefreshCommand.Execute(null);
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var vm = DataContext as MainViewModel;
                if (vm?.AddNoteCommand.CanExecute(null) == true)
                    vm.AddNoteCommand.Execute(null);
            }
        }

        private void Flyout_Closed(object sender, object e)
        {
            if (ViewModel.SaveNoteCollectionCommand.CanExecute(ViewModel.ActiveCollection))
                ViewModel.SaveNoteCollectionCommand.Execute(ViewModel.ActiveCollection);
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            RemoveNoteCollectionFlyout.Hide();
            AddNoteCollectionFlyout.Hide();
        }
    }
}
