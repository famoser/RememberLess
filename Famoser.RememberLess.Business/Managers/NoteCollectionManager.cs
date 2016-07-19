using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.RememberLess.Business.Models;

namespace Famoser.RememberLess.Business.Managers
{
    public class NoteCollectionManager
    {
        private static readonly ObservableCollection<NoteCollectionModel> Collections = new ObservableCollection<NoteCollectionModel>();
        private static readonly ObservableCollection<NoteCollectionModel> DeletedCollections = new ObservableCollection<NoteCollectionModel>();

        private static void Relink(NoteCollectionModel model)
        {
            foreach (var completedNote in model.CompletedNotes)
            {
                completedNote.NoteCollection = model;
            }
            foreach (var deletedNote in model.DeletedNotes)
            {
                deletedNote.NoteCollection = model;
            }
            foreach (var noteModel in model.NewNotes)
            {
                noteModel.NoteCollection = model;
            }
        }

        public static void AddNoteCollection(NoteCollectionModel model)
        {
            Relink(model);
            Collections.Add(model);
        }
        public static void TryAddNoteCollection(NoteCollectionModel model)
        {
            if (!Collections.Contains(model))
                AddNoteCollection(model);
        }

        public static void AddDeletedNoteCollection(NoteCollectionModel model)
        {
            Relink(model);
            DeletedCollections.Add(model);
        }

        public static void RemoveFromCollection(NoteCollectionModel model)
        {
            if (Collections.Contains(model))
                Collections.Remove(model);
        }

        public static void RemoveFromDeletedCollection(NoteCollectionModel model)
        {
            if (DeletedCollections.Contains(model))
                DeletedCollections.Remove(model);
        }

        public static ObservableCollection<NoteCollectionModel> GetCollection()
        {
            return Collections;
        }

        public static ObservableCollection<NoteCollectionModel> GetDeletedCollection()
        {
            return DeletedCollections;
        }

        public static NoteCollectionsStorageModel GetStorageModel()
        {
            return new NoteCollectionsStorageModel()
            {
                Collections = Collections,
                DeletedCollections = DeletedCollections
            };
        }
    }
}
