using System.ComponentModel;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Business.Repositories.Interfaces;
using Famoser.RememberLess.View.Enums;
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
    public class NoteViewModel : ViewModelBase
    {
        private readonly INoteRepository _noteRepository;
        private readonly IHistoryNavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public NoteViewModel(INoteRepository noteRepository, IHistoryNavigationService navigationService)
        {
            _noteRepository = noteRepository;
            _navigationService = navigationService;

            _goBackCommand = new RelayCommand(GoBack);
            _saveNoteCommand = new RelayCommand(SaveNote, () => CanSaveNote);
            _removeNoteCommand = new RelayCommand(RemoveNote);

            if (IsInDesignMode)
            {
                ActiveNote = noteRepository.GetExampleCollections()[0].CompletedNotes[0];
            }

            Messenger.Default.Register<NoteModel>(this, Messages.Select, EvaluateSelectMessage);
        }

        private void EvaluateSelectMessage(NoteModel obj)
        {
            _originActiveNote = obj;

            if (ActiveNote != null)
                ActiveNote.PropertyChanged -= ActiveNoteOnPropertyChanged;
            ActiveNote = new NoteModel()
            {
                Content = obj.Content,
                IsCompleted = obj.IsCompleted,
                CreateTime = obj.CreateTime,
                Description = obj.Description
            };
            if (ActiveNote != null)
                ActiveNote.PropertyChanged += ActiveNoteOnPropertyChanged;
        }

        private void ActiveNoteOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _saveNoteCommand.RaiseCanExecuteChanged();
        }

        private NoteModel _activeNote;
        public NoteModel ActiveNote
        {
            get { return _activeNote; }
            set { Set(ref _activeNote, value); }
        }

        private NoteModel _originActiveNote;

        private readonly RelayCommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand;

        private void GoBack()
        {
            _navigationService.GoBack();
        }

        private readonly RelayCommand _saveNoteCommand;
        public ICommand SaveNoteCommand => _saveNoteCommand;
        private bool CanSaveNote => !_isSaving && (_originActiveNote.Description != ActiveNote.Description || _originActiveNote.Content != ActiveNote.Content || _originActiveNote.IsCompleted != ActiveNote.IsCompleted);
        private bool _isSaving;

        private async void SaveNote()
        {
            _isSaving = true;
            _saveNoteCommand.RaiseCanExecuteChanged();
            _removeNoteCommand.RaiseCanExecuteChanged();

            _originActiveNote.Description = ActiveNote.Description;
            _originActiveNote.Content = ActiveNote.Content;
            _originActiveNote.IsCompleted = ActiveNote.IsCompleted;
            await _noteRepository.Save(_originActiveNote);

            _isSaving = false;
            _saveNoteCommand.RaiseCanExecuteChanged();
            _removeNoteCommand.RaiseCanExecuteChanged();
        }


        private readonly RelayCommand _removeNoteCommand;
        public ICommand RemoveNoteCommand => _removeNoteCommand;
        private bool CanDeleteNote => !_isSaving;

        private async void RemoveNote()
        {
            _isSaving = true;
            _saveNoteCommand.RaiseCanExecuteChanged();
            _removeNoteCommand.RaiseCanExecuteChanged();

            await _noteRepository.Delete(_originActiveNote);
            GoBack();

            _isSaving = false;
            _saveNoteCommand.RaiseCanExecuteChanged();
            _removeNoteCommand.RaiseCanExecuteChanged();
        }

    }
}