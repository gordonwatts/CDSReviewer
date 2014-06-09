using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
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
        public async Task GetBothBackWithFileInfo()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p = CreatePaperInfoWithFiles("CDS12345");
            await paperdb.Add(p.Item1, p.Item2);
            var info = await paperdb.GetPaperInfoForID(p.Item1.ID);
            Assert.IsNotNull(info, "getting back the item");
            Assert.AreEqual(p.Item2.Abstract, info.Item2.Abstract, "Abstract");
            Assert.AreEqual(p.Item1.ID, info.Item1.ID, "Abstract");
            Assert.AreEqual(1, p.Item2.Files.Count(), "# of files");
            Assert.AreEqual(2, p.Item2.Files.First().Versions.Count(), "# of versions");
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
        /// Merging to a paper that doesn't exsit should cause an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task MergeToBadPaperID()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            await paperdb.Merge("CDS1234", CreatePaperFileInfo(1, 1));
        }

        /// <summary>
        /// When there is no file information, we should be able
        /// to add a new file list.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MergeToNoNewInformation()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p1 = CreatePaperInfo("CDS1234");
            await paperdb.Add(p1.Item1, p1.Item2);

            await paperdb.Merge("CDS1234", CreatePaperFileInfo(1, 1));

            var fullInfo = await paperdb.GetFullInfoForID("CDS1234");
            Assert.IsNotNull(fullInfo.Files);
            Assert.AreEqual(1, fullInfo.Files.Length);
        }

        /// <summary>
        /// Add normal file info, then merge in new file info.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MergeUpdatedFileInfo()
        {
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            var p1 = CreatePaperInfo("CDS1234");
            await paperdb.Add(p1.Item1, p1.Item2);

            await paperdb.Merge("CDS1234", CreatePaperFileInfo(1, 1));
            await paperdb.Merge("CDS1234", CreatePaperFileInfo(2, 1));

            var fullInfo = await paperdb.GetFullInfoForID("CDS1234");
            Assert.IsNotNull(fullInfo.Files);
            Assert.AreEqual(2, fullInfo.Files.Length);
        }

        /// <summary>
        /// We've not even created a directory yet - this shuld throw.
        /// In code, this should never happen (I hope).
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public async Task MakeSureFileDoesNotExist()
        {
            var p1 = CreatePaperInfoWithFiles("CDS1234");
            IInternalPaperDB paperdb = new IsolatedStorageDB();

            Assert.IsFalse(await paperdb.IsFileDownloaded(p1.Item1, p1.Item2.Files[0], p1.Item2.Files[0].Versions[0]));
        }

        /// <summary>
        /// Create the paper, but the file still isn't in the db.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public async Task MakeSureFileDoesNotExistAfterStubCreation()
        {
            var p1 = CreatePaperInfoWithFiles("CDS1234");
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            await paperdb.Add(p1.Item1, p1.Item2);

            Assert.IsFalse(await paperdb.IsFileDownloaded(p1.Item1, p1.Item2.Files[0], p1.Item2.Files[0].Versions[0]));
        }

        [TestMethod]
        public async Task WriteFile()
        {
            var p1 = CreatePaperInfoWithFiles("CDS1234");
            IInternalPaperDB paperdb = new IsolatedStorageDB();
            await paperdb.Add(p1.Item1, p1.Item2);

            using (var wtr = await paperdb.CreatePaperFile(p1.Item1, p1.Item2.Files[0], p1.Item2.Files[0].Versions[0]))
            {
                var txtwtr = new StreamWriter(wtr);
                await txtwtr.WriteAsync("hi");
                txtwtr.Close();
                txtwtr.Dispose();
            }

            Assert.IsTrue(await paperdb.IsFileDownloaded(p1.Item1, p1.Item2.Files[0], p1.Item2.Files[0].Versions[0]));
        }

        /// <summary>
        /// Some of the storage API's seem to have problems if you try to create a file that already
        /// exists. Make sure we can handle that.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task WriteFileTwice()
        {
            Assert.Inconclusive();
        }

        /// <summary>
        /// Create simple file info
        /// </summary>
        /// <returns></returns>
        private PaperFile[] CreatePaperFileInfo(int nFiles, int nVersions)
        {
            return Enumerable.Range(1, nFiles).Select(findex => CreatePaperFile(findex, nVersions)).ToArray();
        }

        /// <summary>
        /// Create a single paper file.
        /// </summary>
        /// <param name="findex"></param>
        /// <param name="nVersions"></param>
        /// <returns></returns>
        private PaperFile CreatePaperFile(int findex, int nVersions)
        {
            return new PaperFile()
            {
                FileName = string.Format("{0}.pdf", findex),
                Versions = Enumerable.Range(0, nVersions).Select(CreatePaperVersion).ToArray()
            };
        }

        /// <summary>
        /// Create a single paper version.
        /// </summary>
        /// <param name="nVers"></param>
        /// <returns></returns>
        private PaperFileVersion CreatePaperVersion(int nVers)
        {
            return new PaperFileVersion()
            {
                VersionDate = DateTime.Now,
                VersionNumber = nVers
            };
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

        /// <summary>
        /// Create a paper with attached files for testing
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        private Tuple<PaperStub, PaperFullInfo> CreatePaperInfoWithFiles(string paperID)
        {
            var ps = new PaperStub() { ID = paperID, Title = string.Format("Paper title for {0}.", paperID) };
            var pf = new PaperFullInfo()
            {
                Abstract = string.Format("Abstract of paper for {0}.", paperID),
                Authors = new string[] { "A. Berson", "C. Duderson", "E. Fillerson" },
                Files = new PaperFile[] { 
                    new PaperFile() { 
                        FileName="ATL-COM-STAT-2014-223.pdf", 
                        Versions = new PaperFileVersion[] { 
                            new PaperFileVersion() { VersionDate = DateTime.Now, VersionNumber = 1},
                            new PaperFileVersion() { VersionDate = DateTime.Now, VersionNumber = 2}
                        }
                    }
                }
            };

            return Tuple.Create(ps, pf);
        }
    }
}
