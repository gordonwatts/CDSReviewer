using CDSReviewerCore.Services.CDS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CDSReviewerCoreTest.Services.CDS
{
    [TestClass]
    public class CDSSearchStringParserTest
    {
        /// <summary>
        /// Get ID from a http url
        /// </summary>
        [TestMethod]
        public async Task cdsURL1()
        {
            string uid = "https://cds.cern.ch/record/1512932?";
            var schr = new CDSSearchStringParser();
            var all = await schr.GetPaperFinders(uid).ToList().FirstAsync();
            Assert.AreEqual(1, all.Count);
            var s = all[0] as CDSPaperSearch;
            Assert.IsNotNull(s);
            Assert.AreEqual(1512932, s.ID);
        }

        [TestMethod]
        public async Task cdsURL2()
        {
            string uid = "https://cds.cern.ch/record/1512932/comments";
            var schr = new CDSSearchStringParser();
            var all = await schr.GetPaperFinders(uid).ToList().FirstAsync();
            Assert.AreEqual(1, all.Count);
            var s = all[0] as CDSPaperSearch;
            Assert.IsNotNull(s);
            Assert.AreEqual(1512932, s.ID);
        }

        [TestMethod]
        public async Task cdsID()
        {
            string uid = "1512932";
            var schr = new CDSSearchStringParser();
            var all = await schr.GetPaperFinders(uid).ToList().FirstAsync();
            Assert.AreEqual(1, all.Count);
            var s = all[0] as CDSPaperSearch;
            Assert.IsNotNull(s);
            Assert.AreEqual(1512932, s.ID);
        }

        [TestMethod]
        public async Task cdsBadID()
        {
            string uid = "15123a2C";
            var schr = new CDSSearchStringParser();
            var all = await schr.GetPaperFinders(uid).ToList().FirstAsync();
            Assert.AreEqual(0, all.Count);
        }

        [TestMethod]
        public async Task cdsBadURL()
        {
            string uid = "https://indico.cern.ch/categoryDisplay.py?categId=3285";
            var schr = new CDSSearchStringParser();
            var all = await schr.GetPaperFinders(uid).ToList().FirstAsync();
            Assert.AreEqual(0, all.Count);
        }
    }
}
