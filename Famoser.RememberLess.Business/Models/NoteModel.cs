using System;
using Newtonsoft.Json;

namespace Famoser.RememberLess.Business.Models
{
    public class NoteModel : SyncModel
    {
        private string _content;
        public string Content
        {
            get { return _content; }
            set { Set(ref _content, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set { Set(ref _isCompleted, value); }
        }

        private DateTime _createTime;
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { Set(ref _createTime, value); }
        }

        [JsonIgnore]
        public NoteCollectionModel NoteCollection { get; set; }
    }
}
