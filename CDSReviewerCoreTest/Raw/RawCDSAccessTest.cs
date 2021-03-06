﻿using CDSReviewerCore.Raw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
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
        }

        [TestMethod]
        public async Task CatchDeletedDocumentException()
        {
            // 22 - has been deleted
            var r = RawCDSAccess.GetDocumentMetadata(22).Catch(Observable.Empty<IDocumentMetadata>());
            var actual = await r.Count();
            Assert.AreEqual(0, actual);
        }

        /// <summary>
        /// Dummy function to return a string after a search.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private IObservable<string> FindPaper(int id)
        {
            return RawCDSAccess.GetDocumentMetadata(id)
                .Select(x => "hi there");
        }

        [TestMethod]
        public async Task CatchDeletedDocumentExceptionAfterSelect()
        {
            // 22 - has been deleted
            var id = Observable.Return(22);
            var strs = id
                .SelectMany(i => FindPaper(i).Catch(Observable.Empty<string>()));

            var actual = await strs.Count();
            Assert.AreEqual(0, actual);
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

        [TestMethod]
        public async Task FileListFetch()
        {
            // SUSY 3L paper - this is a public ATLAS published paper.
            var r = RawCDSAccess.GetDocumentFiles(1694329);
            Assert.IsNotNull(r);
            var actual = (await r).ToArray();

            Assert.AreEqual(2, actual.Count());
            var p1 = actual[0];
            Assert.AreEqual("SUSY-2013-09_ss3L.pdf", p1.FileName);
            Assert.AreEqual(1, p1.Versions.Length);
            Assert.AreEqual(DateTime.Parse("09 Apr 2014, 13:28"), p1.Versions[0].VersionDate);
            Assert.AreEqual(1, p1.Versions[0].VersionNumber);
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
        public async Task GetPublicFileWithVersion()
        {
            // Get a file associated with a workshop https://cds.cern.ch/record/1007190
            var localFile = new FileInfo(@"GetPublicFileWithVersion.pdf");
            using (var rdr = localFile.Create())
            {
                var r = await RawCDSAccess.SaveDocumentLocally("1007190", "ard-2005-013.pdf", 1, rdr);
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
            public string Abstract { get; set; }
            public string[] Authors { get; set; }
        }


    }
}
