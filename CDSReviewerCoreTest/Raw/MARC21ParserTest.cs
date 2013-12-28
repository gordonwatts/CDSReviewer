using CDSReviewerCore.Raw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace CDSReviewerCoreTest
{
    [TestClass]
    public class MARC21ParserTest
    {
        /// <summary>
        /// Load up the public paper metadata
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Raw\1636207.xml")]
        public void PubicPaperMD()
        {
            var mdstring = LoadXML(@"1636207.xml");
            var r = MARC21Parser.ParseForMetadata(mdstring);
            Assert.AreEqual("Measurement of dijet cross sections in pp collisions at 7 TeV centre−of−mass energy using the ATLAS detector", r.Title, "paper title");
            Assert.IsTrue(r.Abstract.StartsWith("Double-differential dijet cross sections measured"), "Abstract");
            Assert.IsTrue(r.Authors.Contains("Gumpert, Christian"), "Authors");
            Assert.AreEqual(2938, r.Authors.Length, "# of authors");
        }

        [TestMethod]
        [DeploymentItem(@"Raw\22.xml")]
        [ExpectedException(typeof(CDSReviewerCore.Raw.MARC21Parser.CDSException))]
        public void DeletedRecord()
        {
            var mdstring = LoadXML(@"22.xml");
            var r = MARC21Parser.ParseForMetadata(mdstring);
        }

        /// <summary>
        /// Helper method to read a file to its end.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string LoadXML(string file)
        {
            var fi = new FileInfo(file);
            Assert.IsTrue(fi.Exists, string.Format("File {0} does not exist.", fi.FullName));

            using (var reader = fi.OpenText())
            {
                return reader.ReadToEnd();
            }
        }

#if false
        // Do these tests when we get files up and running.

        /// <summary>
        /// Get the PDF from an external paper (like the archive).
        /// </summary>
        [TestMethod]
        public void PaperExternalPDF()
        {
            Assert.Inconclusive();
        }

        /// <summary>
        /// Make sure we can parse https://cds.cern.ch/record/1007190 - it has two items
        /// for download, one of which is a PDF and the other of which is something "else".
        /// </summary>
        [TestMethod]
        public void ReportWithTwoEntries()
        {
            Assert.Inconclusive();
        }

        /// <summary>
        /// The document has multiple versions that can be downloaded. Make sure they are in there right.
        /// </summary>
        [TestMethod]
        public void DocumentWithMultipleVersions()
        {
            Assert.Inconclusive();
        }

        /// <summary>
        /// https://cds.cern.ch/record/1639578 is a paper for review, and has
        /// an external link that is for comments, and the full text (that we want to
        /// review) is in a funny spot.
        /// </summary>
        [TestMethod]
        public void DocumentWithCommentExternalLink()
        {
            Assert.Inconclusive();
        }

        /// <summary>
        /// https://cds.cern.ch/record/1638366 has an even other different style
        /// of links.
        /// </summary>
        [TestMethod]
        public void FullPaperWithOtherLinks()
        {
            Assert.Inconclusive();
        }
#endif
    }
}
