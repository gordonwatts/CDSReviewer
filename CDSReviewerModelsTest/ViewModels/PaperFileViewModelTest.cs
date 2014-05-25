using CDSReviewerCore.Data;
using CDSReviewerModels.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CDSReviewerModelsTest.ViewModels
{
    [TestClass]
    public class PaperFileViewModelTest
    {
        /// <summary>
        /// Make sure init works
        /// </summary>
        [TestMethod]
        public void TestInit()
        {
            var pfv = new PaperFileVersion() { VersionDate = DateTime.Parse("July 1, 2011"), VersionNumber = 22 };
            var pf = new PaperFile() { FileName = "fName", Versions = new PaperFileVersion[] { pfv } };
            var f = new PaperFileViewModel(pf, pfv);
            Assert.AreEqual("fName", f.FileName);
            Assert.AreEqual(22, f.Version);
            Assert.AreEqual(DateTime.Parse("July 1, 2011"), f.FileDate);
        }
    }
}
