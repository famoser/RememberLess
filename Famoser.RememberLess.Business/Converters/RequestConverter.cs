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
        public NoteRequest ConvertToNoteRequest(Guid guid, PossibleActions action, List<NoteModel> notes, int count = -1)
        {
            return new NoteRequest(action,guid)
            {
                Notes = ConvertAllToNoteEntity(notes),
                ExpectedCount = count
            };
        }

        public List<NoteEntity> ConvertAllToNoteEntity(List<NoteModel> notes)
        {
            return notes.Select(ConvertToNoteEntity).ToList();
        }

        public NoteEntity ConvertToNoteEntity(NoteModel noteModel)
        {
            return new NoteEntity()
            {
                 Guid = noteModel.Guid,
                 Content = noteModel.Content,
                 CreateTime = noteModel.CreateTime,
                 IsCompletedBool = noteModel.IsCompleted
            };
        }
    }
}
