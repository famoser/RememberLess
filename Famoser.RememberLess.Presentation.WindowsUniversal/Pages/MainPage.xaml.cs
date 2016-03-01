using System;
using System.ComponentModel;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
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
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, ev) =>
            {
                if (!ev.Handled)
                {
                    if (EditCollectionGrid.Visibility == Visibility.Visible)
                    {
                        ev.Handled = true;
                        EditCollectionGrid.Visibility = Visibility.Collapsed;
                    }
                }
            };
        }

        private MainViewModel ViewModel => DataContext as MainViewModel;

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ViewModel.RefreshCommand.CanExecute(null))
                ViewModel.RefreshCommand.Execute(null);
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "ActiveCollection")
            {
                MySplitView.IsPaneOpen = false;
                if (NoteCollectionsOverview.Visibility == Visibility.Visible)
                    UIElement_OnTapped(null, null);
            }
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
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

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("Liste wirklich löschen? Dieser Schritt kann nicht rückgängig gemacht werden", "Liste löschen");
            dialog.Commands.Add(new UICommand("abbrechen"));
            dialog.Commands.Add(new UICommand("löschen", command =>
            {
                if (ViewModel.RemoveNoteCollectionCommand.CanExecute(ViewModel.ActiveCollection))
                    ViewModel.RemoveNoteCollectionCommand.Execute(ViewModel.ActiveCollection);
                EditCollectionGrid.Visibility = Visibility.Collapsed;
            }));
            dialog.CancelCommandIndex = 0;
            dialog.DefaultCommandIndex = 0;
            await dialog.ShowAsync();
        }

        private void ListsAppbar_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }


        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
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
    }
}
