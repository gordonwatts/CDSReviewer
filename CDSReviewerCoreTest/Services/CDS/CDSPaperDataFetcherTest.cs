using CDSReviewerCore.Data;
using CDSReviewerCore.Services.CDS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CDSReviewerCoreTest.Services.CDS
{
    [TestClass]
    public class CDSPaperDataFetcherTest
    {
        /// <summary>
        /// Go get a paper that doesn't already exist locally
        /// </summary>
        [TestMethod]
        public async Task DownloadNewPaper()
        {
            var info = CreatePaperInfo();

            var dnldr = new CDSPaperDataFetcher();
            var res = dnldr.DownloadPaper(info.Item1, info.Item2, info.Item3);

            // Now, wait until the paper is totally finished downloading.

            int donePercent = 0;
            while (donePercent != 0)
            {
                Console.WriteLine("{0} - {1}%", DateTime.Now, donePercent);
                donePercent = await res;
            }

            // Now, see if the file has actually been downloaded or not!
            Assert.Inconclusive();
        }

        /// <summary>
        /// Make sure that if we try to redownload this guy we get it ok.
        /// </summary>
        [TestMethod]
        public void ReDownloadPaper()
        {
            Assert.Inconclusive();
        }

        Tuple<PaperStub, PaperFile, PaperFileVersion> CreatePaperInfo()
        {
            var paperStub = new PaperStub() { ID = "1706342", Title = "hi" };
            var paperFile = new PaperFile() { FileName = "arXiv%3A1406.1122.pdf" };
            var paperVersion = new PaperFileVersion() { VersionNumber = 1 };

            return Tuple.Create(paperStub, paperFile, paperVersion);
        }
    }
}
