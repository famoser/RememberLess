using System.Runtime.Serialization;
using Famoser.RememberLess.Data.Entities.Communication.Base;

namespace Famoser.RememberLess.Data.Entities.Communication
{
    [DataContract]
    public class StringReponse : BaseResponse
    {
        [DataMember]
        public string Response { get; set; }
    }
}
