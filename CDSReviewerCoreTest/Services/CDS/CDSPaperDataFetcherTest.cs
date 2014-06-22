using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using CDSReviewerCore.Services.CDS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
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

            var outfile = new FileInfo("DownloadNewPaperResult.pdf");
            using (var fstream = outfile.Create())
            {

                var dnldr = new CDSPaperDataFetcher();
                var db = Mock.Of<IInternalPaperDB>(b =>
                    b.IsFileDownloaded(info.Item1, info.Item2, info.Item3) == Task<bool>.Factory.StartNew(() => false)
                    && b.CreatePaperFile(info.Item1, info.Item2, info.Item3) == Task<Stream>.Factory.StartNew(() => fstream)
                    );
                var res = await dnldr.DownloadPaper(db, info.Item1, info.Item2, info.Item3);
                var v = await res;
                Assert.AreEqual(100, v, "percent complete should be just one number here");
            }

            // Check the file came down ok
            outfile.Refresh();
            Assert.AreEqual(826454, outfile.Length, "Length of file we downloaded");
        }

        /// <summary>
        /// Make sure that if we try to re-download nothing like CreatePaperFile is called!
        /// </summary>
        [TestMethod]
        public async Task ReDownloadPaper()
        {
            var info = CreatePaperInfo();

            var dnldr = new CDSPaperDataFetcher();
            var db = Mock.Of<IInternalPaperDB>(b =>
                b.IsFileDownloaded(info.Item1, info.Item2, info.Item3) == Task<bool>.Factory.StartNew(() => true)
                );
            var res = await dnldr.DownloadPaper(db, info.Item1, info.Item2, info.Item3);
            var v = await res;
            Assert.AreEqual(100, v, "percent complete should be just one number here");

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
