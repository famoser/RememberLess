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
        Task<List<NoteModel>> GetNotes();
        Task<List<NoteModel>> SyncNotes(List<NoteModel> notes);
        List<NoteModel> GetExampleNotes();

        Task<bool> Save(NoteModel nm, List<NoteModel> notes);
    }
}
