using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Base;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.RememberLess.Business.Converters;
using Famoser.RememberLess.Business.Enums;
using Famoser.RememberLess.Business.Managers;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Business.Repositories.Interfaces;
using Famoser.RememberLess.Data.Enum;
using Famoser.RememberLess.Data.Services;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace Famoser.RememberLess.Business.Repositories
{
    public class NoteRepository : BaseHelper, INoteRepository
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
        private bool _isInitialized;
        private readonly AsyncLock _isInitializedLock = new AsyncLock();
        public ObservableCollection<NoteCollectionModel> GetCollections()
        {
#pragma warning disable 4014
            Initialize();
#pragma warning restore 4014

            return NoteCollectionManager.GetCollection();
        }

        private async Task Initialize()
        {
            using (await _isInitializedLock.LockAsync())
            {
                if (_isInitialized)
                    return;

                _isInitialized = true;

                await ExecuteSafe(async () =>
                {
                    if (await RetrieveUserInformationsFromStorage())
                    {
                        await RetrieveNoteCollectionsFromStorage();
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
                        NoteCollectionManager.AddNoteCollection(coll);
                    }
                });
            }

            await SyncNotes();
        }

        public Task<bool> SyncNotes()
        {
            return ExecuteSafe(async () =>
            {
                await Initialize();

                //add/update/delete collections
                var collPending = NoteCollectionManager.GetCollection().Where(c => c.PendingAction == PendingAction.AddOrUpdate).ToList();
                if (collPending.Any())
                {
                    var addUpdateRequest =
                        RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid,
                            PossibleActions.AddOrUpdate, collPending);
                    var addUpdateRes = (await _dataService.PostNoteCollection(addUpdateRequest)).IsSuccessfull;
                    if (addUpdateRes)
                        foreach (var collModel in collPending)
                            collModel.PendingAction = PendingAction.None;
                }

                //delete
                var collDelete =
                    NoteCollectionManager.GetDeletedCollection()
                        .Where(c => c.PendingAction == PendingAction.Remove)
                        .ToList();
                if (collDelete.Any())
                {
                    var deleteRequest = RequestConverter.Instance.ConvertToNoteCollectionRequest(
                        _userInformations.Guid, PossibleActions.Delete, collDelete);
                    var deleteRes = (await _dataService.PostNoteCollection(deleteRequest)).IsSuccessfull;
                    if (deleteRes)
                        foreach (var collModel in collDelete)
                        {
                            collModel.PendingAction = PendingAction.None;
                            NoteCollectionManager.RemoveFromDeletedCollection(collModel);
                        }
                }

                //sync
                var syncRequest = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid,
                    PossibleActions.Get, new List<NoteCollectionModel>());
                var syncRequestResult = await _dataService.GetNoteCollections(syncRequest);
                if (syncRequestResult.IsSuccessfull)
                {
                    //actualize existing / add new
                    foreach (var collectionEntity in syncRequestResult.NoteCollections)
                    {
                        var existingModel =
                            NoteCollectionManager.GetCollection().FirstOrDefault(n => n.Guid == collectionEntity.Guid);
                        if (existingModel == null)
                        {
                            var newModel = ResponseConverter.Instance.Convert(collectionEntity);
                            NoteCollectionManager.AddNoteCollection(newModel);
                        }
                        else
                        {
                            ResponseConverter.Instance.WriteValues(collectionEntity, existingModel);
                        }
                    }
                    //remove old
                    var old =
                        NoteCollectionManager.GetCollection()
                            .Where(n => syncRequestResult.NoteCollections.All(no => no.Guid != n.Guid))
                            .ToList();
                    foreach (var noteModel in old)
                    {
                        NoteCollectionManager.RemoveFromCollection(noteModel);
                    }
                }


                //add/update/delete all notes
                foreach (var noteCollectionModel in NoteCollectionManager.GetCollection())
                {
                    //add & update
                    var pending =
                        noteCollectionModel.CompletedNotes.Where(n => n.PendingAction == PendingAction.AddOrUpdate)
                            .ToList();
                    var pending2 =
                        noteCollectionModel.NewNotes.Where(n => n.PendingAction == PendingAction.AddOrUpdate).ToList();
                    if (pending.Any() || pending2.Any())
                    {
                        pending.AddRange(pending2);
                        var addUpdateRequest = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid,
                            noteCollectionModel.Guid, PossibleActions.AddOrUpdate, pending);
                        var addUpdateRes = await _dataService.PostNote(addUpdateRequest);
                        if (addUpdateRes.IsSuccessfull)
                            foreach (var noteModel in pending)
                                noteModel.PendingAction = PendingAction.None;
                    }

                    //removes
                    var removes =
                        noteCollectionModel.DeletedNotes.Where(n => n.PendingAction == PendingAction.Remove).ToList();
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
                    var getRequest = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid,
                        noteCollectionModel.Guid, PossibleActions.Get, new List<NoteModel>());
                    var getRequestResult = await _dataService.GetNotes(getRequest);
                    if (getRequestResult.IsSuccessfull)
                    {
                        //actualize existing / add new
                        foreach (var noteEntity in getRequestResult.Notes)
                        {
                            NoteModel existingModel =
                                noteCollectionModel.CompletedNotes.FirstOrDefault(n => n.Guid == noteEntity.Guid) ??
                                noteCollectionModel.NewNotes.FirstOrDefault(n => n.Guid == noteEntity.Guid);
                            if (existingModel == null)
                            {
                                var newModel = ResponseConverter.Instance.Convert(noteEntity);
                                newModel.NoteCollection = noteCollectionModel;
                                InsertIntoList(
                                    !newModel.IsCompleted
                                        ? noteCollectionModel.NewNotes
                                        : noteCollectionModel.CompletedNotes, newModel);
                            }
                            else
                            {
                                ResponseConverter.Instance.WriteValues(noteEntity, existingModel);
                                if (noteEntity.IsCompletedBool != existingModel.IsCompleted)
                                {
                                    if (existingModel.IsCompleted &&
                                        noteCollectionModel.CompletedNotes.Contains(existingModel))
                                        noteCollectionModel.CompletedNotes.Remove(existingModel);
                                    else if (!existingModel.IsCompleted &&
                                             noteCollectionModel.NewNotes.Contains(existingModel))
                                        noteCollectionModel.NewNotes.Remove(existingModel);

                                    InsertIntoList(
                                        !existingModel.IsCompleted
                                            ? noteCollectionModel.NewNotes
                                            : noteCollectionModel.CompletedNotes, existingModel);
                                }
                            }
                        }
                        //remove old
                        var old =
                            noteCollectionModel.CompletedNotes.Where(
                                n => getRequestResult.Notes.All(no => no.Guid != n.Guid)).ToList();
                        foreach (var noteModel in old)
                        {
                            noteCollectionModel.CompletedNotes.Remove(noteModel);
                        }
                        var old2 =
                            noteCollectionModel.NewNotes.Where(n => getRequestResult.Notes.All(no => no.Guid != n.Guid))
                                .ToList();
                        foreach (var noteModel in old2)
                        {
                            noteCollectionModel.NewNotes.Remove(noteModel);
                        }
                    }
                }

                return await SaveNoteCollectionsToStorage();
            });

        }

        public Task<bool> Save(NoteModel nm)
        {
            return ExecuteSafe(async () =>
            {
                await Initialize();

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
            });
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

        public Task<bool> Delete(NoteModel nm)
        {
            return ExecuteSafe(async () =>
            {
                await Initialize();

                var obj = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid, nm.NoteCollection.Guid,
                    PossibleActions.Delete,
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
            });
        }

        public Task<bool> Save(NoteCollectionModel nm)
        {
            return ExecuteSafe(async () =>
            {
                await Initialize();

                var obj = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid,
                    PossibleActions.AddOrUpdate,
                    new List<NoteCollectionModel>() { nm });
                var res = await _dataService.PostNoteCollection(obj);
                nm.PendingAction = !res.IsSuccessfull ? PendingAction.AddOrUpdate : PendingAction.None;

                NoteCollectionManager.TryAddNoteCollection(nm);

                return await SaveNoteCollectionsToStorage() && res.IsSuccessfull;
            });
        }

        public Task<bool> Delete(NoteCollectionModel nm)
        {
            return ExecuteSafe(async () =>
            {
                await Initialize();

                var obj = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid,
                    PossibleActions.Delete,
                    new List<NoteCollectionModel>() { nm });
                var res = await _dataService.PostNoteCollection(obj);
                nm.PendingAction = !res.IsSuccessfull ? PendingAction.Remove : PendingAction.None;

                NoteCollectionManager.RemoveFromCollection(nm);
                if (nm.PendingAction != PendingAction.None)
                    NoteCollectionManager.AddDeletedNoteCollection(nm);

                return await SaveNoteCollectionsToStorage() && res.IsSuccessfull;
            });
        }

        private Task<bool> SaveUserInformationsToStorage()
        {
            var userInformations = JsonConvert.SerializeObject(_userInformations);
            return _storageService.SetRoamingTextFileAsync("user.json", userInformations);
        }

        private async Task<bool> RetrieveUserInformationsFromStorage()
        {
            try
            {
                var userInformations = await _storageService.GetRoamingTextFileAsync("user.json");
                if (userInformations != null)
                {
                    _userInformations = JsonConvert.DeserializeObject<UserInformationModel>(userInformations);
                    /*
                    _userInformations.Guid = Guid.Parse("54071bb7-47ef-4ca9-aa2b-f65fb1ea405b");
                    await SaveUserInformationsToStorage();
                    */
                    if (_userInformations != null)
                        return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }

            _userInformations = new UserInformationModel(Guid.NewGuid()) { SaveDataVersion = ActiveDataVersion };
            return await SaveUserInformationsToStorage();
        }

        private Task<bool> SaveNoteCollectionsToStorage()
        {
            var dm = NoteCollectionManager.GetStorageModel();
            var notesJson = JsonConvert.SerializeObject(dm);
            return _storageService.SetCachedTextFileAsync("data2.json", notesJson);
        }

        private Task<bool> RetrieveNoteCollectionsFromStorage()
        {
            return ExecuteSafe(async () =>
            {
                var cachedNotesJson = await _storageService.GetCachedTextFileAsync("data2.json");
                if (cachedNotesJson != null)
                {
                    var dm = JsonConvert.DeserializeObject<NoteCollectionsStorageModel>(cachedNotesJson);
                    foreach (var collection in dm.Collections)
                    {
                        NoteCollectionManager.AddNoteCollection(collection);
                    }
                    foreach (var noteCollectionModel in dm.DeletedCollections)
                    {
                        NoteCollectionManager.AddDeletedNoteCollection(noteCollectionModel);
                    }
                    return true;
                }
                return false;
            });
        }
    }

}
