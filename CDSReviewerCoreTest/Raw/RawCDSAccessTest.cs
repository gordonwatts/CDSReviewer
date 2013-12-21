using CDSReviewerCore.Raw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task NotPresentDocumentNumber()
        {
            // 22636207 - not created yet
            var ra = new RawCDSAccess();
            var r = ra.GetDocumentMetadata(22636207);

            var actual = await r;
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

    }
}
