using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CDSReviewerCore.Raw;
using System.Linq;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace CDSReviewerCoreTest.Raw
{
    [TestClass]
    public class HTMLFileVersionListParserTest
    {
        [TestMethod]
        public void LoadHVFiles()
        {
            var mdstring = LoadXML(@"Raw\hvfiles.html");
            var r = HTMLFileVersionListParser.ParseToFileList(mdstring);
            Assert.IsNotNull(r);
            var l = r.ToArray();
            Assert.AreEqual(1, l.Length, "#of files");
            Assert.AreEqual("ATL-COM-PHYS-2013-113.pdf", l[0].FileName);
            Assert.IsNotNull(l[0].Versions);
            var vs = l[0].Versions.ToArray();
            Assert.AreEqual(8, vs.Length, "# of versions");

            var h = new HashSet<int>();
            foreach (var f in vs.Select(v => v.VersionNumber))
            {
                h.Add(f);
            }
            Assert.AreEqual(8, h.Count, "All versions are different");

            var specV = vs.Where(v => v.VersionNumber == 4).FirstOrDefault();
            Assert.AreEqual(DateTime.Parse("23 Jan 2014, 23:05"), specV.VersionDate, "date parse");
        }

        [TestMethod]
        public void LoadSS3LFiles()
        {
            var mdstring = LoadXML(@"Raw\ss3lfiles.html");
            var r = HTMLFileVersionListParser.ParseToFileList(mdstring);
            Assert.IsNotNull(r);
            var l = r.ToArray();
            Assert.AreEqual(2, l.Length, "#of files");
            Assert.IsNotNull(l[0].Versions);
            var vs = l[0].Versions.ToArray();
            Assert.AreEqual(1, vs.Length, "# of versions");

            var h = new HashSet<int>();
            foreach (var f in vs.Select(v => v.VersionNumber))
            {
                h.Add(f);
            }
            Assert.AreEqual(1, h.Count, "All versions are different");

            var specV = vs.Where(v => v.VersionNumber == 1).FirstOrDefault();
            Assert.AreEqual(DateTime.Parse("9 April 2014, 13:28"), specV.VersionDate, "date parse");
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
