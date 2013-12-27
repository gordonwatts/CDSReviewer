using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace CDSReviewerCoreTest.PaperDB
{
    [TestClass]
    public class IsolatedStorageDBTest
    {
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
