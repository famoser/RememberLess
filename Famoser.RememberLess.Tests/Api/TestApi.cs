using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.RememberLess.Business.Converters;
using Famoser.RememberLess.Business.Models;
using Famoser.RememberLess.Data.Entities.Communication;
using Famoser.RememberLess.Data.Enum;
using Famoser.RememberLess.Data.Services;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Famoser.RememberLess.Tests.Api
{
    [TestClass]
    public class TestApi
    {
        [TestMethod]
        public void TestNotes()
        {
            Task.Run(async () =>
            {
                /*
                var note1 = new NoteModel()
                {
                    IsCompleted = false,
                    Content = "hallo welt",
                    CreateTime = DateTime.Now,
                    Guid = Guid.NewGuid()
                };
                var note2 = new NoteModel()
                {
                    IsCompleted = false,
                    Content = "hallo welt 1",
                    CreateTime = DateTime.Today,
                    Guid = Guid.NewGuid()
                };
                var note3 = new NoteModel()
                {
                    IsCompleted = true,
                    Content = "hallo welt 2",
                    CreateTime = DateTime.Today - TimeSpan.FromDays(1),
                    Guid = Guid.NewGuid()
                };
                var ds = new DataService();

                var addRequest = RequestConverter.Instance.ConvertToNoteRequest(ApiTestHelper.TestUserGuid,
                    PossibleActions.Add, new List<NoteModel>() { note1, note2, note3 });
                var remove1 = RequestConverter.Instance.ConvertToNoteRequest(ApiTestHelper.TestUserGuid,
                    PossibleActions.Delete, new List<NoteModel>() { note1 });
                var remove2 = RequestConverter.Instance.ConvertToNoteRequest(ApiTestHelper.TestUserGuid,
                    PossibleActions.Delete, new List<NoteModel>() { note2, note3 });

                //act
                //check if 0 Notes
                var notes = await ds.GetNotes(ApiTestHelper.TestUserGuid);
                ApiAssertHelper.CheckBaseResponse(notes);
                Assert.IsTrue(notes.Notes == null || !notes.Notes.Any());

                //add Notes;
                var res = await ds.PostNote(addRequest);
                ApiAssertHelper.CheckBooleanResponse(res);

                //check if 2 Notes
                notes = await ds.GetNotes(ApiTestHelper.TestUserGuid);
                ApiAssertHelper.CheckBaseResponse(notes);
                Assert.IsTrue(notes.Notes != null && notes.Notes.Count == 3);

                //remove 1 Notes;
                res = await ds.PostNote(remove1);
                ApiAssertHelper.CheckBooleanResponse(res);

                //check if 2 Note, check Date
                notes = await ds.GetNotes(ApiTestHelper.TestUserGuid);
                ApiAssertHelper.CheckBaseResponse(notes);
                Assert.IsTrue(notes.Notes != null && notes.Notes.Count == 2);
                Assert.IsTrue(notes.Notes[0].Guid == note2.Guid);
                Assert.IsTrue(notes.Notes[0].CreateTime - note2.CreateTime < TimeSpan.FromSeconds(1));
                Assert.IsTrue(notes.Notes[1].Guid == note3.Guid);
                Assert.IsTrue(notes.Notes[1].CreateTime - note3.CreateTime < TimeSpan.FromSeconds(1));

                //remove 1 Note left;
                res = await ds.PostNote(remove2);
                ApiAssertHelper.CheckBooleanResponse(res);

                //check if 0 Notes
                notes = await ds.GetNotes(ApiTestHelper.TestUserGuid);
                ApiAssertHelper.CheckBaseResponse(notes);
                Assert.IsTrue(notes.Notes == null || !notes.Notes.Any());
                */

            }).GetAwaiter().GetResult();
        }
    }
}
