using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.RememberLess.Business.Converters;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Business.Repositories.Interfaces;
using Famoser.RememberLess.Data.Enum;
using Famoser.RememberLess.Data.Services;
using Newtonsoft.Json;

namespace Famoser.RememberLess.Business.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly IDataService _dataService;
        private readonly IStorageService _storageService;

        public NoteRepository(IDataService dataService, IStorageService storageService)
        {
            _dataService = dataService;
            _storageService = storageService;
        }

        private UserInformationModel _usrInfo;
        public async Task<List<NoteModel>> GetNotes()
        {
            try
            {
                var cachedNotesJson = await _storageService.GetCachedData();
                if (cachedNotesJson != null)
                {
                    return JsonConvert.DeserializeObject<List<NoteModel>>(cachedNotesJson);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return new List<NoteModel>();
        }

        public async Task<List<NoteModel>> SyncNotes(List<NoteModel> notes)
        {
            try
            {
                var userInfoJson = await _storageService.GetUserInformations();
                if (userInfoJson == null)
                {
                    _usrInfo = new UserInformationModel(Guid.NewGuid());
                    userInfoJson = JsonConvert.SerializeObject(_usrInfo);
                    await _storageService.SetUserInformations(userInfoJson);
                }
                else
                {
                    _usrInfo = JsonConvert.DeserializeObject<UserInformationModel>(userInfoJson);
                }

                //remove deleted & not posted
                var remove = notes.Where(b => b.DeletePending && !b.IsPosted).ToList();
                foreach (var beer in remove)
                {
                    notes.Remove(beer);
                }

                //remove deleted
                var deleted = notes.Where(b => b.DeletePending).ToList();
                if (deleted.Any())
                {
                    var obj = RequestConverter.Instance.ConvertToNoteRequest(_usrInfo.Guid, PossibleActions.Remove, deleted);
                    if ((await _dataService.PostNote(obj)).IsSuccessfull)
                    {
                        foreach (var beer in deleted)
                        {
                            notes.Remove(beer);
                        }
                    }
                }

                //add new
                var add = notes.Where(b => !b.IsPosted).ToList();
                if (add.Any())
                {
                    var obj = RequestConverter.Instance.ConvertToNoteRequest(_usrInfo.Guid, PossibleActions.AddOrUpdate, add);
                    if ((await _dataService.PostNote(obj)).IsSuccessfull)
                    {
                        foreach (var beer in notes)
                        {
                            beer.IsPosted = true;
                        }
                    }
                }

                var orderedBeers = notes.OrderByDescending(b => b.CreateTime);
                var sync = RequestConverter.Instance.ConvertToNoteRequest(_usrInfo.Guid, PossibleActions.Sync, orderedBeers.Take(20).ToList(), orderedBeers.Count());
                var newbeers = await _dataService.PostNote(sync);
                if (newbeers.IsSuccessfull)
                {
                    if (!newbeers.Response)
                    {
                        var allnotes = await _dataService.GetNotes(_usrInfo.Guid);
                        if (allnotes.IsSuccessfull)
                        {
                            var syncNotes = new List<NoteModel>(ResponseConverter.Instance.Convert(allnotes.Notes));
                            foreach (var newbeer in syncNotes)
                            {
                                newbeer.IsPosted = true;
                            }
                            return syncNotes;
                        }
                    }
                }
                return new List<NoteModel>(notes);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return null;
        }

        public List<NoteModel> GetExampleNotes()
        {
            return new List<NoteModel>()
            {
                new NoteModel()
                {
                    Guid = Guid.NewGuid(),
                    Content = "Note 1",
                    CreateTime = DateTime.Now,
                    IsCompleted = false,
                },
                new NoteModel()
                {
                    Guid = Guid.NewGuid(),
                    Content = "Note 2",
                    CreateTime = DateTime.Now,
                    IsCompleted = false,
                },
                new NoteModel()
                {
                    Guid = Guid.NewGuid(),
                    Content = "Note 3",
                    CreateTime = DateTime.Now,
                    IsCompleted = false,
                },
                new NoteModel()
                {
                    Guid = Guid.NewGuid(),
                    Content = "Note 4",
                    CreateTime = DateTime.Now,
                    IsCompleted = true,
                },
                new NoteModel()
                {
                    Guid = Guid.NewGuid(),
                    Content = "Note 5",
                    CreateTime = DateTime.Now,
                    IsCompleted = true,
                },
            };
        }

        public async Task<bool> Save(NoteModel nm, List<NoteModel> notes)
        {
            try
            {
                var successfull = true;
                if (nm.DeletePending)
                {
                    if (nm.IsPosted)
                    {
                        var obj = RequestConverter.Instance.ConvertToNoteRequest(_usrInfo.Guid, PossibleActions.Remove,
                            new List<NoteModel>() { nm });
                        nm.IsPosted = (await _dataService.PostNote(obj)).IsSuccessfull;
                    }
                }
                else
                {

                    var obj = RequestConverter.Instance.ConvertToNoteRequest(_usrInfo.Guid, PossibleActions.AddOrUpdate,
                        new List<NoteModel>() { nm });
                    nm.IsPosted = (await _dataService.PostNote(obj)).IsSuccessfull; ;
                }


                var notesJson = JsonConvert.SerializeObject(notes);
                successfull &= await _storageService.SetCachedData(notesJson);
                return successfull;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }
    }
}
