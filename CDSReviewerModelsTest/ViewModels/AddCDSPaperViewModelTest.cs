using Caliburn.Micro.Portable;
using CDSReviewerCore.Data;
using CDSReviewerModels.ServiceInterfaces;
using CDSReviewerModels.ViewModels;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI;
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

        /// <summary>
        /// Test simle and straight forward instant search
        /// </summary>
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
                Assert.IsFalse(vm.SearchInProgress, "Search spec");

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(50000);

                // Did the results of the search get through?
                Assert.IsFalse(vm.SearchInProgress, "Search spec");
                Assert.AreEqual("title", vm.Title, "searched for title");
                Assert.AreEqual("abstract", vm.Abstract, "searched for abstract");
                Assert.AreEqual(1, vm.Authors.Length, "Searched for authors");
                Assert.AreEqual("Authors", vm.Authors[0], "Searched for authors");

                // Make sure prop changed was called correctly!
                Assert.AreEqual(5, propChanged.Count, "# of properties");
                Assert.IsTrue(propChanged.Contains("Title"), "title");
                Assert.IsTrue(propChanged.Contains("Authors"), "Authors");
                Assert.IsTrue(propChanged.Contains("Abstract"), "Abstract");
                Assert.IsTrue(propChanged.Contains("CDSLookupString"), "CDSLookupString");
                Assert.IsTrue(propChanged.Contains("SearchInProgress"), "SearchInProgress");
            });
        }

        /// <summary>
        /// Test search busy indicator
        /// </summary>
        [TestMethod]
        public void SearchBusyIndicator()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                // Return a single paper
                var paperInfo = Observable.Return(Tuple.Create(new PaperStub() { Title = "title" }, new PaperFullInfo() { Abstract = "abstract", Authors = new string[] { "Authors" } }));
                IPaperSearch search = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(p => p.GetPaperFinders("1234") == Observable.Return(search).Delay(TimeSpan.FromSeconds(3), RxApp.TaskpoolScheduler));

                var vm = new AddCDSPaperViewModel(obj, parser);

                // Initial value access to force subscription.
                var t = vm.Title;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                // No search should be in progress
                Assert.IsFalse(vm.SearchInProgress, "Initial state should be off");

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(501); // Give it a chance to get going!
                Assert.IsTrue(vm.SearchInProgress, "just after search started");
                shed.AdvanceByMs(2 * 1000 + 500);
                Assert.IsTrue(vm.SearchInProgress, "just after 2.5 seconds in");
                shed.AdvanceByMs(1000);
                Assert.IsFalse(vm.SearchInProgress, "after search is done");
            });
        }

        /// <summary>
        /// Make sure the search starts 500 ms after the stuff is entered a single time
        /// </summary>
        [TestMethod]
        public void DelayedStartSearch()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                // Return a single paper
                var paperInfo = Observable.Return(Tuple.Create(new PaperStub() { Title = "title" }, new PaperFullInfo() { Abstract = "abstract", Authors = new string[] { "Authors" } }));
                IPaperSearch search = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(p => p.GetPaperFinders("1234") == Observable.Return(search).Delay(TimeSpan.FromSeconds(3), RxApp.TaskpoolScheduler));

                var vm = new AddCDSPaperViewModel(obj, parser);

                // Initial value access to force subscription.
                var t = vm.Title;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(459); // Give it a chance to get going!
                Assert.IsFalse(vm.SearchInProgress, "Just before the timeout should hit");
                shed.AdvanceByMs(50); // Give it a chance to get going!
                Assert.IsTrue(vm.SearchInProgress, "Once we've passed the start search");
            });
        }

        /// <summary>
        /// Make sure the search starts 500 ms after the stuff is entered a single time
        /// </summary>
        [TestMethod]
        public void DelayedStartSearchResetWithNewTyping()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                // Return a single paper
                var paperInfo = Observable.Return(Tuple.Create(new PaperStub() { Title = "title" }, new PaperFullInfo() { Abstract = "abstract", Authors = new string[] { "Authors" } }));
                IPaperSearch search = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(p => p.GetPaperFinders("1234") == Observable.Return(search).Delay(TimeSpan.FromSeconds(3), RxApp.TaskpoolScheduler));

                var vm = new AddCDSPaperViewModel(obj, parser);

                // Initial value access to force subscription.
                var t = vm.Title;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(200); // Give it a chance to get going!
                vm.CDSLookupString = "123";
                shed.AdvanceByMs(459); // Give it a chance to get going!
                Assert.IsFalse(vm.SearchInProgress, "Just before the timeout should hit");
                shed.AdvanceByMs(50); // Give it a chance to get going!
                Assert.IsTrue(vm.SearchInProgress, "Once we've passed the start search");
            });
        }

        /// <summary>
        /// Make sure the search starts 500 ms after the stuff is entered a single time
        /// </summary>
        [TestMethod]
        public void DelayedStartSearchResetWithEmptyTyping()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                // Return a single paper
                var paperInfo = Observable.Return(Tuple.Create(new PaperStub() { Title = "title" }, new PaperFullInfo() { Abstract = "abstract", Authors = new string[] { "Authors" } }));
                IPaperSearch search = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(p => p.GetPaperFinders("1234") == Observable.Return(search).Delay(TimeSpan.FromSeconds(3), RxApp.TaskpoolScheduler));

                var vm = new AddCDSPaperViewModel(obj, parser);

                // Initial value access to force subscription.
                var t = vm.Title;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(200); // Give it a chance to get going!
                vm.CDSLookupString = "";
                shed.AdvanceByMs(600); // Give it a chance to get going!
                Assert.IsFalse(vm.SearchInProgress, "Just before the timeout should hit");
            });
        }

        [TestMethod]
        public void AddButtonEnabledAfterSearch()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                // Return a single paper
                var paperInfo = Observable.Return(Tuple.Create(new PaperStub() { Title = "title" }, new PaperFullInfo() { Abstract = "abstract", Authors = new string[] { "Authors" } }));
                IPaperSearch search = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(p => p.GetPaperFinders("1234") == Observable.Return(search));

                var vm = new AddCDSPaperViewModel(obj, parser);

                // Initial value access to force subscription.
                var t = vm.Title;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                Assert.IsFalse(vm.AddButtonCommand.CanExecute(false), "before search string");
                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                Assert.IsFalse(vm.AddButtonCommand.CanExecute(false), "just after search string");
                shed.AdvanceByMs(459); // Give it a chance to get going!
                Assert.IsFalse(vm.AddButtonCommand.CanExecute(false), "Just before the timeout should hit");
                shed.AdvanceByMs(50); // Give it a chance to get going!
                Assert.IsTrue(vm.AddButtonCommand.CanExecute(false), "Once we've passed the start search");
            });
        }
    }
}
