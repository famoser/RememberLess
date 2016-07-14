using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Business.Repositories.Interfaces;
using Famoser.RememberLess.View.Enums;
using Famoser.RememberLess.View.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

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
        private readonly INoteRepository _noteRepository;
        private readonly IIndeterminateProgressService _progressService;
        private readonly IHistoryNavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INoteRepository noteRepository, IIndeterminateProgressService progressService, IHistoryNavigationService navigationService)
        {
            _noteRepository = noteRepository;
            _progressService = progressService;
            _navigationService = navigationService;

            _refreshCommand = new RelayCommand(Refresh, () => CanRefresh);
            _addNoteCommand = new RelayCommand(AddNote, () => CanAddNote);
            _removeNote = new RelayCommand<NoteModel>(RemoveNote);
            _toggleCompleted = new RelayCommand<NoteModel>(ToggleCompleted);

            if (IsInDesignMode)
            {
                NoteCollections = noteRepository.GetExampleCollections();
                ActiveCollection = NoteCollections[0];
            }
            else
            {
                Initialize();
            }
            _removeNoteCollection = new RelayCommand<NoteCollectionModel>(RemoveNoteCollection, CanRemoveNoteCollection);
            _saveNoteCollection = new RelayCommand<NoteCollectionModel>(SaveNoteCollection, CanSaveNoteCollection);
            _addNoteCollectionCommand = new RelayCommand(AddNoteCollection, () => CanAddNoteCollection);
            _selectNoteCommand = new RelayCommand<NoteModel>(SelectNote);

            Messenger.Default.Register<NoteCollectionModel>(this, Messages.Select, EvaluateSelectMessage);
        }

        private void EvaluateSelectMessage(NoteCollectionModel obj)
        {
            ActiveCollection = obj;
        }

        //enable for debug purposes
        private bool _isInitializing;
        private async void Initialize()
        {
            _progressService.ShowProgress(ProgressKeys.InitializingApplication);
            _isInitializing = true;
            _refreshCommand.RaiseCanExecuteChanged();

            NoteCollections = await _noteRepository.GetCollections();
            ActiveCollection = NoteCollections.FirstOrDefault();

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
            Messenger.Default.Send(Messages.NotesChanged);

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

        private string _newNoteCollection;
        public string NewNoteCollection
        {
            get { return _newNoteCollection; }
            set
            {
                if (Set(ref _newNoteCollection, value))
                    _addNoteCollectionCommand.RaiseCanExecuteChanged();
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
            Messenger.Default.Send(Messages.NotesChanged);
        }

        private readonly RelayCommand _addNoteCollectionCommand;
        public ICommand AddNoteCollectionCommand => _addNoteCollectionCommand;

        public bool CanAddNoteCollection => !string.IsNullOrEmpty(NewNoteCollection);

        private async void AddNoteCollection()
        {
            var newNoteCollection = new NoteCollectionModel()
            {
                Guid = Guid.NewGuid(),
                Name = NewNoteCollection,
                CreateTime = DateTime.Now
            };
            NewNoteCollection = "";
            await _noteRepository.Save(newNoteCollection);
            ActiveCollection = newNoteCollection;
        }

        private readonly RelayCommand<NoteModel> _toggleCompleted;
        public ICommand ToggleCompletedCommand => _toggleCompleted;

        private async void ToggleCompleted(NoteModel note)
        {
            note.IsCompleted = !note.IsCompleted;
            await _noteRepository.Save(note);
            Messenger.Default.Send(Messages.NotesChanged);
        }

        private readonly RelayCommand<NoteCollectionModel> _removeNoteCollection;
        public ICommand RemoveNoteCollectionCommand => _removeNoteCollection;

        public bool CanRemoveNoteCollection(NoteCollectionModel model)
        {
            return NoteCollections.Count > 1;
        }

        private async void RemoveNoteCollection(NoteCollectionModel model)
        {
            if (model == ActiveCollection)
            {
                var index = NoteCollections.IndexOf(ActiveCollection);
                if (index == 0)
                    ActiveCollection = NoteCollections[1];
                else
                    ActiveCollection = NoteCollections[--index];
            }
            await _noteRepository.Delete(model);
            Messenger.Default.Send(Messages.NotesChanged);
        }

        private readonly RelayCommand<NoteCollectionModel> _saveNoteCollection;
        public ICommand SaveNoteCollectionCommand => _saveNoteCollection;

        public bool CanSaveNoteCollection(NoteCollectionModel model)
        {
            return !string.IsNullOrEmpty(model?.Name);
        }

        private async void SaveNoteCollection(NoteCollectionModel model)
        {
            await _noteRepository.Save(model);
        }

        private readonly RelayCommand<NoteModel> _removeNote;
        public ICommand RemoveNoteCommand => _removeNote;

        private async void RemoveNote(NoteModel note)
        {
            await _noteRepository.Delete(note);
            Messenger.Default.Send(Messages.NotesChanged);
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

        private readonly RelayCommand<NoteModel> _selectNoteCommand;
        public ICommand SelectNoteCommand => _selectNoteCommand;

        private void SelectNote(NoteModel note)
        {
            _navigationService.NavigateTo(PageKeys.NotePage.ToString());
            Messenger.Default.Send(note, Messages.Select);
        }
    }
}