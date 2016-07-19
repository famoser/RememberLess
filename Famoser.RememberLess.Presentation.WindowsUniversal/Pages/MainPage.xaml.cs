using System;
using System.ComponentModel;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.RememberLess.View.ViewModel;
using GalaSoft.MvvmLight.Ioc;

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
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, ev) =>
            {
                if (!ev.Handled)
                {
                    if (EditCollectionGrid.Visibility == Visibility.Visible)
                    {
                        ev.Handled = true;
                        EditCollectionGrid.Visibility = Visibility.Collapsed;
                    }
                    else if (NoteCollectionsOverview.Visibility == Visibility.Visible)
                    {
                        ev.Handled = true;
                        UIElement_OnTapped();
                    }
                }
            };
        }

        private MainViewModel ViewModel => DataContext as MainViewModel;

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "ActiveCollection")
            {
                if (NoteCollectionsOverview.Visibility == Visibility.Visible)
                    UIElement_OnTapped();
            }
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var vm = DataContext as MainViewModel;
                var tb = sender as TextBox;
                if (tb == AddNewNoteCollectionTextBox)
                {
                    if (vm?.AddNoteCollectionCommand.CanExecute(null) == true)
                        vm.AddNoteCollectionCommand.Execute(null);
                }
                else if (tb == AddNewNoteTextBox)
                {
                    if (vm?.AddNoteCommand.CanExecute(null) == true)
                        vm.AddNoteCommand.Execute(null);
                }
            }
        }
        
        private void EditCollectionButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EditCollectionGrid.Visibility = EditCollectionGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }


        private void UIElement_OnTapped(object sender = null, TappedRoutedEventArgs e = null)
        {
            if (NoteCollectionsOverview.Visibility == Visibility.Visible)
            {
                NoteCollectionsOverview.Visibility = Visibility.Collapsed;
                ActiveNoteCollection.Visibility = Visibility.Visible;
            }
            else
            {
                NoteCollectionsOverview.Visibility = Visibility.Visible;
                ActiveNoteCollection.Visibility = Visibility.Collapsed;
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            UIElement_OnTapped();
        }
    }
}
