using Caliburn.Micro.Portable;
using CDSReviewerModels.ServiceInterfaces;
using CDSReviewerModels.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CDSReviewerModelsTest.ViewModels
{
    [TestClass]
    public class PaperListViewModelTest
    {
        [TestMethod]
        public void SimpleListOfPapers()
        {
            var nav = Mock.Of<INavService>();
            var db = Mock.Of<ILocalDBAccess>();

            var list = new PaperListViewModel(nav, db);

            Assert.Inconclusive();
        }
    }
}
