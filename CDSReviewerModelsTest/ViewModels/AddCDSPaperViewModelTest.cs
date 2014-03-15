using Caliburn.Micro.Portable;
using CDSReviewerCore.Data;
using CDSReviewerModels.ServiceInterfaces;
using CDSReviewerModels.ViewModels;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI.Testing;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace CDSReviewerModelsTest.ViewModels
{
    [TestClass]
    public class AddCDSPaperViewModelTest
    {
        [TestMethod]
        public void CreateVM()
        {
            INavService obj = Mock.Of<INavService>();
            ISearchStringParser parser = Mock.Of<ISearchStringParser>();
            var vm = new AddCDSPaperViewModel(obj, parser);
        }

        [TestMethod]
        public void SetCDSSearchPropNotify()
        {
            INavService obj = Mock.Of<INavService>();
            ISearchStringParser parser = Mock.Of<ISearchStringParser>();
            var vm = new AddCDSPaperViewModel(obj, parser);
            bool setit = false;
            vm.PropertyChanged += (sender, args) => { setit = args.PropertyName == "CDSLookupString"; };
            vm.CDSLookupString = "hi there";
            Assert.AreEqual("hi there", vm.CDSLookupString);
            Assert.IsTrue(setit, "PropChanged Raised");
        }

        [TestMethod]
        public void FastSimpleCDSSearch()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();
                var paperInfo = Observable.Return(Tuple.Create(new PaperStub() { Title = "title" }, new PaperFullInfo() { Abstract = "abstract", Authors = new string[] { "Authors" } }));
                IPaperSearch search = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(p => p.GetPaperFinders("1234") == Observable.Return(search));

                var vm = new AddCDSPaperViewModel(obj, parser);
                var propChanged = new HashSet<string>();
                vm.PropertyChanged += (sender, args) => { propChanged.Add(args.PropertyName); };

                // Start the search
                vm.CDSLookupString = "1234";

                //shed.AdvanceBy(1000); // All the code should return right away, so, we can just let this go here.

                Assert.AreEqual("title", vm.Title, "searched for title");
                Assert.AreEqual("abstract", vm.Abstract, "searched for abstract");
                Assert.AreEqual("Authors", vm.Authors, "Searched for authors");

                // Make sure prop changed was called correctly!
                Assert.AreEqual(3, propChanged.Count, "# of properties");
                Assert.IsTrue(propChanged.Contains("Title"), "title");
                Assert.IsTrue(propChanged.Contains("Authors"), "Authors");
                Assert.IsTrue(propChanged.Contains("Abstract"), "Abstract");
            });
        }
    }
}
