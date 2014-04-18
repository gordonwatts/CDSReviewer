using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CDSReviewerModels.ViewModels;

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
            var f = new PaperFileViewModel("fName", 22, DateTime.Parse("July 1, 2011"));
            Assert.AreEqual("fName", f.FileName);
            Assert.AreEqual(22, f.Version);
            Assert.AreEqual(DateTime.Parse("July 1, 2011"), f.FileDate);
        }
    }
}
