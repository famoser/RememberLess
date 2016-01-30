﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.RememberLess.Data.Entities.Communication.Base;

namespace Famoser.RememberLess.Data.Entities.Communication
{
    [DataContract]
    public class NoteResponse : BaseResponse
    {
        [DataMember]
        public List<NoteEntity> Notes { get; set; }
    }
}
