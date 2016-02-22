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
                Notes = noteRepository.GetExampleNotes();
                NotesModified(null, NoteAction.Unknown);
            }
            else
            {
                Initialize();
            }

            Messenger.Default.Register<Messages>(this, EvaluateMessages);
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

            Notes = await _noteRepository.GetNotes();
            NotesModified(null, NoteAction.Unknown);

            

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

            var notes = await _noteRepository.SyncNotes(_notes);
            if (notes != null)
            {
                Notes = notes;
                NotesModified(null, NoteAction.Unknown);
            }

            _isSyncing = false;
            _refreshCommand.RaiseCanExecuteChanged();
            _progressService.HideProgress(ProgressKeys.SyncingNotes);
        }

        private readonly RelayCommand _refreshCommand;
        public ICommand RefreshCommand { get { return _refreshCommand; } }

        public bool CanRefresh { get { return !_isInitializing && !_isSyncing; } }

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
        public ICommand AddNoteCommand { get { return _addNoteCommand; } }

        public bool CanAddNote { get { return !string.IsNullOrEmpty(_newNote); } }

        private async void AddNote()
        {
            var newNote = new NoteModel()
            {
                Content = NewNote,
                Guid = Guid.NewGuid(),
                CreateTime = DateTime.Now
            };
            Notes.Add(newNote);
            NotesModified(newNote, NoteAction.Add);
            NewNote = "";

            await _noteRepository.Save(newNote, Notes);
        }

        private readonly RelayCommand<NoteModel> _toggleCompleted;
        public ICommand ToggleCompletedCommand { get { return _toggleCompleted; } }

        private async void ToggleCompleted(NoteModel note)
        {
            note.IsCompleted = !note.IsCompleted;
            NotesModified(note, note.IsCompleted ? NoteAction.ToCompleted : NoteAction.ToNotCompleted);

            await _noteRepository.Save(note, Notes);
        }

        private readonly RelayCommand _connectCommand;
        public ICommand ConnectCommand { get { return _connectCommand; } }

        private void Connect()
        {
            _navigationService.NavigateTo(PageKeys.ConnectPage.ToString());
        }

        private readonly RelayCommand<NoteModel> _removeNote;
        public ICommand RemoveNoteCommand { get { return _removeNote; } }

        private async void RemoveNote(NoteModel note)
        {
            Notes.Remove(note);
            NotesModified(note, NoteAction.Remove);

            note.DeletePending = true;
            await _noteRepository.Save(note, Notes);
        }

        private void NotesModified(NoteModel note, NoteAction action)
        {
            if (action == NoteAction.Add)
                _newNotes.Insert(0, note);
            else if (action == NoteAction.Remove)
            {
                if (_newNotes.Contains(note))
                    _newNotes.Remove(note);
                else if (_completedNotes.Contains(note))
                    _completedNotes.Remove(note);
            }
            else if (action == NoteAction.ToCompleted)
            {
                if (_newNotes.Contains(note))
                    _newNotes.Remove(note);
                InsertInOrder(note, _completedNotes);
            }
            else if (action == NoteAction.ToNotCompleted)
            {
                if (_completedNotes.Contains(note))
                    _completedNotes.Remove(note);
                InsertInOrder(note, _newNotes);
            }
            else
            {
                _completedNotes = null;
                _newNotes = null;
                RaisePropertyChanged(() => CompletedNotes);
                RaisePropertyChanged(() => NewNotes);
            }
            Messenger.Default.Send(Messages.NotesChanged);
        }

        private void InsertInOrder(NoteModel note, ObservableCollection<NoteModel> list)
        {

            var found = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CreateTime < note.CreateTime)
                {
                    list.Insert(i, note);
                    found = true;
                    break;
                }
            }
            if (!found)
                list.Add(note);
        }

        private List<NoteModel> _notes;
        private List<NoteModel> Notes
        {
            get { return _notes; }
            set { Set(ref _notes, value); }
        }

        private ObservableCollection<NoteModel> _completedNotes;
        public ObservableCollection<NoteModel> CompletedNotes
        {
            get
            {
                if (_completedNotes == null)
                    _completedNotes = new ObservableCollection<NoteModel>(_notes.Where(n => n.IsCompleted).OrderByDescending(n => n.CreateTime));
                return _completedNotes;
            }
        }

        private ObservableCollection<NoteModel> _newNotes;
        public ObservableCollection<NoteModel> NewNotes
        {
            get
            {
                if (_newNotes == null)
                    _newNotes = new ObservableCollection<NoteModel>(_notes.Where(n => !n.IsCompleted).OrderByDescending(n => n.CreateTime));
                return _newNotes;
            }
        }
    }
}