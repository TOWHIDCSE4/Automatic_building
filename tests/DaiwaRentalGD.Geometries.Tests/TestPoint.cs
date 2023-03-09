using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestPoint
    {
        #region Constructors

        public TestPoint(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConstructor_Default()
        {
            Point point = new Point();

            Assert.Equal(0.0, point.X);
            Assert.Equal(0.0, point.Y);
            Assert.Equal(0.0, point.Z);

            Assert.Equal(
                new DenseVector(new[] { 0.0, 0.0, 0.0 }),
                point.Vector
            );
        }

        [Fact]
        public void TestConstructor_WithCoordinates()
        {
            Point point = new Point(1.2, 3.4, 5.6);

            Assert.Equal(1.2, point.X);
            Assert.Equal(3.4, point.Y);
            Assert.Equal(5.6, point.Z);

            Assert.Equal(
                new DenseVector(new[] { 1.2, 3.4, 5.6 }),
                point.Vector
            );
        }

        [Fact]
        public void TestConstructor_WithVector()
        {
            Vector<double> vector = new DenseVector(new[] { 1.2, 3.4, 5.6 });

            Point point = new Point(vector);

            Assert.Equal(1.2, point.X);
            Assert.Equal(3.4, point.Y);
            Assert.Equal(5.6, point.Z);
        }

        [Fact]
        public void TestConstructor_Copy()
        {
            Point point0 = new Point(1.2, 3.4, 5.6);
            Point point1 = new Point(point0);

            Assert.Equal(1.2, point1.X);
            Assert.Equal(3.4, point1.Y);
            Assert.Equal(5.6, point1.Z);

            Assert.Equal(
                new DenseVector(new[] { 1.2, 3.4, 5.6 }),
                point1.Vector
            );

            point1.X = 100.0;
            Assert.Equal(1.2, point0.X);
        }

        [Fact]
        public void TestTransform()
        {
            const int DecimalPlaces = 6;

            Point point = new Point(1.2, 3.4, 5.6);

            ITransform3D tf = new TrsTransform3D
            {
                Tx = 1.0,
                Rz = Math.PI / 2.0,
                Sx = 5.0
            };

            point.Transform(tf);

            Assert.Equal(-2.4, point.X, DecimalPlaces);
            Assert.Equal(6.0, point.Y, DecimalPlaces);
            Assert.Equal(5.6, point.Z, DecimalPlaces);
        }

        [Fact]
        public void TestGetDistance()
        {
            const int DecimalPlaces = 6;

            Point point0 = new Point(1.0, 2.0, 3.0);
            Point point1 = new Point(4.0, 6.0, 3.0);

            Assert.Equal(5.0, point0.GetDistance(point1), DecimalPlaces);
            Assert.Equal(5.0, point1.GetDistance(point0), DecimalPlaces);
            Assert.Equal(0.0, point0.GetDistance(point0));
        }

        [Fact]
        public void TestGetBBox()
        {
            Point point = new Point(1.2, 3.4, 5.6);

            BBox bbox = point.GetBBox();

            Assert.Equal(1.2, bbox.MinX);
            Assert.Equal(1.2, bbox.MaxX);

            Assert.Equal(3.4, bbox.MinY);
            Assert.Equal(3.4, bbox.MaxY);

            Assert.Equal(5.6, bbox.MinZ);
            Assert.Equal(5.6, bbox.MaxZ);
        }

        [Theory]
        [InlineData(0.0, 0.0, 0.0, 0.1, 0.2, 0.3, false)]
        [InlineData(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, true)]
        [InlineData(0.1, 2.3, 4.5, 0.1, 2.3, 4.5, true)]
        [InlineData(0.1, 2.3, 4.5, 0.1, 2.3, 4.51, false)]
        public void TestGetHashCodeAndEqualsAndOps(
            double x0, double y0, double z0,
            double x1, double y1, double z1,
            bool equals
        )
        {
            Point point0 = new Point(x0, y0, z0);
            Point point1 = new Point(x1, y1, z1);

            Assert.Equal(point0, point0);
            Assert.Equal(point0.GetHashCode(), point0.GetHashCode());
            Assert.NotNull(point0);

            Assert.True(point0 == point0);
            Assert.False(point0 == null);
            Assert.False(null == point0);

            Assert.False(point0 != point0);
            Assert.True(point0 != null);
            Assert.True(null != point0);

            if (equals)
            {
                Assert.Equal(point0, point1);
                Assert.Equal(point1, point0);
                Assert.Equal(point0.GetHashCode(), point1.GetHashCode());
                Assert.True(point0 == point1);
                Assert.True(point1 == point0);
                Assert.False(point0 != point1);
                Assert.False(point1 != point0);
            }
            else
            {
                Assert.NotEqual(point0, point1);
                Assert.NotEqual(point1, point0);
                Assert.NotEqual(point0.GetHashCode(), point1.GetHashCode());
                Assert.False(point0 == point1);
                Assert.False(point1 == point0);
                Assert.True(point0 != point1);
                Assert.True(point1 != point0);
            }
        }

        [Fact]
        public void TestVector()
        {
            Point point = new Point(1.2, 3.4, 5.6);

            Assert.Equal(
                new DenseVector(new[] { 1.2, 3.4, 5.6 }),
                point.Vector
            );

            point.Vector = new DenseVector(new[] { 1.0, 10.0, 100.0 });

            Assert.Equal(1.0, point.X);
            Assert.Equal(10.0, point.Y);
            Assert.Equal(100.0, point.Z);

            Assert.Equal(
                new DenseVector(new[] { 1.0, 10.0, 100.0 }),
                point.Vector
            );
        }

        [Fact]
        public void TestSerialization()
        {
            var point = new Point(1.0, 2.0, 3.0);

            var pointItem = new HelperWorkspaceItem<Point>(point);

            var workspace = new JsonWorkspace();
            workspace.Save(pointItem);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedItem =
                workspaceCopy.Load<HelperWorkspaceItem<Point>>(
                    pointItem.ItemInfo.Uid
                );

            Assert.Equal(point, deserializedItem.Data);
        }

        #endregion

        #region Properties

        public ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
