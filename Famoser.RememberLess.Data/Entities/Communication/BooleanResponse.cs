using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.RememberLess.Data.Entities.Communication.Base;

namespace Famoser.RememberLess.Data.Entities.Communication
{
    public class BooleanResponse : BaseResponse
    {
        public bool Response { get; set; }
    }
}
