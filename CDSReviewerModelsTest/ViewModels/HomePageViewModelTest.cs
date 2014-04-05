using Caliburn.Micro.Portable;
using CDSReviewewrModels.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CDSReviewerModelsTest.ViewModels
{
    [TestClass]
    public class HomePageViewModelTest
    {
        [TestMethod]
        public void TestCTor()
        {
            INavService obj = Mock.Of<INavService>();
            var h = new HomePageViewModel(obj);
        }

        [TestMethod]
        public void TestAddButtonHit()
        {
            var moqObj = new Mock<INavService>(MockBehavior.Loose);

            var h = new HomePageViewModel(moqObj.Object);
            h.CmdAdd();

            moqObj.Verify(n => n.NavigateToViewModel<AddCDSPaperViewModel>());
        }
    }
}
