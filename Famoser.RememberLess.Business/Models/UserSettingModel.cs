﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.RememberLess.Business.Models
{
    public class UserInformationModel
    {
        public UserInformationModel(Guid guid)
        {
            Guid = guid;
        }

        public Guid Guid { get; set; }
    }
}
