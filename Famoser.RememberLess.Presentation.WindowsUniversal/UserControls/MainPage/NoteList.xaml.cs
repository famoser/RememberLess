using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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
