using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.RememberLess.Data.Entities
{
    [DataContract]
    public class NoteCollectionEntity
    {
        [DataMember]
        public Guid Guid;

        [DataMember]
        public string Name;

        [DataMember]
        public DateTime CreateTime;
    }
}
