using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Famoser.RememberLess.Data.Entities.Communication.Base;

namespace Famoser.RememberLess.Data.Entities.Communication
{
    [DataContract]
    public class NoteCollectionResponse : BaseResponse
    {
        [DataMember]
        public List<NoteCollectionEntity> NoteCollections { get; set; }
    }
}
