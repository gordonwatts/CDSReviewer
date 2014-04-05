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
        [TestInitialize]
        public void TestInit()
        {
            //CERNSSOPCL.CERNWebAccess.ResetCredentials();
        }

        /// <summary>
        /// Fail if we request a CDS document number that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CDSReviewerCore.Raw.MARC21Parser.CDSException))]
        public async Task DeletedDocumentNumber()
        {
            // 22 - has been deleted
            var r = RawCDSAccess.GetDocumentMetadata(22);
            var actual = await r;
            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public async Task NotPresentDocumentNumber()
        {
            // 22636207 - not created yet
            var r = RawCDSAccess.GetDocumentMetadata(22636207);

            var actual = await r;
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task BasicPublicDocumentMetadata()
        {
            // https://cds.cern.ch/record/1637926?ln=en - Weird turbulence paper, that is fast with CDS.
            var r = RawCDSAccess.GetDocumentMetadata(1637926);
            Assert.IsNotNull(r);

            var actual = await r;

            Assert.AreEqual("Geometrical statistics of the vorticity vector and the strain rate tensor in rotating turbulence", actual.Title, "paper title");
        }

        /// <summary>
        /// Anyone that needs a username password for testing should call this guy.
        /// This will throw if it can't find the generic credentials for cern.ch.
        /// </summary>
        private void LoadUserPassword()
        {
            var r = WebGenericCredentialsLib.CredAccess.LookupUserPass("cern.ch");
            //CERNSSOPCL.CERNWebAccess.LoadUsernamePassword(r.Item1, r.Item2);
        }

#if false
        // We can't use username/password, so we ignore this for the time being... eventually we will have to get
        // private access working again. Though may not be able to do it with username/password.
        [TestMethod]
        public async Task BasicPrivateDocumentMetadata()
        {
            // https://cds.cern.ch/record/1512932?ln=en - Cal Ratio Internal Note
            LoadUserPassword(); // This is private access, so make sure that goes through.
            var r = RawCDSAccess.GetDocumentMetadata(1512932);
            Assert.IsNotNull(r);

            var actual = await r;

            Assert.AreEqual("Searches for long-lived neutral particles decaying into Heavy Flavors In the Hadronic Calorimeter of ATLAS at sqrt{s} = 8 TeV", actual.Title, "paper title");
        }
#endif

        [TestMethod]
        public async Task CountNumberOfReturnItemsFromMetadataRequest()
        {
            // https://cds.cern.ch/record/1637926?ln=en - Weird turbulence paper, that is fast with CDS.
            var r = RawCDSAccess.GetDocumentMetadata(1637926);

            var c = await r.Count();
            Assert.AreEqual(1, c, "# of items that came back from the web request");
        }

        [TestMethod]
        public async Task GetPublicFile()
        {
            // Get a file associated with a workshop https://cds.cern.ch/record/1007190
            // https://cds.cern.ch/record/1007190/files/ard-2005-013.pdf

            var doc = new dummyDoc() { Title = "dude", MainDocument = new Uri(@"https://cds.cern.ch/record/1007190/files/ard-2005-013.pdf") };
            var localFile = new FileInfo(@"GetPublicFile.pdf");
            using (var rdr = localFile.Create())
            {
                var r = await RawCDSAccess.GetMainDocumentHttp(doc, rdr);
                Assert.IsNotNull(r);
                rdr.Close();
            }

            localFile.Refresh();
            Assert.AreEqual(405757, localFile.Length, "Length of downloaded file");
        }

        /// <summary>
        /// If you try to access a document that isn't authorized, then what we
        /// get back looks different from everything else (a CMS internal document??).
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task MetadataForUnauthorizedRecord()
        {
            // https://cds.cern.ch/record/1512932?ln=en - Cal Ratio Internal Note
            var r = await RawCDSAccess.GetDocumentMetadata(1512932);
        }

        /// <summary>
        /// Helper class for above calling stuff.
        /// </summary>
        class dummyDoc : IDocumentMetadata
        {
            public string Title { get; set; }
            public Uri MainDocument { get; set; }
            public string Abstract { get; set; }
            public string[] Authors { get; set; }
        }


    }
}
