using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Famoser.RememberLess.Business.Models
{
    public class NoteModel : ObservableObject
    {
        private Guid _guid;
        public Guid Guid
        {
            get { return _guid; }
            set { Set(ref _guid, value); }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { Set(ref _content, value); }
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
        
        public bool DeletePending;
        public bool IsPosted;
    }
}
