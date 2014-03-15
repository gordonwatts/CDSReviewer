using Caliburn.Micro.Portable;
using CDSReviewerModels.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
        public void SetCDSSearch()
        {
            INavService obj = Mock.Of<INavService>();
            var vm = new AddCDSPaperViewModel(obj);
            bool setit = false;
            vm.PropertyChanged += (sender, args) => { setit = args.PropertyName == "CDSLookupString"; };
            vm.CDSLookupString = "hi there";
            Assert.AreEqual("hi there", vm.CDSLookupString);
            Assert.IsTrue(setit, "PropChanged Raised");
        }
    }
}
