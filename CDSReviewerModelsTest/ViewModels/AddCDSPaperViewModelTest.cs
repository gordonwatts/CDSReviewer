using Caliburn.Micro.Portable;
using CDSReviewerModels.ViewModels;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI.Testing;
using System.Collections.Generic;

namespace CDSReviewerModelsTest.ViewModels
{
    [TestClass]
    public class AddCDSPaperViewModelTest
    {
        [TestMethod]
        public void CreateVM()
        {
            INavService obj = Mock.Of<INavService>();
            var vm = new AddCDSPaperViewModel(obj);
        }

        [TestMethod]
        public void SetCDSSearchPropNotify()
        {
            INavService obj = Mock.Of<INavService>();
            var vm = new AddCDSPaperViewModel(obj);
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
                var vm = new AddCDSPaperViewModel(obj);
                var propChanged = new HashSet<string>();
                vm.PropertyChanged += (sender, args) => { propChanged.Add(args.PropertyName); };

                // Start the search
                vm.CDSLookupString = "1234";

                shed.AdvanceBy(1000); // All the code should return right away, so, we can just let this go here.

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
