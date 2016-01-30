using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.RememberLess.Data.Entities.Communication.Base
{
    [DataContract]
    public class BaseResponse
    {
        public bool IsSuccessfull
        {
            get { return ErrorMessage == null; }
        }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
