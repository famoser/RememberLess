using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Famoser.RememberLess.Data.Entities.Communication.Base;
using Famoser.RememberLess.Data.Enum;

namespace Famoser.RememberLess.Data.Entities.Communication
{
    public class NoteCollectionRequest :  BaseRequest
    {
        public NoteCollectionRequest(PossibleActions action, Guid noteTakerGuid) : base(action, noteTakerGuid)
        {
        }
        
        [DataMember]
        public List<NoteCollectionEntity> NoteCollections { get; set; }
    }
}
