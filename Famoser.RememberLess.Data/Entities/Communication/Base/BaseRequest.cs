using System;
using System.Runtime.Serialization;
using Famoser.FrameworkEssentials.Logging;
using Famoser.RememberLess.Data.Enum;

namespace Famoser.RememberLess.Data.Entities.Communication.Base
{
    [DataContract]
    public class BaseRequest
    {
        public BaseRequest(PossibleActions action, Guid guid)
        {
            _possibleAction = action;
            _guid = guid;
        }

        private PossibleActions _possibleAction;
        private Guid _guid;

        [DataMember]
        public Guid Guid { get { return _guid; } }
        [DataMember]
        public string Action
        {
            get
            {
                if (_possibleAction == PossibleActions.Add)
                    return "add";
                if (_possibleAction == PossibleActions.Remove)
                    return "remove";
                if (_possibleAction == PossibleActions.AddOrUpdate)
                    return "addorupdate";
                if (_possibleAction == PossibleActions.Sync)
                    return "sync";
                LogHelper.Instance.Log(LogLevel.WtfAreYouDoingError, this, "Unknown Possible Action used!");
                return "";
            }
        }
    }
}
