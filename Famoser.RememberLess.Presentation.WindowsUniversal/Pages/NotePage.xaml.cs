using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.RememberLess.View.ViewModel;
using GalaSoft.MvvmLight.Ioc;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotePage : Page
    {
        public NotePage()
        {
            this.InitializeComponent();
        }

        public static async Task GoBackEventHandler(object sender, BackRequestedEventArgs ev)
        {
            if (!ev.Handled)
            {
                var nvm = SimpleIoc.Default.GetInstance<NoteViewModel>();
                if (nvm.SaveNoteCommand.CanExecute(nvm.ActiveNote))
                {
                    ev.Handled = true;
                    var messageDialog = new MessageDialog("unsaved changes are pending, do you want to save them before going back?", "changes pending");
                    messageDialog.Commands.Add(new UICommand()
                    {
                        Label = "save changes",
                        Invoked = command =>
                        {
                            if (nvm.SaveNoteCommand.CanExecute(nvm.ActiveNote))
                                nvm.SaveNoteCommand.Execute(nvm.ActiveNote);
                            SimpleIoc.Default.GetInstance<IHistoryNavigationService>().GoBack();
                        }
                    });
                    messageDialog.Commands.Add(new UICommand()
                    {
                        Label = "discard changes",
                        Invoked = command =>
                        {
                            SimpleIoc.Default.GetInstance<IHistoryNavigationService>().GoBack();
                        }
                    });
                    messageDialog.Commands.Add(new UICommand()
                    {
                        Label = "cancel",
                        Invoked = command =>
                        {

                        }
                    });

                    messageDialog.CancelCommandIndex = 3;
                    messageDialog.DefaultCommandIndex = 1;

                    await messageDialog.ShowAsync();
                }
            }
        }
    }
}
