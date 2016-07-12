using Windows.UI.Xaml.Controls;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.View.ViewModel;
using GalaSoft.MvvmLight.Ioc;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Famoser.RememberLess.Presentation.WindowsUniversal.UserControls.MainPage
{
    public sealed partial class NoteList : UserControl
    {
        public NoteList()
        {
            this.InitializeComponent();
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var vm = SimpleIoc.Default.GetInstance<MainViewModel>();
            vm.SelectNoteCommand.Execute(e.ClickedItem as NoteModel);
        }
    }
}
