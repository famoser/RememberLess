using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.RememberLess.Business.Converters;
using Famoser.RememberLess.Business.Enums;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Business.Repositories.Interfaces;
using Famoser.RememberLess.Data.Enum;
using Famoser.RememberLess.Data.Services;
using Newtonsoft.Json;

namespace Famoser.RememberLess.Business.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private const int ActiveDataVersion = 1;
        private readonly IDataService _dataService;
        private readonly IStorageService _storageService;

        public NoteRepository(IDataService dataService, IStorageService storageService)
        {
            _dataService = dataService;
            _storageService = storageService;
        }

        private UserInformationModel _userInformations;
        private DataModel _dataModel;


        public ObservableCollection<NoteCollectionModel> GetCollections()
        {
            try
            {
                if (await RetrieveUserInformationsFromStorage())
                {
                    if (await RetrieveNoteCollectionsFromStorage())
                    {
                        foreach (var noteCollection in _dataModel.Collections)
                        {
                            foreach (var note in noteCollection.NewNotes)
                            {
                                note.NoteCollection = noteCollection;
                            }
                            foreach (var note in noteCollection.CompletedNotes)
                            {
                                note.NoteCollection = noteCollection;
                            }
                            foreach (var note in noteCollection.DeletedNotes)
                            {
                                note.NoteCollection = noteCollection;
                            }
                        }
                        foreach (var noteCollection in _dataModel.DeletedCollections)
                        {
                            foreach (var note in noteCollection.NewNotes)
                            {
                                note.NoteCollection = noteCollection;
                            }
                            foreach (var note in noteCollection.CompletedNotes)
                            {
                                note.NoteCollection = noteCollection;
                            }
                            foreach (var note in noteCollection.DeletedNotes)
                            {
                                note.NoteCollection = noteCollection;
                            }
                        }
                    }
                    else
                    {
                        _dataModel = new DataModel();
                        //recover old data
                        if (_userInformations.SaveDataVersion == 0)
                        {
                            var oldCachedNotes =
                                await _storageService.GetCachedTextFileAsync("data.json");
                            if (!string.IsNullOrEmpty(oldCachedNotes))
                            {
                                var notes =
                                    JsonConvert.DeserializeObject<ObservableCollection<NoteModel>>(oldCachedNotes);
                                foreach (var noteModel in notes)
                                {
                                    noteModel.PendingAction = PendingAction.AddOrUpdate;
                                }
                                var coll = new NoteCollectionModel()
                                {
                                    Name = "To do",
                                    Guid = Guid.NewGuid(),
                                    NewNotes = new ObservableCollection<NoteModel>(notes.Where(n => !n.IsCompleted)),
                                    CompletedNotes =
                                        new ObservableCollection<NoteModel>(notes.Where(n => n.IsCompleted)),
                                    PendingAction = PendingAction.AddOrUpdate
                                };
                                foreach (var note in coll.NewNotes)
                                {
                                    note.NoteCollection = coll;
                                }
                                foreach (var note in coll.CompletedNotes)
                                {
                                    note.NoteCollection = coll;
                                }
                                _dataModel.Collections.Add(coll);
                            }
                            _userInformations.SaveDataVersion = 1;
                            await SaveUserInformationsToStorage();
                            await SaveNoteCollectionsToStorage();
                        }
                        await SyncNotes();
                    }
                }
                else
                {
                    var coll = new NoteCollectionModel()
                    {
                        Name = "To do",
                        Guid = Guid.NewGuid(),
                        NewNotes = new ObservableCollection<NoteModel>()
                        {
                            new NoteModel()
                            {
                                Guid = Guid.NewGuid(),
                                IsCompleted = false,
                                Content = "Add new Note",
                                CreateTime = DateTime.Now,
                                PendingAction = PendingAction.AddOrUpdate
                            },
                            new NoteModel()
                            {
                                Guid = Guid.NewGuid(),
                                IsCompleted = false,
                                Content = "Install Application on my other Windows devices to sync notes",
                                CreateTime = DateTime.Now,
                                PendingAction = PendingAction.AddOrUpdate
                            }
                        },
                        CompletedNotes = new ObservableCollection<NoteModel>()
                        {
                            new NoteModel()
                            {
                                Guid = Guid.NewGuid(),
                                IsCompleted = true,
                                Content = "Install to do application",
                                CreateTime = DateTime.Now,
                                PendingAction = PendingAction.AddOrUpdate
                            }
                        },
                        PendingAction = PendingAction.AddOrUpdate
                    };
                    foreach (var note in coll.NewNotes)
                    {
                        note.NoteCollection = coll;
                    }
                    foreach (var note in coll.CompletedNotes)
                    {
                        note.NoteCollection = coll;
                    }
                    _dataModel.Collections.Add(coll);

                    await SyncNotes();
                    await SaveNoteCollectionsToStorage();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return _dataModel.Collections;
        }

        public async Task<bool> SyncNotes()
        {
            try
            {
                //add/update/delete collections
                var collPending = _dataModel.Collections.Where(c => c.PendingAction == PendingAction.AddOrUpdate).ToList();
                if (collPending.Any())
                {
                    var addUpdateRequest = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid, PossibleActions.AddOrUpdate, collPending);
                    var addUpdateRes = (await _dataService.PostNoteCollection(addUpdateRequest)).IsSuccessfull;
                    if (addUpdateRes)
                        foreach (var collModel in collPending)
                            collModel.PendingAction = PendingAction.None;
                }

                //delete
                var collDelete = _dataModel.DeletedCollections.Where(c => c.PendingAction == PendingAction.Remove).ToList();
                if (collDelete.Any())
                {
                    var deleteRequest = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid, PossibleActions.Delete, collDelete);
                    var deleteRes = (await _dataService.PostNoteCollection(deleteRequest)).IsSuccessfull;
                    if (deleteRes)
                        foreach (var collModel in collDelete)
                        {
                            collModel.PendingAction = PendingAction.None;
                            _dataModel.DeletedCollections.Remove(collModel);
                        }
                }

                //sync
                var syncRequest = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid, PossibleActions.Get, new List<NoteCollectionModel>());
                var syncRequestResult = await _dataService.GetNoteCollections(syncRequest);
                if (syncRequestResult.IsSuccessfull)
                {
                    //actualize existing / add new
                    foreach (var collectionEntity in syncRequestResult.NoteCollections)
                    {
                        var existingModel = _dataModel.Collections.FirstOrDefault(n => n.Guid == collectionEntity.Guid);
                        if (existingModel == null)
                        {
                            var newModel = ResponseConverter.Instance.Convert(collectionEntity);
                            InsertIntoList(_dataModel.Collections, newModel);
                        }
                        else
                        {
                            ResponseConverter.Instance.WriteValues(collectionEntity, existingModel);
                        }
                    }
                    //remove old
                    var old = _dataModel.Collections.Where(n => syncRequestResult.NoteCollections.All(no => no.Guid != n.Guid)).ToList();
                    foreach (var noteModel in old)
                    {
                        _dataModel.Collections.Remove(noteModel);
                    }
                }


                //add/update/delete all notes
                foreach (var noteCollectionModel in _dataModel.Collections)
                {
                    //add & update
                    var pending = noteCollectionModel.CompletedNotes.Where(n => n.PendingAction == PendingAction.AddOrUpdate).ToList();
                    var pending2 = noteCollectionModel.NewNotes.Where(n => n.PendingAction == PendingAction.AddOrUpdate).ToList();
                    if (pending.Any() || pending2.Any())
                    {
                        pending.AddRange(pending2);
                        var addUpdateRequest = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid, noteCollectionModel.Guid, PossibleActions.AddOrUpdate, pending);
                        var addUpdateRes = await _dataService.PostNote(addUpdateRequest);
                        if (addUpdateRes.IsSuccessfull)
                            foreach (var noteModel in pending)
                                noteModel.PendingAction = PendingAction.None;
                    }

                    //removes
                    var removes = noteCollectionModel.DeletedNotes.Where(n => n.PendingAction == PendingAction.Remove).ToList();
                    if (removes.Any())
                    {
                        var deleteRequest = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid,
                            noteCollectionModel.Guid, PossibleActions.Delete, removes);
                        var delets = await _dataService.PostNote(deleteRequest);
                        if (delets.IsSuccessfull)
                            foreach (var noteModel in removes)
                            {
                                noteModel.PendingAction = PendingAction.None;
                                noteCollectionModel.DeletedNotes.Remove(noteModel);
                            }
                    }

                    //sync
                    var getRequest = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid, noteCollectionModel.Guid, PossibleActions.Get, new List<NoteModel>());
                    var getRequestResult = await _dataService.GetNotes(getRequest);
                    if (getRequestResult.IsSuccessfull)
                    {
                        //actualize existing / add new
                        foreach (var noteEntity in getRequestResult.Notes)
                        {
                            NoteModel existingModel = noteCollectionModel.CompletedNotes.FirstOrDefault(n => n.Guid == noteEntity.Guid) ?? noteCollectionModel.NewNotes.FirstOrDefault(n => n.Guid == noteEntity.Guid);
                            if (existingModel == null)
                            {
                                var newModel = ResponseConverter.Instance.Convert(noteEntity);
                                newModel.NoteCollection = noteCollectionModel;
                                InsertIntoList(!newModel.IsCompleted ? noteCollectionModel.NewNotes : noteCollectionModel.CompletedNotes, newModel);
                            }
                            else
                            {
                                ResponseConverter.Instance.WriteValues(noteEntity, existingModel);
                                if (noteEntity.IsCompletedBool != existingModel.IsCompleted)
                                {
                                    if (existingModel.IsCompleted && noteCollectionModel.CompletedNotes.Contains(existingModel))
                                        noteCollectionModel.CompletedNotes.Remove(existingModel);
                                    else if (!existingModel.IsCompleted && noteCollectionModel.NewNotes.Contains(existingModel))
                                        noteCollectionModel.NewNotes.Remove(existingModel);

                                    InsertIntoList(!existingModel.IsCompleted ? noteCollectionModel.NewNotes : noteCollectionModel.CompletedNotes, existingModel);
                                }
                            }
                        }
                        //remove old
                        var old = noteCollectionModel.CompletedNotes.Where(n => getRequestResult.Notes.All(no => no.Guid != n.Guid)).ToList();
                        foreach (var noteModel in old)
                        {
                            noteCollectionModel.CompletedNotes.Remove(noteModel);
                        }
                        var old2 = noteCollectionModel.NewNotes.Where(n => getRequestResult.Notes.All(no => no.Guid != n.Guid)).ToList();
                        foreach (var noteModel in old2)
                        {
                            noteCollectionModel.NewNotes.Remove(noteModel);
                        }
                    }
                }

                return await SaveNoteCollectionsToStorage();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        public async Task<bool> Save(NoteModel nm)
        {
            try
            {
                var obj = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid, nm.NoteCollection.Guid, PossibleActions.AddOrUpdate,
                    new List<NoteModel>() { nm });
                var res = await _dataService.PostNote(obj);
                nm.PendingAction = !res.IsSuccessfull ? PendingAction.AddOrUpdate : PendingAction.None;

                if (nm.NoteCollection.CompletedNotes.Contains(nm) && !nm.IsCompleted)
                    nm.NoteCollection.CompletedNotes.Remove(nm);
                else if (nm.NoteCollection.NewNotes.Contains(nm) && nm.IsCompleted)
                    nm.NoteCollection.NewNotes.Remove(nm);

                if (nm.IsCompleted && !nm.NoteCollection.CompletedNotes.Contains(nm))
                    InsertIntoList(nm.NoteCollection.CompletedNotes, nm);
                else if (!nm.IsCompleted && !nm.NoteCollection.NewNotes.Contains(nm))
                    InsertIntoList(nm.NoteCollection.NewNotes, nm);

                return await SaveNoteCollectionsToStorage() && res.IsSuccessfull;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private void InsertIntoList(ObservableCollection<NoteModel> list, NoteModel model)
        {
            var found = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CreateTime < model.CreateTime)
                {
                    list.Insert(i, model);
                    found = true;
                    break;
                }
            }
            if (!found)
                list.Add(model);
        }

        private void InsertIntoList(ObservableCollection<NoteCollectionModel> list, NoteCollectionModel model)
        {
            var found = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Compare(model.Name, list[i].Name, StringComparison.Ordinal) < 0)
                {
                    list.Insert(i, model);
                    found = true;
                    break;
                }
            }
            if (!found)
                list.Add(model);
        }

        public async Task<bool> Delete(NoteModel nm)
        {
            try
            {
                var obj = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid, nm.NoteCollection.Guid, PossibleActions.Delete,
                    new List<NoteModel>() { nm });
                var res = await _dataService.PostNote(obj);
                nm.PendingAction = !res.IsSuccessfull ? PendingAction.Remove : PendingAction.None;

                if (nm.NoteCollection.CompletedNotes.Contains(nm))
                    nm.NoteCollection.CompletedNotes.Remove(nm);
                else if (nm.NoteCollection.NewNotes.Contains(nm))
                    nm.NoteCollection.NewNotes.Remove(nm);
                else if (nm.NoteCollection.DeletedNotes.Contains(nm))
                    nm.NoteCollection.DeletedNotes.Remove(nm);

                if (nm.PendingAction != PendingAction.None)
                    nm.NoteCollection.DeletedNotes.Add(nm);

                return await SaveNoteCollectionsToStorage() && res.IsSuccessfull;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        public async Task<bool> Save(NoteCollectionModel nm)
        {
            try
            {
                var obj = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid, PossibleActions.AddOrUpdate,
                    new List<NoteCollectionModel>() { nm });
                var res = await _dataService.PostNoteCollection(obj);
                nm.PendingAction = !res.IsSuccessfull ? PendingAction.AddOrUpdate : PendingAction.None;

                if (!_dataModel.Collections.Contains(nm))
                    _dataModel.Collections.Add(nm);

                return await SaveNoteCollectionsToStorage() && res.IsSuccessfull;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        public async Task<bool> Delete(NoteCollectionModel nm)
        {
            try
            {
                var obj = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid, PossibleActions.Delete,
                    new List<NoteCollectionModel>() { nm });
                var res = await _dataService.PostNoteCollection(obj);
                nm.PendingAction = !res.IsSuccessfull ? PendingAction.Remove : PendingAction.None;

                if (_dataModel.Collections.Contains(nm))
                    _dataModel.Collections.Remove(nm);

                if (nm.PendingAction != PendingAction.None)
                    _dataModel.DeletedCollections.Add(nm);

                return await SaveNoteCollectionsToStorage() && res.IsSuccessfull;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private async Task<bool> SaveUserInformationsToStorage()
        {
            try
            {
                var userInformations = JsonConvert.SerializeObject(_userInformations);
                return await _storageService.SetRoamingTextFileAsync("user.json", userInformations);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private async Task<bool> RetrieveUserInformationsFromStorage()
        {
            try
            {
                var userInformations = await _storageService.GetRoamingTextFileAsync("user.json");
                if (userInformations != null)
                {
                    _userInformations = JsonConvert.DeserializeObject<UserInformationModel>(userInformations);
                    return _userInformations != null;
                }
                else
                {
                    _userInformations = new UserInformationModel(Guid.NewGuid()) { SaveDataVersion = ActiveDataVersion };
                    return await SaveUserInformationsToStorage();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private async Task<bool> SaveNoteCollectionsToStorage()
        {
            try
            {
                var notesJson = JsonConvert.SerializeObject(_dataModel);
                return await _storageService.SetCachedTextFileAsync("data2.json", notesJson);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private async Task<bool> RetrieveNoteCollectionsFromStorage()
        {
            try
            {
                var cachedNotesJson = await _storageService.GetCachedTextFileAsync("data2.json");
                if (cachedNotesJson != null)
                {
                    _dataModel = JsonConvert.DeserializeObject<DataModel>(cachedNotesJson);
                    foreach (var collection in _dataModel.Collections)
                    {
                        foreach (var completedNote in collection.CompletedNotes)
                        {
                            completedNote.NoteCollection = collection;
                        }
                        foreach (var completedNote in collection.NewNotes)
                        {
                            completedNote.NoteCollection = collection;
                        }
                        foreach (var completedNote in collection.DeletedNotes)
                        {
                            completedNote.NoteCollection = collection;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }
    }

}
