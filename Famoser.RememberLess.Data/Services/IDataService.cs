using System.Threading.Tasks;
using Famoser.RememberLess.Data.Entities.Communication;

namespace Famoser.RememberLess.Data.Services
{
    public interface IDataService
    {
        Task<BooleanResponse> PostNote(NoteRequest request);
        Task<BooleanResponse> PostNoteCollection(NoteCollectionRequest request);
        Task<NoteResponse> GetNotes(NoteRequest request);
        Task<NoteCollectionResponse> GetNoteCollections(NoteCollectionRequest request);
    }
}
