using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Singleton;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Business.Repositories.Interfaces;
using Famoser.RememberLess.View.Enums;
using Famoser.RememberLess.View.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;

namespace Famoser.RememberLess.View.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private INoteRepository _noteRepository;
        private IProgressService _progressService;
        private INavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INoteRepository noteRepository, IProgressService progressService, INavigationService navigationService)
        {
            _noteRepository = noteRepository;
            _progressService = progressService;
            _navigationService = navigationService;

            _refreshCommand = new RelayCommand(Refresh, () => CanRefresh);
            _addNoteCommand = new RelayCommand(AddNote, () => CanAddNote);
            _removeNote = new RelayCommand<NoteModel>(RemoveNote);
            _toggleCompleted = new RelayCommand<NoteModel>(ToggleCompleted);
            _connectCommand = new RelayCommand(Connect);

            if (IsInDesignMode)
            {
                NoteCollections = noteRepository.GetExampleCollections();
                ActiveCollection = NoteCollections[0];
            }
            else
            {
                Initialize();
            }

            Messenger.Default.Register<Messages>(this, EvaluateMessages);
            Messenger.Default.Register<NoteCollectionModel>(this, Messages.Select, EvaluateSelectMessage);
        }

        private void EvaluateSelectMessage(NoteCollectionModel obj)
        {
            ActiveCollection = obj;
        }

        private async void EvaluateMessages(Messages obj)
        {
            if (obj == Messages.SyncNotes)
                await SyncNotes();
        }

        //enable for debug purposes
        private bool _isInitializing;
        private async void Initialize()
        {
            _progressService.ShowProgress(ProgressKeys.InitializingApplication);
            _isInitializing = true;
            _refreshCommand.RaiseCanExecuteChanged();

            NoteCollections = await _noteRepository.GetCollections();
            ActiveCollection = NoteCollections[0];

            _isInitializing = false;
            _refreshCommand.RaiseCanExecuteChanged();
            _progressService.HideProgress(ProgressKeys.InitializingApplication);
        }

        private bool _isSyncing;
        private async Task SyncNotes()
        {
            _progressService.ShowProgress(ProgressKeys.SyncingNotes);
            _isSyncing = true;
            _refreshCommand.RaiseCanExecuteChanged();

            await _noteRepository.SyncNotes();

            _isSyncing = false;
            _refreshCommand.RaiseCanExecuteChanged();
            _progressService.HideProgress(ProgressKeys.SyncingNotes);
        }

        private readonly RelayCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand;

        public bool CanRefresh => !_isInitializing && !_isSyncing;

        private async void Refresh()
        {
            await SyncNotes();
        }

        private string _newNote;
        public string NewNote
        {
            get { return _newNote; }
            set
            {
                if (Set(ref _newNote, value))
                    _addNoteCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly RelayCommand _addNoteCommand;
        public ICommand AddNoteCommand => _addNoteCommand;

        public bool CanAddNote => !string.IsNullOrEmpty(_newNote);

        private async void AddNote()
        {
            var newNote = new NoteModel()
            {
                Content = NewNote,
                Guid = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                NoteCollection = ActiveCollection
            };
            NewNote = "";
            await _noteRepository.Save(newNote);
        }

        private readonly RelayCommand<NoteModel> _toggleCompleted;
        public ICommand ToggleCompletedCommand { get { return _toggleCompleted; } }

        private async void ToggleCompleted(NoteModel note)
        {
            note.IsCompleted = !note.IsCompleted;
            await _noteRepository.Save(note);
        }

        private readonly RelayCommand _connectCommand;
        public ICommand ConnectCommand => _connectCommand;

        private void Connect()
        {
            _navigationService.NavigateTo(PageKeys.ConnectPage.ToString());
        }

        private readonly RelayCommand<NoteModel> _removeNote;
        public ICommand RemoveNoteCommand => _removeNote;

        private async void RemoveNote(NoteModel note)
        {
            await _noteRepository.Delete(note);
        }

        private ObservableCollection<NoteCollectionModel> _noteCollections;
        public ObservableCollection<NoteCollectionModel> NoteCollections
        {
            get { return _noteCollections; }
            set { Set(ref _noteCollections, value); }
        }

        private NoteCollectionModel _activeCollection;
        public NoteCollectionModel ActiveCollection
        {
            get { return _activeCollection; }
            set { Set(ref _activeCollection, value); }
        }
    }
}