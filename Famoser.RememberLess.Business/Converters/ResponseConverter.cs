using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Singleton;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Data.Entities;

namespace Famoser.RememberLess.Business.Converters
{
    public class ResponseConverter : SingletonBase<ResponseConverter>
    {
        public IEnumerable<NoteModel> Convert(List<NoteEntity> notes)
        {
            return notes.Select(Convert);
        }

        public NoteModel Convert(NoteEntity note)
        {
            return new NoteModel()
            {
                Guid = note.Guid,
                Content = note.Content,
                CreateTime = note.CreateTime,
                IsCompleted = note.IsCompletedBool
            };
        }
    }
}
