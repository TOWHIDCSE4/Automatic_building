using Xunit;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestGeometryUtils
    {
        [Fact]
        public void TestConvertToMesh()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 0.0)
                }
            );

            Mesh mesh = GeometryUtils.CovnertToMesh(polygon);

            Assert.Equal(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 0.0)
                },
                mesh.Points
            );
            Assert.Equal(
                new[]
                {
                    new[] { 0, 1, 2 }
                },
                mesh.Faces
            );
        }

        [Fact]
        public void TestExtrude_PositiveDistance()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 1.0, 0.0)
                }
            );

            Mesh mesh = GeometryUtils.Extrude(polygon, 3.0);

            Assert.Equal(
                new[]
                {
                    new Point(2.0, 1.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(0.0, 0.0, 0.0),
                    new Point(0.0, 0.0, 3.0),
                    new Point(2.0, 0.0, 3.0),
                    new Point(2.0, 1.0, 3.0)
                },
                mesh.Points
            );
            Assert.Equal(
                new[]
                {
                    new[] { 0, 1, 2 },
                    new[] { 3, 4, 5 },
                    new[] { 1, 0, 5, 4 },
                    new[] { 2, 1, 4, 3 },
                    new[] { 0, 2, 3, 5 }
                },
                mesh.Faces
            );
        }

        [Fact]
        public void TestExtrude_NegativeDistance()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 1.0, 0.0)
                }
            );

            Mesh mesh = GeometryUtils.Extrude(polygon, -3.0);

            Assert.Equal(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 1.0, 0.0),
                    new Point(2.0, 1.0, -3.0),
                    new Point(2.0, 0.0, -3.0),
                    new Point(0.0, 0.0, -3.0)
                },
                mesh.Points
            );
            Assert.Equal(
                new[]
                {
                    new[] { 0, 1, 2 },
                    new[] { 3, 4, 5 },
                    new[] { 1, 0, 5, 4 },
                    new[] { 2, 1, 4, 3 },
                    new[] { 0, 2, 3, 5 }
                },
                mesh.Faces
            );
        }

        [Fact]
        public void TestCreateBoxMesh()
        {
            Mesh boxMesh0 = GeometryUtils.CreateBoxMesh(1.0, 2.0, 3.0, false);

            Assert.Equal(
                new[]
                {
                    new Point(0.0, 2.0, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(0.0, 0.0, 0.0),
                    new Point(0.0, 0.0, 3.0),
                    new Point(1.0, 0.0, 3.0),
                    new Point(1.0, 2.0, 3.0),
                    new Point(0.0, 2.0, 3.0)
                },
                boxMesh0.Points
            );
            Assert.Equal(
                new[]
                {
                    new[] { 0, 1, 2, 3 },
                    new[] { 4, 5, 6, 7 },
                    new[] { 1, 0, 7, 6 },
                    new[] { 2, 1, 6, 5 },
                    new[] { 3, 2, 5, 4 },
                    new[] { 0, 3, 4, 7 }
                },
                boxMesh0.Faces
            );

            Mesh boxMesh1 = GeometryUtils.CreateBoxMesh(1.0, 2.0, 3.0, true);

            Assert.Equal(
                new[]
                {
                    new Point(-0.5, 1.0, -1.5),
                    new Point(0.5, 1.0, -1.5),
                    new Point(0.5, -1.0, -1.5),
                    new Point(-0.5, -1.0, -1.5),
                    new Point(-0.5, -1.0, 1.5),
                    new Point(0.5, -1.0, 1.5),
                    new Point(0.5, 1.0, 1.5),
                    new Point(-0.5, 1.0, 1.5)
                },
                boxMesh1.Points
            );
            Assert.Equal(
                new[]
                {
                    new[] { 0, 1, 2, 3 },
                    new[] { 4, 5, 6, 7 },
                    new[] { 1, 0, 7, 6 },
                    new[] { 2, 1, 6, 5 },
                    new[] { 3, 2, 5, 4 },
                    new[] { 0, 3, 4, 7 }
                },
                boxMesh1.Faces
            );
        }
    }
}
