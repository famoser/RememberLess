using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Famoser.RememberLess.Business.Models
{
    public class NoteCollectionModel : SyncModel
    {
        public NoteCollectionModel()
        {
            NewNotes = new ObservableCollection<NoteModel>();
            CompletedNotes = new ObservableCollection<NoteModel>();
            DeletedNotes = new ObservableCollection<NoteModel>();
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private DateTime _createTime;
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { Set(ref _createTime, value); }
        }

        public ObservableCollection<NoteModel> NewNotes { get; set; }
        public ObservableCollection<NoteModel> CompletedNotes { get; set; }
        public ObservableCollection<NoteModel> DeletedNotes { get; set; }
    }
}
