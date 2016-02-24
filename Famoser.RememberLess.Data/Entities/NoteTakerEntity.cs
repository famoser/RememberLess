using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.RememberLess.Data.Entities
{
    [DataContract]
    public class NoteTakerEntity
    {
        [DataMember]
        public Guid Guid;
    }
}
