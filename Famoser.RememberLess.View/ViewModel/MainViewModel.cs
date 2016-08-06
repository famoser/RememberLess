using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Business.Repositories.Interfaces;
using Famoser.RememberLess.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
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
        private readonly IProgressService _progressService;
        private readonly IHistoryNavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INoteRepository noteRepository, IProgressService progressService, IHistoryNavigationService navigationService)
        {
            _noteRepository = noteRepository;
            _progressService = progressService;
            _navigationService = navigationService;

            _refreshCommand = new LoadingRelayCommand(Refresh);
            _addNoteCommand = new LoadingRelayCommand(AddNote, () => CanAddNote);
            _removeNote = new LoadingRelayCommand<NoteModel>(RemoveNote);
            _toggleCompleted = new LoadingRelayCommand<NoteModel>(ToggleCompleted);

            NoteCollections = noteRepository.GetCollections();
            NoteCollections.CollectionChanged += NoteCollectionsOnCollectionChanged;
            if (IsInDesignMode)
            {
                ActiveCollection = NoteCollections[0];
            }

            _removeNoteCollection = new LoadingRelayCommand<NoteCollectionModel>(RemoveNoteCollection, CanRemoveNoteCollection);
            _saveNoteCollection = new LoadingRelayCommand<NoteCollectionModel>(SaveNoteCollection, CanSaveNoteCollection);
            _addNoteCollectionCommand = new LoadingRelayCommand(AddNoteCollection, () => CanAddNoteCollection);
            _selectNoteCommand = new RelayCommand<NoteModel>(SelectNote);
        }
        
        private void NoteCollectionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (NoteCollections.Count > 0)
            {
                ActiveCollection = NoteCollections[0];
                NoteCollections.CollectionChanged -= NoteCollectionsOnCollectionChanged;
            }
        }

        private readonly LoadingRelayCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand;

        private async void Refresh()
        {
            using (_refreshCommand.GetProgressDisposable(_progressService, ProgressKeys.SyncingNotes))
            {
                await _noteRepository.SyncNotes();
                Messenger.Default.Send(Messages.NotesChanged);
            }
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

        private readonly LoadingRelayCommand _addNoteCommand;
        public ICommand AddNoteCommand => _addNoteCommand;

        public bool CanAddNote => !string.IsNullOrEmpty(_newNote);

        private async void AddNote()
        {
            using (_addNoteCommand.GetProgressDisposable(_progressService, ProgressKeys.SavingNote))
            {
                var newNote = new NoteModel()
                {
                    Content = NewNote,
                    Guid = Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    NoteCollection = ActiveCollection
                };
                await _noteRepository.Save(newNote);
                NewNote = "";

                Messenger.Default.Send(Messages.NotesChanged);
            }
        }

        private readonly LoadingRelayCommand _addNoteCollectionCommand;
        public ICommand AddNoteCollectionCommand => _addNoteCollectionCommand;

        public bool CanAddNoteCollection => !string.IsNullOrEmpty(NewNoteCollection);

        private async void AddNoteCollection()
        {
            using (_addNoteCollectionCommand.GetProgressDisposable(_progressService, ProgressKeys.SavingNote))
            {
                var newNoteCollection = new NoteCollectionModel()
                {
                    Guid = Guid.NewGuid(),
                    Name = NewNoteCollection,
                    CreateTime = DateTime.Now
                };
                await _noteRepository.Save(newNoteCollection);
                NewNoteCollection = "";

                ActiveCollection = newNoteCollection;
            }
        }

        private readonly LoadingRelayCommand<NoteModel> _toggleCompleted;
        public ICommand ToggleCompletedCommand => _toggleCompleted;

        private async void ToggleCompleted(NoteModel note)
        {
            using (_toggleCompleted.GetProgressDisposable(_progressService, ProgressKeys.SavingNote))
            {
                note.IsCompleted = !note.IsCompleted;
                await _noteRepository.Save(note);
                Messenger.Default.Send(Messages.NotesChanged);
            }
        }

        private readonly LoadingRelayCommand<NoteCollectionModel> _removeNoteCollection;
        public ICommand RemoveNoteCollectionCommand => _removeNoteCollection;

        public bool CanRemoveNoteCollection(NoteCollectionModel model)
        {
            return NoteCollections.Count > 1;
        }

        private async void RemoveNoteCollection(NoteCollectionModel model)
        {
            using (_removeNoteCollection.GetProgressDisposable(_progressService, ProgressKeys.SavingNote))
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
        }

        private readonly LoadingRelayCommand<NoteCollectionModel> _saveNoteCollection;
        public ICommand SaveNoteCollectionCommand => _saveNoteCollection;

        public bool CanSaveNoteCollection(NoteCollectionModel model)
        {
            return !string.IsNullOrEmpty(model?.Name);
        }

        private async void SaveNoteCollection(NoteCollectionModel model)
        {
            using (_saveNoteCollection.GetProgressDisposable(_progressService, ProgressKeys.SavingNote))
            {
                await _noteRepository.Save(model);
            }
        }

        private readonly LoadingRelayCommand<NoteModel> _removeNote;
        public ICommand RemoveNoteCommand => _removeNote;

        private async void RemoveNote(NoteModel note)
        {
            using (_removeNote.GetProgressDisposable(_progressService, ProgressKeys.SavingNote))
            {
                await _noteRepository.Delete(note);
                Messenger.Default.Send(Messages.NotesChanged);
            }
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
            SimpleIoc.Default.GetInstance<NoteViewModel>().SelectNote(note);

            _activeCollection = note.NoteCollection;
            RaisePropertyChanged(() => ActiveCollection);
        }
    }
}