using Caliburn.Micro.Portable;
using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using CDSReviewerCore.Services;
using CDSReviewerModels.ViewModels;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI;
using ReactiveUI.Testing;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CDSReviewerModelsTest.ViewModels
{
    [TestClass]
    public class PaperViewModelTest
    {
        [TestMethod]
        public void TestSimpleCreation()
        {
            new TestScheduler().With(shed =>
            {

                var ps = new PaperStub() { ID = "1234", Title = "this title" };
                var psf = new PaperFullInfo()
                {
                    Abstract = "this abstract",
                    Authors = new string[] { "this author" },
                    Files = new PaperFile[] { 
                        new PaperFile() {
                             FileName="1.pdf",
                             Versions = new PaperFileVersion[] {
                                 new PaperFileVersion() {
                                      VersionNumber = 1,
                                      VersionDate = DateTime.Now,
                                 },
                                 new PaperFileVersion() {
                                     VersionNumber = 2,
                                     VersionDate = DateTime.Now,
                                 }
                            }
                        }
                    }
                };

                var nav = Mock.Of<INavService>();
                var addr = Mock.Of<IInternalPaperDB>(a => a.GetPaperInfoForID("1234") == Task.Factory.StartNew(() => Tuple.Create(ps, psf)));
                var fetcher = Mock.Of<IPaperFetcher>(f => f.GetPaperFiles("1234") == Observable.Return<PaperFile[]>(psf.Files));
                var fileIO = Mock.Of<IOSFileHandler>();

                var pvobj = new PaperViewModel(nav, addr, fetcher, fileIO);
                var at = pvobj.PaperTitle;
                var ab = pvobj.Abstract;
                var ath = pvobj.Authors;
                var paps = pvobj.PaperVersions;

                pvobj.PaperID = "1234";

                shed.AdvanceByMs(10);

                Assert.AreEqual("this title", pvobj.PaperTitle);
                Assert.AreEqual("this abstract", pvobj.Abstract);
                Assert.AreEqual(1, pvobj.Authors.Length);
                Assert.AreEqual("this author", pvobj.Authors[0]);
                Assert.AreEqual(1, pvobj.PaperVersions.Count);
                Assert.AreEqual(2, pvobj.PaperVersions.First().Version);
            });
        }

        /// <summary>
        /// If the task to generate the files returns null, make sure that we don't bomb out.
        /// </summary>
        [TestMethod]
        public void PaperFileReturnsNull()
        {
            new TestScheduler().With(shed =>
            {

                var ps = new PaperStub() { ID = "1234", Title = "this title" };
                var psf = new PaperFullInfo()
                {
                    Abstract = "this abstract",
                    Authors = new string[] { "this author" },
                    Files = new PaperFile[] { 
                        new PaperFile() {
                             FileName="1.pdf",
                             Versions = new PaperFileVersion[] {
                                 new PaperFileVersion() {
                                      VersionNumber = 1,
                                      VersionDate = DateTime.Now,
                                 },
                                 new PaperFileVersion() {
                                     VersionNumber = 2,
                                     VersionDate = DateTime.Now,
                                 }
                            }
                        }
                    }
                };

                var nav = Mock.Of<INavService>();
                var addr = Mock.Of<IInternalPaperDB>(a => a.GetPaperInfoForID("1234") == Task.Factory.StartNew(() => Tuple.Create(ps, psf)));
                var fetcher = Mock.Of<IPaperFetcher>(f => f.GetPaperFiles("1234") == Observable.Return<PaperFile[]>(null));
                var fileIO = Mock.Of<IOSFileHandler>();

                var pvobj = new PaperViewModel(nav, addr, fetcher, fileIO);
                var at = pvobj.PaperTitle;
                var ab = pvobj.Abstract;
                var ath = pvobj.Authors;
                var paps = pvobj.PaperVersions;

                pvobj.PaperID = "1234";

                shed.AdvanceByMs(1);

                // Make sure # of papers hasn't changed.
                Assert.AreEqual(1, pvobj.PaperVersions.Count);
                Assert.AreEqual(2, pvobj.PaperVersions.First().Version);
            });
        }

        /// <summary>
        /// We update the paper files to have more than one file - make sure it correctly updates our DB!
        /// </summary>
        [TestMethod]
        public void FileAddsNewFileVersion()
        {
            new TestScheduler().With(shed =>
            {
                var ps = new PaperStub() { ID = "1234", Title = "this title" };
                var psf = new PaperFullInfo()
                {
                    Abstract = "this abstract",
                    Authors = new string[] { "this author" },
                    Files = new PaperFile[] { 
                        new PaperFile() {
                             FileName="1.pdf",
                             Versions = new PaperFileVersion[] {
                                 new PaperFileVersion() {
                                      VersionNumber = 1,
                                      VersionDate = DateTime.Now,
                                 },
                                 new PaperFileVersion() {
                                     VersionNumber = 2,
                                     VersionDate = DateTime.Now,
                                 }
                            }
                        }
                    }
                };

                // New one will have a version two to show off.
                var updatedFiles = new PaperFile[] { 
                    new PaperFile() {
                            FileName="1.pdf",
                            Versions = new PaperFileVersion[] {
                                new PaperFileVersion() {
                                    VersionNumber = 1,
                                    VersionDate = DateTime.Now,
                                },
                                new PaperFileVersion() {
                                    VersionNumber = 2,
                                    VersionDate = DateTime.Now,
                                },
                                new PaperFileVersion() {
                                    VersionNumber = 3,
                                    VersionDate = DateTime.Now,
                                }
                        }
                    }
                };

                var nav = Mock.Of<INavService>();

                var addrMock = new Mock<IInternalPaperDB>();
                addrMock.Setup(a => a.GetPaperInfoForID("1234")).Returns(Task.Factory.StartNew(() => Tuple.Create(ps, psf)));
                addrMock.Setup(a => a.Merge("1234", updatedFiles)).Returns(Task.Factory.StartNew(() => true));

                var fetcher = Mock.Of<IPaperFetcher>(a => a.GetPaperFiles("1234") == Observable.Return(updatedFiles));
                var fileIO = Mock.Of<IOSFileHandler>();

                var pvobj = new PaperViewModel(nav, addrMock.Object, fetcher, fileIO);
                var paps = pvobj.PaperVersions;

                pvobj.PaperID = "1234";

                // Allow the update to complete
                shed.AdvanceByMs(1);

                Assert.AreEqual(1, pvobj.PaperVersions.Count);
                Assert.AreEqual(3, pvobj.PaperVersions.First().Version);

                // Check that the database got updated correctly as well.
                addrMock.VerifyAll();
            });
        }

        /// <summary>
        /// See what happens when a file file is added.
        /// </summary>
        [TestMethod]
        public void FileAddsNewFileName()
        {
            new TestScheduler().With(shed =>
            {
                var ps = new PaperStub() { ID = "1234", Title = "this title" };
                var psf = new PaperFullInfo()
                {
                    Abstract = "this abstract",
                    Authors = new string[] { "this author" },
                    Files = new PaperFile[] { 
                        new PaperFile() {
                             FileName="1.pdf",
                             Versions = new PaperFileVersion[] {
                                 new PaperFileVersion() {
                                      VersionNumber = 1,
                                      VersionDate = DateTime.Now,
                                 },
                                 new PaperFileVersion() {
                                     VersionNumber = 2,
                                     VersionDate = DateTime.Now,
                                 }
                            }
                        }
                    }
                };

                // New one will have a version two to show off.
                var updatedFiles = new PaperFile[] { 
                    new PaperFile() {
                            FileName="1.pdf",
                            Versions = new PaperFileVersion[] {
                                new PaperFileVersion() {
                                    VersionNumber = 1,
                                    VersionDate = DateTime.Now,
                                },
                                new PaperFileVersion() {
                                    VersionNumber = 2,
                                    VersionDate = DateTime.Now,
                                },
                        }
                    },
                    new PaperFile() {
                            FileName="2.pdf",
                            Versions = new PaperFileVersion[] {
                                new PaperFileVersion() {
                                    VersionNumber = 1,
                                    VersionDate = DateTime.Now,
                                },
                                new PaperFileVersion() {
                                    VersionNumber = 2,
                                    VersionDate = DateTime.Now,
                                },
                        }
                    }
                };

                var nav = Mock.Of<INavService>();

                var addrMock = new Mock<IInternalPaperDB>();
                addrMock.Setup(a => a.GetPaperInfoForID("1234")).Returns(Task.Factory.StartNew(() => Tuple.Create(ps, psf)));
                addrMock.Setup(a => a.Merge("1234", updatedFiles)).Returns(Task.Factory.StartNew(() => true));

                var fetcher = Mock.Of<IPaperFetcher>(a => a.GetPaperFiles("1234") == Observable.Return(updatedFiles));
                var fileIO = Mock.Of<IOSFileHandler>();

                var pvobj = new PaperViewModel(nav, addrMock.Object, fetcher, fileIO);
                var paps = pvobj.PaperVersions;

                pvobj.PaperID = "1234";

                shed.AdvanceByMs(1);

                Assert.AreEqual(2, pvobj.PaperVersions.Count);

                // Check that the database got updated correctly as well.
                addrMock.VerifyAll();

            });
        }

        /// <summary>
        /// The file isn't downloaded, when they click open it should download
        /// the file.
        /// </summary>
        [TestMethod]
        public void TriggerFileDownload()
        {
            new TestScheduler().With(shed =>
            {

                var ps = new PaperStub() { ID = "1234", Title = "this title" };
                var psf = new PaperFullInfo()
                {
                    Abstract = "this abstract",
                    Authors = new string[] { "this author" },
                    Files = new PaperFile[] { 
                        new PaperFile() {
                             FileName="1.pdf",
                             Versions = new PaperFileVersion[] {
                                 new PaperFileVersion() {
                                      VersionNumber = 1,
                                      VersionDate = DateTime.Now,
                                 },
                                 new PaperFileVersion() {
                                     VersionNumber = 2,
                                     VersionDate = DateTime.Now,
                                 }
                            }
                        }
                    }
                };

                var nav = Mock.Of<INavService>();
                var addr = Mock.Of<IInternalPaperDB>(a => a.GetPaperInfoForID("1234") == Task.Factory.StartNew(() => Tuple.Create(ps, psf)));
                var fetcher = Mock.Of<IPaperFetcher>(f => f.GetPaperFiles("1234") == Observable.Return<PaperFile[]>(psf.Files)
                    && f.DownloadPaper(ps, psf.Files[0], psf.Files[0].Versions[1]) == Observable.Return<int>(1));
                var fileIO = Mock.Of<IOSFileHandler>();

                var pvobj = new PaperViewModel(nav, addr, fetcher, fileIO);
                var paps = pvobj.PaperVersions;

                pvobj.PaperID = "1234";

                shed.AdvanceByMs(1);

                // Check the file isn't marked as already downloaded
                Assert.IsFalse(pvobj.PaperVersions[0].IsDownloaded);

                // Now, trigger the download
                pvobj.OpenPaperVersion.Execute(pvobj.PaperVersions.First());

                // Next, wait for it to complete and then make sure it is marked as downloaded.
                shed.AdvanceBy(1);
                Assert.AreEqual(100, pvobj.PaperVersions[0].DownloadFraction);
                Assert.IsTrue(pvobj.PaperVersions[0].IsDownloaded);

                Mock.Get(fetcher).VerifyAll();
            });
        }

        /// <summary>
        /// The file isn't downloaded, when they click open it should download
        /// the file. The download takes some time, and the fraction downloaded
        /// gets updated appropriately.
        /// </summary>
        [TestMethod]
        public void FiledownloadTakesTime()
        {
            new TestScheduler().With(shed =>
            {

                var ps = new PaperStub() { ID = "1234", Title = "this title" };
                var psf = new PaperFullInfo()
                {
                    Abstract = "this abstract",
                    Authors = new string[] { "this author" },
                    Files = new PaperFile[] { 
                        new PaperFile() {
                             FileName="1.pdf",
                             Versions = new PaperFileVersion[] {
                                 new PaperFileVersion() {
                                      VersionNumber = 1,
                                      VersionDate = DateTime.Now,
                                 },
                                 new PaperFileVersion() {
                                     VersionNumber = 2,
                                     VersionDate = DateTime.Now,
                                 }
                            }
                        }
                    }
                };

                var nav = Mock.Of<INavService>();
                var addr = Mock.Of<IInternalPaperDB>(a => a.GetPaperInfoForID("1234") == Task.Factory.StartNew(() => Tuple.Create(ps, psf)));
                var fetcher = Mock.Of<IPaperFetcher>(f => f.GetPaperFiles("1234") == Observable.Return<PaperFile[]>(psf.Files)
                    && f.DownloadPaper(ps, psf.Files[0], psf.Files[0].Versions[1]) == Observable.Return<int>(15).Concat(Observable.Return<int>(100).Delay(TimeSpan.FromMilliseconds(50), RxApp.TaskpoolScheduler)));
                var fileIO = Mock.Of<IOSFileHandler>();

                var pvobj = new PaperViewModel(nav, addr, fetcher, fileIO);
                var paps = pvobj.PaperVersions;

                pvobj.PaperID = "1234";

                shed.AdvanceByMs(1);

                // Check the file isn't marked as already downloaded
                Assert.IsFalse(pvobj.PaperVersions[0].IsDownloaded);

                // Now, trigger the download
                pvobj.OpenPaperVersion.Execute(pvobj.PaperVersions.First());

                // Get it going a little bit, make sure that the numbers show up as expected.
                shed.AdvanceByMs(10);
                Assert.AreEqual(15, pvobj.PaperVersions[0].DownloadFraction);
                Assert.IsFalse(pvobj.PaperVersions[0].IsDownloaded);

                // Travel to the end and make sure it recovers correctly.
                shed.AdvanceByMs(50);
                Assert.AreEqual(100, pvobj.PaperVersions[0].DownloadFraction);
                Assert.IsTrue(pvobj.PaperVersions[0].IsDownloaded);
            });
        }

        /// <summary>
        /// The file is already downloaded. Open it.
        /// </summary>
        [TestMethod]
        public void OpenFileAlreadyDownloaded()
        {
            new TestScheduler().With(shed =>
            {

                var ps = new PaperStub() { ID = "1234", Title = "this title" };
                var psf = new PaperFullInfo()
                {
                    Abstract = "this abstract",
                    Authors = new string[] { "this author" },
                    Files = new PaperFile[] { 
                        new PaperFile() {
                             FileName="1.pdf",
                             Versions = new PaperFileVersion[] {
                                 new PaperFileVersion() {
                                      VersionNumber = 1,
                                      VersionDate = DateTime.Now,
                                 },
                                 new PaperFileVersion() {
                                     VersionNumber = 2,
                                     VersionDate = DateTime.Now,
                                 }
                            }
                        }
                    }
                };

                var nav = Mock.Of<INavService>();
                var addr = Mock.Of<IInternalPaperDB>(a => a.GetPaperInfoForID("1234") == Task.Factory.StartNew(() => Tuple.Create(ps, psf)));
                var fetcher = Mock.Of<IPaperFetcher>(f => f.GetPaperFiles("1234") == Observable.Return<PaperFile[]>(psf.Files));
                var fileIO = Mock.Of<IOSFileHandler>(f => f.OpenFile() == true);

                var pvobj = new PaperViewModel(nav, addr, fetcher, fileIO);
                var paps = pvobj.PaperVersions;

                pvobj.PaperID = "1234";

                shed.AdvanceByMs(1);

                // Check the file isn't marked as already downloaded
                Assert.IsFalse(pvobj.PaperVersions[0].IsDownloaded);

                // Now, trigger the download
                pvobj.OpenPaperVersion.Execute(pvobj.PaperVersions.First());

                // Next, wait for it to complete and then make sure it is marked as downloaded.
                shed.AdvanceBy(1);

                Mock.Get(fileIO).VerifyAll();
            });
        }

        /// <summary>
        /// When the download fails for some reason, make sure that
        /// the error message makes it out.
        /// </summary>
        [TestMethod]
        public void DownloadFails()
        {
            Assert.Inconclusive();
        }
    }
}
