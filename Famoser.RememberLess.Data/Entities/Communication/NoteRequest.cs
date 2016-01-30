using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.RememberLess.Data.Entities.Communication.Base;
using Famoser.RememberLess.Data.Enum;

namespace Famoser.RememberLess.Data.Entities.Communication
{
    [DataContract]
    public class NoteRequest : BaseRequest
    {
        public NoteRequest(PossibleActions action, Guid guid) : base(action, guid)
        { }

        [DataMember]
        public List<NoteEntity> Notes { get; set; }

        [DataMember]
        public int ExpectedCount { get; set; }
    }
}
