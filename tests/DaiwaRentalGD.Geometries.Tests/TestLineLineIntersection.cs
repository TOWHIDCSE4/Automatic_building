using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestLineLineIntersection
    {
        #region Constructors

        public TestLineLineIntersection(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConstructor()
        {
            LineSegment ls0 = new LineSegment(
                new Point(0.0, 0.0, 0.0),
                new Point(10.0, 0.0, 0.0)
            );

            LineSegment ls1 = new LineSegment(
                new Point(4.0, 1.0, 0.0),
                new Point(8.0, -3.0, 0.0)
            );

            var intersection = new LineLineIntersection(
                ls0, ls1, 0.5, 0.25, 1e-5
            );

            Assert.Equal(ls0, intersection.LineSegment0);
            Assert.Equal(ls1, intersection.LineSegment1);
            Assert.Equal(0.5, intersection.Param0);
            Assert.Equal(0.25, intersection.Param1);
            Assert.Equal(1e-5, intersection.Epsilon);
        }

        [Fact]
        public void TestConstructor_InvalidArguments()
        {
            LineSegment ls0 = new LineSegment(
                new Point(0.0, 0.0, 0.0),
                new Point(10.0, 0.0, 0.0)
            );

            LineSegment ls1 = new LineSegment(
                new Point(4.0, 1.0, 0.0),
                new Point(8.0, -3.0, 0.0)
            );

            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    new LineLineIntersection(null, ls1, 0.5, 0.25, 1e-5);
                }
            );
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    new LineLineIntersection(ls0, null, 0.5, 0.25, 1e-5);
                }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    new LineLineIntersection(ls0, ls1, 0.5, 0.25, -1.0);
                }
            );
        }

        [Fact]
        public void TestPoint0Point1()
        {
            const int DecimalPlaces = 6;

            LineSegment ls0 = new LineSegment(
                new Point(0.0, 0.0, 0.0),
                new Point(10.0, 0.0, 0.0)
            );

            LineSegment ls1 = new LineSegment(
                new Point(4.0, 1.0, 0.0),
                new Point(8.0, -3.0, 0.0)
            );

            var intersection = new LineLineIntersection(ls0, ls1, 0.5, 0.25);

            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 5.0, 0.0, 0.0 }),
                intersection.Point0.Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 5.0, 0.0, 0.0 }),
                intersection.Point1.Vector,
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestDoesIntersectEtc()
        {
            const int DecimalPlaces = 6;

            LineSegment ls0 = new LineSegment(
                new Point(0.0, 0.0, 0.0),
                new Point(10.0, 0.0, 0.0)
            );

            LineSegment ls1 = new LineSegment(
                new Point(4.0, 1.0, 1e-6),
                new Point(8.0, -3.0, 1e-6)
            );

            LineSegment ls2 = new LineSegment(
                new Point(4.0, 1.0, 0.0),
                new Point(3.0, 2.0, 0.0)
            );

            LineSegment ls3 = new LineSegment(
                new Point(4.0, 1.0, 2.0),
                new Point(8.0, -3.0, 2.0)
            );

            var intersection01A =
                new LineLineIntersection(ls0, ls1, 0.5, 0.25, 1e-5);

            Assert.Equal(1e-6, intersection01A.Distance, DecimalPlaces);
            Assert.True(intersection01A.DoesIntersect);
            Assert.True(intersection01A.DoesIntersectBetween);

            var intersection01B =
                new LineLineIntersection(ls0, ls1, 0.5, 0.25, 1e-7);

            Assert.Equal(1e-6, intersection01B.Distance, DecimalPlaces);
            Assert.False(intersection01B.DoesIntersect);
            Assert.False(intersection01B.DoesIntersectBetween);

            var intersection02 =
                new LineLineIntersection(ls0, ls2, 0.5, -1.0, 1e-6);

            Assert.Equal(0.0, intersection02.Distance, DecimalPlaces);
            Assert.True(intersection02.DoesIntersect);
            Assert.False(intersection02.DoesIntersectBetween);

            var intersection03 =
                new LineLineIntersection(ls0, ls3, 0.5, 0.25, 1e-6);

            Assert.Equal(2.0, intersection03.Distance, DecimalPlaces);
            Assert.False(intersection03.DoesIntersect);
            Assert.False(intersection03.DoesIntersectBetween);
        }

        [Fact]
        public void TestSerialization()
        {
            LineSegment ls0 = new LineSegment(
                new Point(0.0, 0.0, 0.0),
                new Point(10.0, 0.0, 0.0)
            );

            LineSegment ls1 = new LineSegment(
                new Point(4.0, 1.0, 0.0),
                new Point(8.0, -3.0, 0.0)
            );

            var intersection = new LineLineIntersection(ls0, ls1, 0.5, 0.25);

            var intersectionitem =
                new HelperWorkspaceItem<LineLineIntersection>(intersection);

            var workspace = new JsonWorkspace();
            workspace.Save(intersectionitem);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedItem =
                workspaceCopy.Load<HelperWorkspaceItem<LineLineIntersection>>(
                    intersectionitem.ItemInfo.Uid
                );

            Assert.Equal(intersection, deserializedItem.Data);
        }

        #endregion

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
