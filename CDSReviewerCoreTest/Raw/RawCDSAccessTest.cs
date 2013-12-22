using CDSReviewerCore.Raw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CDSReviewerCoreTest
{
    [TestClass]
    public class RawCDSAccessTest
    {
        /// <summary>
        /// Fail if we request a CDS document number that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DeletedDocumentNumber()
        {
            // 22 - has been deleted
            var ra = new RawCDSAccess();
            var r = ra.GetDocumentMetadata(22);
            var actual = await r;
            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public async Task NotPresentDocumentNumber()
        {
            // 22636207 - not created yet
            var ra = new RawCDSAccess();
            var r = ra.GetDocumentMetadata(22636207);

            var actual = await r;
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task BasicPublicDocumentMetadata()
        {
            // https://cds.cern.ch/record/1637926?ln=en - Weird turbulence paper, that is fast with CDS.
            var ra = new RawCDSAccess();
            var r = ra.GetDocumentMetadata(1637926);
            Assert.IsNotNull(r);

            var actual = await r;

            Assert.AreEqual("Geometrical statistics of the vorticity vector and the strain rate tensor in rotating turbulence", actual.Title, "paper title");
        }

        [TestMethod]
        public async Task CountNumberOfReturnItemsFromMetadataRequest()
        {
            // https://cds.cern.ch/record/1637926?ln=en - Weird turbulence paper, that is fast with CDS.
            var ra = new RawCDSAccess();
            var r = ra.GetDocumentMetadata(1637926);

            var c = await r.Count();
            Assert.AreEqual(1, c, "# of items that came back from the web request");
        }

        [TestMethod]
        public async Task GetPublicFile()
        {
            // Get a file associated with a workshop https://cds.cern.ch/record/1007190
            // https://cds.cern.ch/record/1007190/files/ard-2005-013.pdf

            var ra = new RawCDSAccess();
            var doc = new dummyDoc() { Title = "dude", MainDocument = new Uri(@"https://cds.cern.ch/record/1007190/files/ard-2005-013.pdf") };
            var localFile = new FileInfo(@"GetPublicFile.pdf");
            using (var rdr = localFile.Create())
            {
                var r = await ra.GetMainDocumentHttp(doc, rdr);
                Assert.IsNotNull(r);
                rdr.Close();
            }

            localFile.Refresh();
            Assert.AreEqual(405757, localFile.Length, "Length of downloaded file");
        }

        /// <summary>
        /// Helper class for above calling stuff.
        /// </summary>
        class dummyDoc : IDocumentMetadata
        {
            public string Title { get; set; }
            public Uri MainDocument { get; set; }
        }


    }
}
