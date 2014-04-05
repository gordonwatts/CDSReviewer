using Caliburn.Micro.Portable;
using CDSReviewerCore.Data;
using CDSReviewerCore.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CDSReviewerModelsTest.ViewModels
{
    [TestClass]
    public class PaperTileViewModelTest
    {
        [TestMethod]
        public void TitleSet()
        {
            var nav = Mock.Of<INavService>();
            var basicInfo = new PaperStub() { ID = "1234", Title = "paper title" };
            var fullInfo = new PaperFullInfo();

            var tile = new PaperTileViewModel(nav, basicInfo, fullInfo);

            Assert.AreEqual("paper title", tile.Title);

        }
    }
}
