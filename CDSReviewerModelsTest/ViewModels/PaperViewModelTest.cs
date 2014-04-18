using Caliburn.Micro.Portable;
using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using CDSReviewerModels.ViewModels;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI.Testing;
using System;
using System.Linq;
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

                var pvobj = new PaperViewModel(nav, addr);
                var at = pvobj.PaperTitle;
                var ab = pvobj.Abstract;
                var ath = pvobj.Authors;
                var paps = pvobj.PaperVersions;

                pvobj.PaperID = "1234";

                shed.AdvanceByMs(1);

                Assert.AreEqual("this title", pvobj.PaperTitle);
                Assert.AreEqual("this abstract", pvobj.Abstract);
                Assert.AreEqual(1, pvobj.Authors.Length);
                Assert.AreEqual("this author", pvobj.Authors[0]);
                Assert.AreEqual(1, pvobj.PaperVersions.Count);
                Assert.AreEqual(2, pvobj.PaperVersions.First().Version);
            });
        }

        [TestMethod]
        public void TestFileUpdate()
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
                                }
                        }
                    }
                };

                Assert.Inconclusive();
                var nav = Mock.Of<INavService>();
                var addr = Mock.Of<IInternalPaperDB>(a => a.GetPaperInfoForID("1234") == Task.Factory.StartNew(() => Tuple.Create(ps, psf)));

                var pvobj = new PaperViewModel(nav, addr);
                var paps = pvobj.PaperVersions;

                pvobj.PaperID = "1234";

                shed.AdvanceByMs(1);



                Assert.AreEqual("this title", pvobj.PaperTitle);
                Assert.AreEqual("this abstract", pvobj.Abstract);
                Assert.AreEqual(1, pvobj.Authors.Length);
                Assert.AreEqual("this author", pvobj.Authors[0]);
                Assert.AreEqual(1, pvobj.PaperVersions.Count);
                Assert.AreEqual(2, pvobj.PaperVersions.First().Version);
            });
        }
    }
}
