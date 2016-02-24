using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.RememberLess.Business.Models
{
    public class DataModel
    {
        public DataModel()
        {
            Collections = new ObservableCollection<NoteCollectionModel>();
            DeletedCollections = new ObservableCollection<NoteCollectionModel>();
        }

        public ObservableCollection<NoteCollectionModel> Collections { get; set; }
        public ObservableCollection<NoteCollectionModel> DeletedCollections { get; set; }
    }
}
