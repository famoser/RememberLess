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

            if (IsInDesignMode)
            {
                Notes = noteRepository.GetExampleNotes();
            }
            else
            {
                Initialize();
            }
        }

        //enable for debug purposes
        private bool _isInitializing;
        private async void Initialize()
        {
            _progressService.ShowProgress(ProgressKeys.InitializingApplication);
            _isInitializing = true;
            _refreshCommand.RaiseCanExecuteChanged();

            Notes = await _noteRepository.GetNotes();

            await SyncNotes();

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
                Notes = notes;

            _isSyncing = false;
            _refreshCommand.RaiseCanExecuteChanged();
            _progressService.HideProgress(ProgressKeys.SyncingNotes);
        }

        private readonly RelayCommand _refreshCommand;
        public ICommand RefreshCommand { get { return _refreshCommand; } }

        public bool CanRefresh { get { return !_isInitializing && !_isSyncing; } }

        private void Refresh()
        {
            Initialize();
        }

        private string _newNote;
        public string NewNote
        {
            get { return _newNote; }
            set { Set(ref _newNote, value); }
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
            RaisePropertyChanged(() => CompletedNotes);
            RaisePropertyChanged(() => NewNote);

            await _noteRepository.SaveNote(newNote);
        }

        private readonly RelayCommand<NoteModel> _toggleCompleted;
        public ICommand ToggleCompletedCommand { get { return _toggleCompleted; } }

        private async void ToggleCompleted(NoteModel note)
        {
            note.IsCompleted = !note.IsCompleted;
            RaisePropertyChanged(() => CompletedNotes);
            RaisePropertyChanged(() => NewNote);

            await _noteRepository.SaveNote(note);
        }

        private readonly RelayCommand<NoteModel> _removeNote;
        public ICommand RemoveNoteCommand { get { return _removeNote; } }

        private async void RemoveNote(NoteModel note)
        {
            Notes.Remove(note);
            RaisePropertyChanged(() => CompletedNotes);
            RaisePropertyChanged(() => NewNote);

            note.DeletePending = true;
            await _noteRepository.SaveNote(note);
        }


        private List<NoteModel> _notes;
        private List<NoteModel> Notes
        {
            get { return _notes; }
            set
            {
                if (Set(ref _notes, value))
                {
                    RaisePropertyChanged(() => CompletedNotes);
                    RaisePropertyChanged(() => NewNotes);
                }
            }
        }
        
        public ObservableCollection<NoteModel> CompletedNotes
        {
            get { return new ObservableCollection<NoteModel>(_notes.Where(n => n.IsCompleted).OrderByDescending(n => n.CreateTime)); }
        }
        
        public ObservableCollection<NoteModel> NewNotes
        {
            get { return new ObservableCollection<NoteModel>(_notes.Where(n => !n.IsCompleted).OrderByDescending(n => n.CreateTime)); }
        }
    }
}