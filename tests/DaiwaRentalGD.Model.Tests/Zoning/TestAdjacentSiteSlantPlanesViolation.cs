using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Model.Zoning;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Model.Tests.Zoning
{
    public class TestAdjacentSiteSlantPlanesViolation
    {
        #region Constructors

        public TestAdjacentSiteSlantPlanesViolation(
            ITestOutputHelper outputHelper
        )
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestSerialization()
        {
            var violation = new AdjacentSiteSlantPlanesViolation
            {
                Site = new Site(),
                Point = new Point(1, 2, 3)
            };

            violation.PropertyEdgeIndices.Add(1);

            var workspace = new JsonWorkspace();
            workspace.Save(violation);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());
            var violationCopy =
                workspaceCopy.Load<AdjacentSiteSlantPlanesViolation>(
                    violation.ItemInfo.Uid
                );

            Assert.Equal(
                violation.Site.ItemInfo.Uid,
                violationCopy.Site.ItemInfo.Uid
            );

            Assert.Equal(violation.Point, violationCopy.Point);

            Assert.Equal(
                violation.PropertyEdgeIndices,
                violationCopy.PropertyEdgeIndices
            );
        }

        #endregion

        #region Properties

        public ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
