using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.RememberLess.Business.Models;

namespace Famoser.RememberLess.Business.Repositories.Interfaces
{
    public interface INoteRepository
    {
        Task<ObservableCollection<NoteCollectionModel>> GetCollections();
        ObservableCollection<NoteCollectionModel> GetExampleCollections();
        Task<bool> SyncNotes();

        Task<bool> Save(NoteModel nm);
        Task<bool> Delete(NoteModel nm);
    }
}
