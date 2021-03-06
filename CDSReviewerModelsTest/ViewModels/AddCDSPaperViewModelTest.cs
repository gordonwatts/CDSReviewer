﻿using Caliburn.Micro.Portable;
using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using CDSReviewerCore.ServiceInterfaces;
using CDSReviewerModels.ViewModels;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI;
using ReactiveUI.Testing;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

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
            var adder = Mock.Of<IInternalPaperDB>();
            var vm = new AddCDSPaperViewModel(obj, parser, adder);
        }

        [TestMethod]
        public void SetCDSSearchPropNotify()
        {
            INavService obj = Mock.Of<INavService>();
            ISearchStringParser parser = Mock.Of<ISearchStringParser>();
            var adder = Mock.Of<IInternalPaperDB>();
            var vm = new AddCDSPaperViewModel(obj, parser, adder);
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
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Watch for property changed events
                var propChanged = new HashSet<string>();
                vm.PropertyChanged += (sender, args) => { propChanged.Add(args.PropertyName); };

                // Initial values shoudl be set. ALso causes the subscription.
                Assert.AreEqual("", vm.PaperTitle, "inital title");
                Assert.AreEqual("", vm.Abstract, "initial abstract");
                Assert.AreEqual(0, vm.Authors.Length, "# of initial authors");
                Assert.IsFalse(vm.SearchInProgress, "Search spec");

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(50000);

                // Did the results of the search get through?
                Assert.IsFalse(vm.SearchInProgress, "Search spec");
                Assert.AreEqual("title", vm.PaperTitle, "searched for title");
                Assert.AreEqual("abstract", vm.Abstract, "searched for abstract");
                Assert.AreEqual(1, vm.Authors.Length, "Searched for authors");
                Assert.AreEqual("Authors", vm.Authors[0], "Searched for authors");

                // Make sure prop changed was called correctly!
                Assert.AreEqual(5, propChanged.Count, "# of properties");
                Assert.IsTrue(propChanged.Contains("PaperTitle"), "title");
                Assert.IsTrue(propChanged.Contains("Authors"), "Authors");
                Assert.IsTrue(propChanged.Contains("Abstract"), "Abstract");
                Assert.IsTrue(propChanged.Contains("CDSLookupString"), "CDSLookupString");
                Assert.IsTrue(propChanged.Contains("SearchInProgress"), "SearchInProgress");
            });
        }

        /// <summary>
        /// When a good search completes all the items are updated (like Title). When a second search starts, they should
        /// all be cleared.
        /// </summary>
        [TestMethod]
        public void GoodSearchFollowedByBad()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                // Return a single paper
                var paperInfo = Observable.Return(Tuple.Create(new PaperStub() { Title = "title" }, new PaperFullInfo() { Abstract = "abstract", Authors = new string[] { "Authors" } }));
                var nullPaperInfo = Observable.Empty<Tuple<PaperStub, PaperFullInfo>>();
                IPaperSearch search = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo);
                IPaperSearch nullSearch = Mock.Of<IPaperSearch>(s => s.FindPaper() == nullPaperInfo);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(p =>
                    p.GetPaperFinders("1234") == Observable.Return(search)
                    && p.GetPaperFinders("123") == Observable.Return(nullSearch).Delay(TimeSpan.FromMilliseconds(100), RxApp.TaskpoolScheduler)
                    );
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial values shoudl be set. ALso causes the subscription.
                Assert.AreEqual("", vm.PaperTitle, "inital title");
                Assert.AreEqual("", vm.Abstract, "initial abstract");
                Assert.AreEqual(0, vm.Authors.Length, "# of initial authors");
                Assert.IsFalse(vm.SearchInProgress, "Search spec");

                // Start the search, and let it complete. We'll get good stuff populated.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(6000);

                // Restart the search for something "bad".
                vm.CDSLookupString = "123";

                // Before the search actually starts, nothing should happen.
                shed.AdvanceByMs(450);
                Assert.AreEqual("title", vm.PaperTitle);

                // But, once it has started, then everything bad should happen.
                shed.AdvanceByMs(55);
                Assert.IsTrue(vm.SearchInProgress);
                Assert.AreEqual("", vm.PaperTitle);
                Assert.AreEqual("", vm.Abstract);
                Assert.AreEqual(0, vm.Authors.Length);

                // Make sure, once the search is done, and since it is bad, that things remain empty.
                shed.AdvanceByMs(1000);
                Assert.IsFalse(vm.SearchInProgress);
                Assert.AreEqual("", vm.PaperTitle);
                Assert.AreEqual("", vm.Abstract);
                Assert.AreEqual(0, vm.Authors.Length);
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
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                // No search should be in progress
                Assert.IsFalse(vm.SearchInProgress, "Initial state should be off");

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(551); // Give it a chance to get going!
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
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
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
                var paperInfo1 = Observable.Return(Tuple.Create(new PaperStub() { Title = "title1" }, new PaperFullInfo() { Abstract = "abstract1", Authors = new string[] { "Authors1" } }));
                var paperInfo2 = Observable.Return(Tuple.Create(new PaperStub() { Title = "title2" }, new PaperFullInfo() { Abstract = "abstract2", Authors = new string[] { "Authors2" } }));
                IPaperSearch search1 = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo1);
                IPaperSearch search2 = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo2);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(
                    p => p.GetPaperFinders("1234") == Observable.Return(search1).Delay(TimeSpan.FromSeconds(3), RxApp.TaskpoolScheduler)
                        && p.GetPaperFinders("123") == Observable.Return(search2).Delay(TimeSpan.FromSeconds(3), RxApp.TaskpoolScheduler)
                    );
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
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
        /// When the add a new page it should make an entry in the DB
        /// and navagate to the paper display view model.
        /// </summary>
        [TestMethod]
        public void SearchForNewPaper()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                // Return a single paper
                var paperInfo1 = Observable.Return(Tuple.Create(new PaperStub() { Title = "title1" }, new PaperFullInfo() { Abstract = "abstract1", Authors = new string[] { "Authors1" } }));
                var paperInfo2 = Observable.Return(Tuple.Create(new PaperStub() { Title = "title2" }, new PaperFullInfo() { Abstract = "abstract2", Authors = new string[] { "Authors2" } }));
                IPaperSearch search1 = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo1);
                IPaperSearch search2 = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo2);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(
                    p => p.GetPaperFinders("1234") == Observable.Return(search1).Delay(TimeSpan.FromSeconds(3), RxApp.TaskpoolScheduler)
                        && p.GetPaperFinders("123") == Observable.Return(search2).Delay(TimeSpan.FromSeconds(3), RxApp.TaskpoolScheduler)
                    );
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
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
        /// A paper that has been deleted will cause an exception. So we want to
        /// hide all those that happen...
        /// </summary>
        [TestMethod]
        public void SearchForDeletedPaper()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                var searchMoq = new Mock<IPaperSearch>();
                searchMoq.Setup(ps => ps.FindPaper()).Returns(() => Observable.Throw<Tuple<PaperStub, PaperFullInfo>>(new InvalidOperationException()));

                // Return a single paper
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(
                    p => p.GetPaperFinders("1234") == Observable.Return(searchMoq.Object)
                    );
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(459); // Give it a chance to get going!
                shed.AdvanceByMs(50); // Give it a chance to get going!
                Assert.IsFalse(vm.SearchInProgress, "Once we've passed the start search");
                Assert.AreEqual("", vm.PaperTitle);
            });
        }

        /// <summary>
        /// When a search occurs that is "bad", make sure the add button isn't enabled.
        /// </summary>
        [TestMethod]
        public void AddButtonDisabledAfterExceptionSearch()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                var searchMoq = new Mock<IPaperSearch>();
                searchMoq.Setup(ps => ps.FindPaper()).Returns(() => Observable.Throw<Tuple<PaperStub, PaperFullInfo>>(new InvalidOperationException()));

                // Return a single paper
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(
                    p => p.GetPaperFinders("1234") == Observable.Return(searchMoq.Object)
                    );
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(550); // Give it a chance to get going!
                Assert.IsFalse(vm.AddButtonCommand.CanExecute(true));
            });
        }

        /// <summary>
        /// Search returns no results. Make sure add button doesn't get turned on.
        /// </summary>
        [TestMethod]
        public void AddButtonDisabledAfterNullResultSearch()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                var searchMoq = new Mock<IPaperSearch>();
                searchMoq.Setup(ps => ps.FindPaper()).Returns(() => Observable.Empty<Tuple<PaperStub, PaperFullInfo>>());

                // Return a single paper
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(
                    p => p.GetPaperFinders("1234") == Observable.Return(searchMoq.Object)
                    );
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(10000); // Give it a chance to get going!
                Assert.IsFalse(vm.AddButtonCommand.CanExecute(true));
            });
        }

        /// <summary>
        /// Search returns no results. Make sure add button doesn't get turned on.
        /// </summary>
        [TestMethod]
        public void SearchInProgressFalseAfterSearchDone()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                var searchMoq = new Mock<IPaperSearch>();
                searchMoq.Setup(ps => ps.FindPaper()).Returns(() => Observable.Empty<Tuple<PaperStub, PaperFullInfo>>());

                // Return a single paper
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(
                    p => p.GetPaperFinders("1234") == Observable.Return(searchMoq.Object)
                    );
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(10000); // Give it a chance to get going!
                Assert.IsFalse(vm.SearchInProgress);
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
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
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
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
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

        [TestMethod]
        public void DatabaseUpdateAndPageWithAddButton()
        {
            new TestScheduler().With(shed =>
            {
                var navUriBuilder2 = new Mock<INavUriBuilder<PaperViewModel>>();
                navUriBuilder2.Setup(n => n.Navigate());

                var navUriBuilder1 = new Mock<INavUriBuilder<PaperViewModel>>();
                navUriBuilder1.Setup(n => n.WithParam(x => x.PaperID, "1234")).Returns(navUriBuilder2.Object);

                var navService = new Mock<INavService>();
                navService.Setup(n => n.UriForViewModel<PaperViewModel>()).Returns(navUriBuilder1.Object);

                // Return a single paper
                var pstub = new PaperStub() { Title = "title", ID = "1234" };
                var pstubfull = new PaperFullInfo() { Abstract = "abstract", Authors = new string[] { "Authors" } };
                var paperInfo = Observable.Return(Tuple.Create(pstub, pstubfull));
                IPaperSearch search = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(p => p.GetPaperFinders("1234") == Observable.Return(search));
                var adder = Mock.Of<IInternalPaperDB>(a =>
                    a.Add(pstub, pstubfull) == Task.Factory.StartNew(() => 10)
                    );

                var vm = new AddCDSPaperViewModel(navService.Object, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;
                vm.AddButtonCommand.CanExecute(true);

                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(510); // Give it a chance to get going!
                Assert.IsTrue(vm.AddButtonCommand.CanExecute(false), "Once we've passed the start search");

                // Run the add button
                vm.AddButtonCommand.Execute("dude");
                shed.AdvanceByMs(1);

                // Make sure that the adder object was actually called
                Mock.Get(adder).VerifyAll();
                navService.VerifyAll();
                navUriBuilder1.VerifyAll();
                navUriBuilder2.VerifyAll();
            });
        }

        [TestMethod]
        public void AddButtonEnabledAfterSearchAndThenDisabled()
        {
            new TestScheduler().With(shed =>
            {
                INavService obj = Mock.Of<INavService>();

                // Return a single paper
                var paperInfo = Observable.Return(Tuple.Create(new PaperStub() { Title = "title" }, new PaperFullInfo() { Abstract = "abstract", Authors = new string[] { "Authors" } }));
                IPaperSearch search = Mock.Of<IPaperSearch>(s => s.FindPaper() == paperInfo);
                ISearchStringParser parser = Mock.Of<ISearchStringParser>(p => p.GetPaperFinders("1234") == Observable.Return(search));
                var adder = Mock.Of<IInternalPaperDB>();

                var vm = new AddCDSPaperViewModel(obj, parser, adder);

                // Initial value access to force subscription.
                var t = vm.PaperTitle;
                var ab = vm.Abstract;
                var au = vm.Authors;
                var sip = vm.SearchInProgress;

                Assert.IsFalse(vm.AddButtonCommand.CanExecute(false), "before search string");
                // Start the search, and let it complete.
                vm.CDSLookupString = "1234";
                shed.AdvanceByMs(509); // Give it a chance to get going!
                Assert.IsTrue(vm.AddButtonCommand.CanExecute(false), "Once we've passed the start search");
                // Run a second search, make sure it disables again
                vm.CDSLookupString = "123";
                shed.AdvanceByMs(10);
                Assert.IsTrue(vm.AddButtonCommand.CanExecute(false), "Once we've passed the start search");
            });
        }
    }
}
