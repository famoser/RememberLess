using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Business.Repositories.Interfaces;

namespace Famoser.RememberLess.Business.Repositories
{
    public class MockNoteRepository : INoteRepository
    {
        public ObservableCollection<NoteCollectionModel> GetCollections()
        {
            return new ObservableCollection<NoteCollectionModel>()
            {
                GetExampleCollection(),
                GetExampleCollection("at home"),
                GetExampleCollection("work")
            };
        }

        public async Task<bool> SyncNotes()
        {
            return true;
        }

        public async Task<bool> Save(NoteModel nm)
        {
            return true;
        }

        public async Task<bool> Delete(NoteModel nm)
        {
            return true;
        }

        public async Task<bool> Save(NoteCollectionModel nm)
        {
            return true;
        }

        public async Task<bool> Delete(NoteCollectionModel nm)
        {
            return true;
        }

        private NoteCollectionModel GetExampleCollection(string collName = "to do")
        {
            return new NoteCollectionModel()
            {
                Name = collName,
                Guid = Guid.NewGuid(),
                NewNotes = new ObservableCollection<NoteModel>()
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
                }
            },
                CompletedNotes = new ObservableCollection<NoteModel>()
                {
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
                    }
                },
                DeletedNotes = new ObservableCollection<NoteModel>()
                {
                    new NoteModel()
                    {
                        Guid = Guid.NewGuid(),
                        Content = "Note 6 (del)",
                        CreateTime = DateTime.Now,
                        IsCompleted = true,
                    },
                    new NoteModel()
                    {
                        Guid = Guid.NewGuid(),
                        Content = "Note 7 (del)",
                        CreateTime = DateTime.Now,
                        IsCompleted = true,
                    }
                },
            };
        }

    }
}
