using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.RememberLess.Data.Entities.Communication;

namespace Famoser.RememberLess.Data.Services
{
    public interface IDataService
    {
        Task<BooleanResponse> PostNote(NoteRequest obj);
        Task<NoteResponse> GetNotes(Guid guid);
    }
}
