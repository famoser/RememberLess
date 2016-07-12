using System;

namespace Famoser.RememberLess.Business.Models
{
    public class UserInformationModel
    {
        public UserInformationModel(Guid guid)
        {
            Guid = guid;
        }

        public Guid Guid { get; set; }
        public int SaveDataVersion { get; set; }
    }
}
