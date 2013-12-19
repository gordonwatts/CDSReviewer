using CDSReviewerCore.Raw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
    }
}
