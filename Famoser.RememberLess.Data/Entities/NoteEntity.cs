using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.RememberLess.Data.Entities
{
    public class NoteEntity
    {
        public Guid Guid;
        public string Content;
        public DateTime CreateTime;
        public bool IsCompleted;
    }
}
