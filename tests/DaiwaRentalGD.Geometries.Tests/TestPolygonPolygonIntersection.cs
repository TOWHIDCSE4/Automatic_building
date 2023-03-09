using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestPolygonPolygonIntersection
    {
        #region Constructors

        public TestPolygonPolygonIntersection(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestSerialization()
        {
            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(10.0, 0.0, 0.0),
                    new Point(10.0, 2.0, 0.0),
                    new Point(6.0, 2.0, 0.0),
                    new Point(6.0, 3.0, 0.0),
                    new Point(10.0, 3.0, 0.0),
                    new Point(10.0, 5.0, 0.0),
                    new Point(0.0, 5.0, 0.0)
                }
            );

            Polygon polygon2 = new Polygon(
                new[]
                {
                    new Point(7.0, 1.0, 0.0),
                    new Point(8.0, 1.0, 0.0),
                    new Point(8.0, 4.0, 0.0),
                    new Point(7.0, 4.0, 0.0)
                }
            );

            var intersection = Polygon.EdgeIntersect(polygon0, polygon2);

            var intersectionItem =
                new HelperWorkspaceItem<PolygonPolygonIntersection>(
                    intersection
                );

            var workspace = new JsonWorkspace();
            workspace.Save(intersectionItem);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedItem =
                workspaceCopy.Load
                <HelperWorkspaceItem<PolygonPolygonIntersection>>(
                    intersectionItem.ItemInfo.Uid
                );

            Assert.Equal(intersection, deserializedItem.Data);
        }

        #endregion

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
