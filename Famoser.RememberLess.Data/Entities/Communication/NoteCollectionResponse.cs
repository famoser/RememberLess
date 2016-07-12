using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.RememberLess.Data.Entities.Communication.Base;

namespace Famoser.RememberLess.Data.Entities.Communication
{
    [DataContract]
    public class NoteCollectionResponse : BaseResponse
    {
        public NoteCollectionResponse()
        {
            NoteCollections = new List<NoteCollectionEntity>();
        }

        [DataMember]
        public List<NoteCollectionEntity> NoteCollections { get; set; }
    }
}
