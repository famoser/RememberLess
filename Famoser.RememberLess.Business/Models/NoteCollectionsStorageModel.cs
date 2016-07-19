using System.Collections.ObjectModel;

namespace Famoser.RememberLess.Business.Models
{
    public class NoteCollectionsStorageModel
    {
        public NoteCollectionsStorageModel()
        {
            Collections = new ObservableCollection<NoteCollectionModel>();
            DeletedCollections = new ObservableCollection<NoteCollectionModel>();
        }

        public ObservableCollection<NoteCollectionModel> Collections { get; set; }
        public ObservableCollection<NoteCollectionModel> DeletedCollections { get; set; }
    }
}
