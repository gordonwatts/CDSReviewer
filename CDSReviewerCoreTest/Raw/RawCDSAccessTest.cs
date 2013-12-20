using CDSReviewerCore.Raw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

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
        public void DeletedDocumentNumber()
        {
            // 22 - has been deleted
            var ra = new RawCDSAccess();
            var r = ra.GetDocumentMetadata(22);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NotPresentDocumentNumber()
        {
            // 22636207 - not created yet
            var ra = new RawCDSAccess();
            var r = ra.GetDocumentMetadata(22636207);
        }

        [TestMethod]
        public void TestBasicPublicDocumentMetadata()
        {
            // https://cds.cern.ch/record/1636207?ln=en - dijet x-section paper
            var ra = new RawCDSAccess();
            var r = ra.GetDocumentMetadata(1637926);
            Assert.IsNotNull(r);

            int count = 0;
            IDocumentMetadata actual = null;
            bool done = false;
            var sub = r.Subscribe(a => { count++; actual = a; }, () => { done = true; });
            
            while (!done)
                Thread.Sleep(100);

            Assert.AreEqual(1, count, "# of times the observable ran");
            Assert.AreEqual("Geometrical statistics of the vorticity vector and the strain rate tensor in rotating turbulence", actual.Title, "paper title");
        }
    }
}
