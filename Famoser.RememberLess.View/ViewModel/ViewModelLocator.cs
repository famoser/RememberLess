/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Famoser.RememberLess.View"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.RememberLess.Business.Repositories;
using Famoser.RememberLess.Business.Repositories.Interfaces;
using Famoser.RememberLess.Data.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Famoser.RememberLess.View.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class BaseViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public BaseViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IDataService, DataService>();

            if (ViewModelBase.IsInDesignModeStatic)
                SimpleIoc.Default.Register<INoteRepository, MockNoteRepository>();
            else
                SimpleIoc.Default.Register<INoteRepository, NoteRepository>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<NoteViewModel>();
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();
        public ProgressService ProgressViewModel => ServiceLocator.Current.GetInstance<IProgressService>() as ProgressService;
        public NoteViewModel NoteViewModel => ServiceLocator.Current.GetInstance<NoteViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}