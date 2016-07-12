using System;
using System.Runtime.Serialization;

namespace Famoser.RememberLess.Data.Entities
{
    [DataContract]
    public class NoteTakerEntity
    {
        [DataMember]
        public Guid Guid;
    }
}
