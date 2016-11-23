using System.Threading.Tasks;
using Famoser.RememberLess.Data.Entities.Communication;

#pragma warning disable 1998
namespace Famoser.RememberLess.Data.Services.Mocks
{
    public class MockDataService : IDataService
    {
        public async Task<BooleanResponse> PostNote(NoteRequest request)
        {
            return new BooleanResponse();
        }

        public async Task<BooleanResponse> PostNoteCollection(NoteCollectionRequest request)
        {
            return new BooleanResponse();
        }

        public async Task<NoteResponse> GetNotes(NoteRequest request)
        {
            return new NoteResponse();
        }

        public async Task<NoteCollectionResponse> GetNoteCollections(NoteCollectionRequest request)
        {
            return new NoteCollectionResponse();
        }
    }
}
