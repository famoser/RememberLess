using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.RememberLess.Data.Entities
{
    [DataContract]
    public class NoteEntity
    {
        [DataMember]
        public Guid Guid;

        [DataMember]
        public string Content;

        [DataMember]
        public DateTime CreateTime;

        [DataMember]
        public string IsCompleted
        {
            set
            {
                if (value == "1" || value == "true" || value == "True")
                    IsCompletedBool = true;
            }
            get { return IsCompletedBool.ToString(); }
        }

        public bool IsCompletedBool;
    }
}
