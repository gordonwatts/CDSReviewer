using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CDSReviewerCoreTest.PaperDB
{
    [TestClass]
    public class IsolatedStorageDBTest
    {
        /// <summary>
        /// Reset the database to empty before we run each test.
        /// </summary>
        /// <returns></returns>
        [TestInitialize]
        public void TestInit()
        {
            IsolatedStorageDB.Clear().Wait();
        }

        /// <summary>
        /// Nothing in there should get an empty list.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RetreiveEmptyList()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var all = await paperdb.GetFullInformation();
            Assert.IsNotNull(all, "GetFullInformation result");
            Assert.AreEqual(0, all.Count(), "# of items in empty list");
        }

        /// <summary>
        /// Nothing in there should get an empty stub list.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RetreiveEmptyStubList()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var all = await paperdb.GetStubInformation();
            Assert.IsNotNull(all, "GetFullInformation result");
            Assert.AreEqual(0, all.Count(), "# of items in empty list");
        }

        /// <summary>
        /// When we store a single value, we should be able to get back
        /// the attached full information.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task StoreRetreiveSingleValue()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p = CreatePaperInfo("CDS1234");
            await paperdb.Add(p.Item1, p.Item2);
            var info = await paperdb.GetFullInfoForID(p.Item1.ID);
            Assert.IsNotNull(info, "getting back the item");
            Assert.AreEqual(p.Item2.Abstract, info.Abstract, "Abstract");
        }

        [TestMethod]
        public async Task GetBothBack()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p = CreatePaperInfo("CDS1234");
            await paperdb.Add(p.Item1, p.Item2);
            var info = await paperdb.GetPaperInfoForID(p.Item1.ID);
            Assert.IsNotNull(info, "getting back the item");
            Assert.AreEqual(p.Item2.Abstract, info.Item2.Abstract, "Abstract");
            Assert.AreEqual(p.Item1.ID, info.Item1.ID, "Abstract");
        }

        [TestMethod]
        public async Task RetreiveSingleStubList()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p = CreatePaperInfo("CDS1234");
            await paperdb.Add(p.Item1, p.Item2);

            var info = await paperdb.GetStubInformation();
            Assert.IsNotNull(info, "getting back the item");
            Assert.AreEqual(1, info.Count(), "# of items");
            Assert.AreEqual(p.Item1.ID, info.First().ID, "ID");
            Assert.AreEqual(p.Item1.Title, info.First().Title, "title");
        }

        [TestMethod]
        public async Task RetreiveSingleFullList()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p = CreatePaperInfo("CDS1234");
            await paperdb.Add(p.Item1, p.Item2);

            var info = await paperdb.GetFullInformation();
            Assert.IsNotNull(info, "getting back the item");
            Assert.AreEqual(1, info.Count(), "# of items");
            Assert.AreEqual(p.Item1.ID, info.First().Item1.ID, "ID");
            Assert.AreEqual(p.Item1.Title, info.First().Item1.Title, "title");
            Assert.AreEqual(p.Item2.Abstract, info.First().Item2.Abstract, "abstract");
        }

        [TestMethod]
        public async Task UpdateItemTitle()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p1 = CreatePaperInfo("CDS1234");
            await paperdb.Add(p1.Item1, p1.Item2);

            var p2 = CreatePaperInfo("CDS1234");
            p2.Item1.Title = "ops";
            await paperdb.Add(p2.Item1, p2.Item2);

            var info = await paperdb.GetStubInformation();
            Assert.AreEqual(1, info.Count(), "# of items after update");
            Assert.AreEqual("ops", info.First().Title, "Title");
        }

        [TestMethod]
        public async Task UpdateItemAbstract()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p1 = CreatePaperInfo("CDS1234");
            await paperdb.Add(p1.Item1, p1.Item2);

            var p2 = CreatePaperInfo("CDS1234");
            p2.Item2.Abstract = "ops";
            await paperdb.Add(p2.Item1, p2.Item2);

            var info = await paperdb.GetFullInformation();
            Assert.AreEqual(1, info.Count(), "# of items after update");
            Assert.AreEqual("ops", info.First().Item2.Abstract, "Abstract");
        }

        /// <summary>
        /// Removing an item that isn't there should fail silently.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RemoveItemEmptyList()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            await paperdb.Remove("CDS123444");
            Assert.AreEqual(0, (await paperdb.GetStubInformation()).Count(), "# of items");
        }

        /// <summary>
        /// Removing an item that isn't there shouldn't matter at all.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RemoveItemNotThere()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p1 = CreatePaperInfo("CDS1234");
            await paperdb.Add(p1.Item1, p1.Item2);

            await paperdb.Remove("CDS123444");
            Assert.AreEqual(1, (await paperdb.GetStubInformation()).Count(), "# of items");
        }

        [TestMethod]
        public async Task RemoveItem()
        {

            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p1 = CreatePaperInfo("CDS1234");
            await paperdb.Add(p1.Item1, p1.Item2);

            await paperdb.Remove("CDS1234");
            Assert.AreEqual(0, (await paperdb.GetStubInformation()).Count(), "# of items");
        }

        [TestMethod]
        public async Task ClearList()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p1 = CreatePaperInfo("CDS1234");
            await paperdb.Add(p1.Item1, p1.Item2);
            await IsolatedStorageDB.Clear();

            paperdb = new IsolatedStorageDB();
            Assert.AreEqual(0, (await paperdb.GetStubInformation()).Count(), "# of items");
        }

        /// <summary>
        /// Create a paper for testing use.
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        private Tuple<PaperStub, PaperFullInfo> CreatePaperInfo(string paperID)
        {
            var ps = new PaperStub() { ID = paperID, Title = string.Format("Paper title for {0}.", paperID) };
            var pf = new PaperFullInfo()
            {
                Abstract = string.Format("Abstract of paper for {0}.", paperID),
                Authors = new string[] { "A. Berson", "C. Duderson", "E. Fillerson" }
            };

            return Tuple.Create(ps, pf);
        }
    }
}
