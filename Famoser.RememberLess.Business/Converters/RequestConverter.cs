using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Singleton;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Data.Entities;
using Famoser.RememberLess.Data.Entities.Communication;
using Famoser.RememberLess.Data.Enum;

namespace Famoser.RememberLess.Business.Converters
{
    public class RequestConverter : SingletonBase<RequestConverter>
    {
        public NoteRequest ConvertToNoteRequest(Guid userGuid, Guid collectionGuid, PossibleActions action, IEnumerable<NoteModel> notes)
        {
            return new NoteRequest(action,userGuid)
            {
                Notes = ConvertAllToNoteEntity(notes),
                NoteCollectionGuid = collectionGuid
            };
        }

        private List<NoteEntity> ConvertAllToNoteEntity(IEnumerable<NoteModel> notes)
        {
            return notes.Select(ConvertToNoteEntity).ToList();
        }

        private NoteEntity ConvertToNoteEntity(NoteModel noteModel)
        {
            return new NoteEntity()
            {
                 Guid = noteModel.Guid,
                 Content = noteModel.Content,
                 CreateTime = noteModel.CreateTime,
                 IsCompletedBool = noteModel.IsCompleted
            };
        }

        public NoteCollectionRequest ConvertToNoteCollectionRequest(Guid userGuid, PossibleActions action, IEnumerable<NoteCollectionModel> collections)
        {
            return new NoteCollectionRequest(action, userGuid)
            {
                NoteCollections = ConvertAllToNoteCollectionEntity(collections)
            };
        }

        private List<NoteCollectionEntity> ConvertAllToNoteCollectionEntity(IEnumerable<NoteCollectionModel> collections)
        {
            return collections.Select(ConvertToNoteCollectionEntity).ToList();
        }

        private NoteCollectionEntity ConvertToNoteCollectionEntity(NoteCollectionModel collection)
        {
            return new NoteCollectionEntity()
            {
                Guid = collection.Guid,
                Name = collection.Name,
                CreateTime = collection.CreateTime
            };
        }
    }
}
