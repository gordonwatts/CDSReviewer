using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CDSReviewerModels.ViewModels;
using CDSReviewerCore.Data;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;

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

        [TestMethod]
        public void FileNotThere()
        {
            new TestScheduler().With(shed =>
            {
                string id = "1234";
                var file = new PaperFile() { FileName = "fName" };
                var version = new PaperFileVersion() { VersionDate = DateTime.Parse("July 1, 2011"), VersionNumber = 22 };
                var f = new PaperFileViewModel(id, file, version);

                shed.AdvanceByMs(20);

                Assert.Inconclusive("FIle is not downloaded");
            }
        }

        [TestMethod]
        public void FileDownloadStart()
        {
            Assert.Inconclusive("File is not downloaded, and it was started");
        }

        [TestMethod]
        public void FileDownloaded()
        {
            Assert.Inconclusive("File is already downloaded");
        }

        [TestMethod]
        public void FileDownloadFails()
        {
            Assert.Inconclusive("Flie download fails.");
        }
    }
}
