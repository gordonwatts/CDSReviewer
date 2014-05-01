using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CDSReviewerModels.ViewModels;
using CDSReviewerCore.Data;

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
            string id = "1234";
            var file = new PaperFile() { FileName = "fName" };
            var version = new PaperFileVersion() { VersionDate = DateTime.Parse("July 1, 2011"), VersionNumber = 22 };
            var f = new PaperFileViewModel(id, file, version);
            Assert.AreEqual("fName", f.FileName);
            Assert.AreEqual(22, f.Version);
            Assert.AreEqual(DateTime.Parse("July 1, 2011"), f.FileDate);
        }
    }
}
