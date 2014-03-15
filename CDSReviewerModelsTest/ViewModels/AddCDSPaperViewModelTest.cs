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

                // Return a single paper
                var paperInfo = Observable.Return(Tuple.Create(new PaperStub() { Title = "title" }, new PaperFullInfo() { Abstract = "abstract", Authors = new string[] { "Authors" } }));
                IPaperSearch search = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(p => p.GetPaperFinders("1234") == Observable.Return(search));

                var vm = new AddCDSPaperViewModel(obj, parser);

                // Watch for property changed events
                var propChanged = new HashSet<string>();
                vm.PropertyChanged += (sender, args) => { propChanged.Add(args.PropertyName); };

                // Initial values shoudl be set. ALso causes the subscription.
                Assert.AreEqual("", vm.Title, "inital title");
                Assert.AreEqual("", vm.Abstract, "initial abstract");
                Assert.AreEqual(0, vm.Authors.Length, "# of initial authors");

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceBy(10);

                // Did the results of the search get through?
                Assert.AreEqual("title", vm.Title, "searched for title");
                Assert.AreEqual("abstract", vm.Abstract, "searched for abstract");
                Assert.AreEqual(1, vm.Authors.Length, "Searched for authors");
                Assert.AreEqual("Authors", vm.Authors[0], "Searched for authors");

                // Make sure prop changed was called correctly!
                Assert.AreEqual(4, propChanged.Count, "# of properties");
                Assert.IsTrue(propChanged.Contains("Title"), "title");
                Assert.IsTrue(propChanged.Contains("Authors"), "Authors");
                Assert.IsTrue(propChanged.Contains("Abstract"), "Abstract");
                Assert.IsTrue(propChanged.Contains("CDSLookupString"), "CDSLookupString");
            });
        }
    }
}
