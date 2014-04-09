using Caliburn.Micro.Portable;
using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using CDSReviewerModels.ViewModels;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI.Testing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CDSReviewerModelsTest.ViewModels
{
    [TestClass]
    public class HomePageViewModelTest
    {
        [TestMethod]
        public void TestCTor()
        {
            INavService obj = Mock.Of<INavService>();
            var paperDB = Mock.Of<IInternalPaperDB>();
            var h = new HomePageViewModel(obj, paperDB);
        }

        [TestMethod]
        public void TestAddButtonHit()
        {
            var moqObj = new Mock<INavService>(MockBehavior.Loose);
            var paperDB = Mock.Of<IInternalPaperDB>();

            var h = new HomePageViewModel(moqObj.Object, paperDB);
            h.CmdAdd();

            moqObj.Verify(n => n.NavigateToViewModel<AddCDSPaperViewModel>());
        }

        /// <summary>
        /// Make sure that an empty list of papers shows up as non-null, empty list.
        /// </summary>
        [TestMethod]
        public void SimpleEmptyListOfPapers()
        {
            new TestScheduler().With(sched =>
            {
                var nav = Mock.Of<INavService>();
                var allPapersTask = Task<IEnumerable<Tuple<PaperStub, PaperFullInfo>>>.Factory.StartNew(() => new Tuple<PaperStub, PaperFullInfo>[] { });
                var db = Mock.Of<IInternalPaperDB>(dba => dba.GetFullInformation() == allPapersTask);

                var list = new HomePageViewModel(nav, db);
                sched.AdvanceByMs(1);
                Assert.AreEqual(0, list.PaperList.Count);
            });
        }

        /// <summary>
        /// Make sure that a paper or two show up.
        /// </summary>
        [TestMethod]
        public void SimpleListOfPapers()
        {
            new TestScheduler().With(sched =>
            {
                var nav = Mock.Of<INavService>();
                var allPapersTask = Task<IEnumerable<Tuple<PaperStub, PaperFullInfo>>>.Factory.StartNew(() => new Tuple<PaperStub, PaperFullInfo>[] { Tuple.Create(new PaperStub() { ID = "1234", Title = "hi" }, new PaperFullInfo() { Abstract = "abs", Authors = new string[] { "hi" } }) });
                var db = Mock.Of<IInternalPaperDB>(dba => dba.GetFullInformation() == allPapersTask);

                var list = new HomePageViewModel(nav, db);
                sched.AdvanceByMs(1);
                Assert.AreEqual(1, list.PaperList.Count);
            });
        }
    }
}
