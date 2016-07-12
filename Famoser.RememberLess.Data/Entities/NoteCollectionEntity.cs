using System;
using System.Runtime.Serialization;

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
