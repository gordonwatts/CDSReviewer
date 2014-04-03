using Caliburn.Micro.Portable;
using CDSReviewerCore.Data;
using CDSReviewerModels.ServiceInterfaces;
using CDSReviewerModels.ViewModels;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI.Testing;
using System;
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
                var psf = new PaperFullInfo() { Abstract = "this abstract", Authors = new string[] { "this author" } };

                var nav = Mock.Of<INavService>();
                var addr = Mock.Of<ILocalDBAccess>(a => a.LookupPaperLocally("1234") == Task.Factory.StartNew(() => Tuple.Create(ps, psf)));

                var pvobj = new PaperViewModel(nav, addr);
                var at = pvobj.Title;
                var ab = pvobj.Abstract;
                var ath = pvobj.Authors;

                pvobj.PaperID = "1234";

                shed.AdvanceByMs(1);

                Assert.AreEqual("this title", pvobj.Title);
                Assert.AreEqual("this abstract", pvobj.Abstract);
                Assert.AreEqual(1, pvobj.Authors.Length);
                Assert.AreEqual("this author", pvobj.Authors[0]);
            });
        }
    }
}
