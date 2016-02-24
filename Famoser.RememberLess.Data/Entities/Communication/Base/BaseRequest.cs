using System;
using System.Runtime.Serialization;
using Famoser.FrameworkEssentials.Logging;
using Famoser.RememberLess.Data.Enum;

namespace Famoser.RememberLess.Data.Entities.Communication.Base
{
    [DataContract]
    public class BaseRequest
    {
        public BaseRequest(PossibleActions action, Guid noteTakerGuid)
        {
            _possibleAction = action;
            NoteTakerGuid = noteTakerGuid;
        }

        private readonly PossibleActions _possibleAction;

        [DataMember]
        public Guid NoteTakerGuid { get; }

        [DataMember]
        public string Action
        {
            get
            {
                if (_possibleAction == PossibleActions.Delete)
                    return "delete";
                if (_possibleAction == PossibleActions.AddOrUpdate)
                    return "addorupdate";
                if (_possibleAction == PossibleActions.Get)
                    return "get";
                LogHelper.Instance.Log(LogLevel.WtfAreYouDoingError, this, "Unknown Possible Action used!");
                return "";
            }
        }
    }
}
