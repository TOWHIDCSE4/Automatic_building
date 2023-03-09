using System;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestLineSegment
    {
        #region Constructors

        public TestLineSegment(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        [Fact]
        public void TestConstructor_Default()
        {
            LineSegment ls = new LineSegment();

            Assert.Equal(new Point(), ls.Point0);
            Assert.Equal(new Point(), ls.Point1);
        }

        [Fact]
        public void TestConstructor_Points()
        {
            Point point0 = new Point(1.2, 3.4, 5.6);
            Point point1 = new Point(0.1, 2.0, 30.0);

            LineSegment ls = new LineSegment(point0, point1);

            Assert.Equal(point0, ls.Point0);
            Assert.Equal(point1, ls.Point1);
        }

        [Fact]
        public void TestConstructor_Copy()
        {
            Point point0 = new Point(1.2, 3.4, 5.6);
            Point point1 = new Point(0.1, 2.0, 30.0);

            LineSegment ls0 = new LineSegment(point0, point1);
            LineSegment ls1 = new LineSegment(ls0);

            Assert.Equal(ls0.Point0, ls1.Point0);
            Assert.NotSame(ls0.Point0, ls1.Point0);

            Assert.Equal(ls0.Point1, ls1.Point1);
            Assert.NotSame(ls0.Point1, ls1.Point1);
        }

        [Fact]
        public void TestGetPointByParam()
        {
            const int DecimalPlaces = 6;

            LineSegment ls = new LineSegment(
                new Point(1.0, 2.0, 3.0),
                new Point(4.0, 6.0, 3.0)
            );

            Assert.True(Precision.AlmostEqual(
                new Point(-3.2, -3.6, 3.0).Vector,
                ls.GetPointByParam(-1.4).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(-2.0, -2.0, 3.0).Vector,
                ls.GetPointByParam(-1.0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(0.4, 1.2, 3.0).Vector,
                ls.GetPointByParam(-0.2).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(1.0, 2.0, 3.0).Vector,
                ls.GetPointByParam(0.0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(1.6, 2.8, 3.0).Vector,
                ls.GetPointByParam(0.2).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(4.0, 6.0, 3.0).Vector,
                ls.GetPointByParam(1.0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(4.6, 6.8, 3.0).Vector,
                ls.GetPointByParam(1.2).Vector,
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestGetPointByParam_ZeroLength()
        {
            LineSegment ls = new LineSegment(
                new Point(1.2, 3.4, 5.6),
                new Point(1.2, 3.4, 5.6)
            );

            Assert.Equal(0.0, ls.Length);

            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByParam(-2.0)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByParam(-1.0)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByParam(-0.3)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByParam(0.0)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByParam(0.7)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByParam(1.0)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByParam(2.0)
            );
        }

        [Fact]
        public void TestGetPointByDistance()
        {
            const int DecimalPlaces = 6;

            LineSegment ls = new LineSegment(
                new Point(1.0, 2.0, 3.0),
                new Point(4.0, 6.0, 3.0)
            );

            Assert.True(Precision.AlmostEqual(
                new Point(-3.2, -3.6, 3.0).Vector,
                ls.GetPointByLength(-7.0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(-2.0, -2.0, 3.0).Vector,
                ls.GetPointByLength(-5.0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(0.4, 1.2, 3.0).Vector,
                ls.GetPointByLength(-1.0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(1.0, 2.0, 3.0).Vector,
                ls.GetPointByLength(0.0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(1.6, 2.8, 3.0).Vector,
                ls.GetPointByLength(1.0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(4.0, 6.0, 3.0).Vector,
                ls.GetPointByLength(5.0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new Point(4.6, 6.8, 3.0).Vector,
                ls.GetPointByLength(6.0).Vector,
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestGetPointByDistance_ZeroLength()
        {
            LineSegment ls = new LineSegment(
                new Point(1.2, 3.4, 5.6),
                new Point(1.2, 3.4, 5.6)
            );

            Assert.Equal(0.0, ls.Length);

            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByLength(-10.0)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByLength(-5.0)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByLength(-1.5)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByLength(0.0)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByLength(3.5)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByLength(5.0)
            );
            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                ls.GetPointByLength(10.0)
            );
        }

        [Fact]
        public void TestGetClosestPointEtc()
        {
            const int DecimalPlaces = 6;

            LineSegment ls = new LineSegment(
                new Point(0.0, 0.0, 0.0),
                new Point(Math.Sqrt(3.0), 1.0, 0.0)
            );

            // Non-collinear and within
            Point point0 = new Point(0.0, 2.0, 0.0);
            Assert.Equal(
                0.5,
                ls.GetClosestParam(point0),
                DecimalPlaces
            );
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { Math.Sqrt(3.0) * 0.5, 0.5, 0.0 }),
                ls.GetClosestPoint(point0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { Math.Sqrt(3.0) * 0.5, 0.5, 0.0 }),
                ls.GetClosestPointBetween(point0).Vector,
                DecimalPlaces
            ));
            Point point1 = new Point(Math.Sqrt(3.0), 0.0, 0.0);
            Assert.Equal(
                0.75,
                ls.GetClosestParam(point1),
                DecimalPlaces
            );
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { Math.Sqrt(3.0) * 0.75, 0.75, 0.0 }),
                ls.GetClosestPoint(point1).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { Math.Sqrt(3.0) * 0.75, 0.75, 0.0 }),
                ls.GetClosestPointBetween(point1).Vector,
                DecimalPlaces
            ));

            // Collinear and within (on line segment)
            Point point2 = new Point(Math.Sqrt(3.0) * 0.25, 0.25, 0.0);
            Assert.Equal(
                0.25,
                ls.GetClosestParam(point2),
                DecimalPlaces
            );
            Assert.True(Precision.AlmostEqual(
                point2.Vector,
                ls.GetClosestPoint(point2).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                point2.Vector,
                ls.GetClosestPointBetween(point2).Vector,
                DecimalPlaces
            ));

            // On end points
            Assert.Equal(
                0.0,
                ls.GetClosestParam(ls.Point0),
                DecimalPlaces
            );
            Assert.True(Precision.AlmostEqual(
                ls.Point0.Vector,
                ls.GetClosestPoint(ls.Point0).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                ls.Point0.Vector,
                ls.GetClosestPointBetween(ls.Point0).Vector,
                DecimalPlaces
            ));
            Assert.Equal(
                1.0,
                ls.GetClosestParam(ls.Point1),
                DecimalPlaces
            );
            Assert.True(Precision.AlmostEqual(
                ls.Point1.Vector,
                ls.GetClosestPoint(ls.Point1).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                ls.Point1.Vector,
                ls.GetClosestPointBetween(ls.Point1).Vector,
                DecimalPlaces
            ));

            // Collinear and outside line segment
            Point point3 = new Point(-Math.Sqrt(3.0), -1.0, 0.0);
            Assert.Equal(
                -1.0,
                ls.GetClosestParam(point3),
                DecimalPlaces
            );
            Assert.True(Precision.AlmostEqual(
                point3.Vector,
                ls.GetClosestPoint(point3).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                ls.Point0.Vector,
                ls.GetClosestPointBetween(point3).Vector,
                DecimalPlaces
            ));
            Point point4 = new Point(Math.Sqrt(3.0) * 1.2, 1.2, 0.0);
            Assert.Equal(
                1.2,
                ls.GetClosestParam(point4),
                DecimalPlaces
            );
            Assert.True(Precision.AlmostEqual(
                point4.Vector,
                ls.GetClosestPoint(point4).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                ls.Point1.Vector,
                ls.GetClosestPointBetween(point4).Vector,
                DecimalPlaces
            ));

            // Non-collinear and outside line segment
            Point point5 = new Point(-Math.Sqrt(3.0), 0.0, 0.0);
            Assert.Equal(
                -0.75,
                ls.GetClosestParam(point5),
                DecimalPlaces
            );
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { -Math.Sqrt(3.0) * 0.75, -0.75, 0.0 }),
                ls.GetClosestPoint(point5).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                ls.Point0.Vector,
                ls.GetClosestPointBetween(point5).Vector,
                DecimalPlaces
            ));
            Point point6 = new Point(Math.Sqrt(3.0) + 1.0, 1.0, 0.0);
            Assert.Equal(
                1.0 + Math.Sqrt(3.0) / 4,
                ls.GetClosestParam(point6),
                DecimalPlaces
            );
            Assert.True(Precision.AlmostEqual(
                new DenseVector(
                    new[]
                    {
                        Math.Sqrt(3.0) + 0.75,
                        1.0 + Math.Sqrt(3.0) * 0.25,
                        0.0
                    }
                ),
                ls.GetClosestPoint(point6).Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                ls.Point1.Vector,
                ls.GetClosestPointBetween(point6).Vector,
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestGetClosestPointEtc_ZeroLength()
        {
            LineSegment ls = new LineSegment(
                new Point(1.0, 2.0, 3.0),
                new Point(1.0, 2.0, 3.0)
            );

            // Overlapping
            Point point0 = new Point(1.0, 2.0, 3.0);
            Assert.Equal(0.0, ls.GetClosestParam(point0));
            Assert.Equal(ls.Point0, ls.GetClosestPoint(point0));
            Assert.Equal(ls.Point0, ls.GetClosestPointBetween(point0));

            // Non-overlapping
            Point point1 = new Point(2.0, 5.0, 6.0);
            Assert.Equal(0.0, ls.GetClosestParam(point1));
            Assert.Equal(ls.Point0, ls.GetClosestPoint(point1));
            Assert.Equal(ls.Point0, ls.GetClosestPointBetween(point1));
        }

        [Fact]
        public void TestGetClosestParams_NonParallel()
        {
            const int DecimalPlaces = 6;

            LineSegment ls0 = new LineSegment(
                new Point(0.0, 0.0, 0.0),
                new Point(4.0, 4.0, 0.0)
            );

            LineSegment ls1 = new LineSegment(
                new Point(1.0, 4.0, 1.0),
                new Point(4.0, -2.0, 1.0)
            );

            LineSegment ls2 = new LineSegment(
                new Point(5.0, 3.0, 0.0),
                new Point(7.0, 3.0, 0.0)
            );

            LineSegment ls3 = new LineSegment(
                new Point(6.5, 4.0, -2.0),
                new Point(7.0, 2.0, -2.0)
            );

            var closestParams01 = LineSegment.GetClosestParams(ls0, ls1);

            Assert.Equal(0.5, closestParams01.Item1, DecimalPlaces);
            Assert.Equal(1.0 / 3.0, closestParams01.Item2, DecimalPlaces);

            var closestParams02 = LineSegment.GetClosestParams(ls0, ls2);

            Assert.Equal(0.75, closestParams02.Item1, DecimalPlaces);
            Assert.Equal(-1.0, closestParams02.Item2, DecimalPlaces);

            var closestParams03 = LineSegment.GetClosestParams(ls0, ls3);

            Assert.Equal(1.5, closestParams03.Item1, DecimalPlaces);
            Assert.Equal(-1.0, closestParams03.Item2, DecimalPlaces);
        }

        [Fact]
        public void TestGetClosestParams_Collinear()
        {
            const int DecimalPlaces = 6;

            LineSegment ls0 = new LineSegment(
                new Point(1.0, 2.0, 0.0),
                new Point(5.0, 2.0, 0.0)
            );

            // Collinear, non-overlapping with ls0
            LineSegment ls1 = new LineSegment(
                new Point(7.0, 2.0, 0.0),
                new Point(8.0, 2.0, 0.0)
            );

            // Collinear, partially overlapping with ls0
            LineSegment ls2 = new LineSegment(
                new Point(4.0, 2.0, 0.0),
                new Point(6.0, 2.0, 0.0)
            );

            // Collinear, contained by ls0
            LineSegment ls3 = new LineSegment(
                new Point(2.0, 2.0, 0.0),
                new Point(4.0, 2.0, 0.0)
            );

            var closestParams00 = LineSegment.GetClosestParams(ls0, ls0);

            Assert.Equal(0.0, closestParams00.Item1, DecimalPlaces);
            Assert.Equal(0.0, closestParams00.Item2, DecimalPlaces);

            var closestParams01 = LineSegment.GetClosestParams(ls0, ls1);

            Assert.Equal(1.0, closestParams01.Item1, DecimalPlaces);
            Assert.Equal(-2.0, closestParams01.Item2, DecimalPlaces);

            var closestParams10 = LineSegment.GetClosestParams(ls1, ls0);

            Assert.Equal(0.0, closestParams10.Item1, DecimalPlaces);
            Assert.Equal(1.5, closestParams10.Item2, DecimalPlaces);

            var closestParams02 = LineSegment.GetClosestParams(ls0, ls2);

            Assert.Equal(1.0, closestParams02.Item1, DecimalPlaces);
            Assert.Equal(0.5, closestParams02.Item2, DecimalPlaces);

            var closestParams20 = LineSegment.GetClosestParams(ls2, ls0);

            Assert.Equal(0.0, closestParams20.Item1, DecimalPlaces);
            Assert.Equal(0.75, closestParams20.Item2, DecimalPlaces);

            var closestParams03 = LineSegment.GetClosestParams(ls0, ls3);

            Assert.Equal(0.25, closestParams03.Item1, DecimalPlaces);
            Assert.Equal(0.0, closestParams03.Item2, DecimalPlaces);

            var closestParams30 = LineSegment.GetClosestParams(ls3, ls0);

            Assert.Equal(0.0, closestParams30.Item1, DecimalPlaces);
            Assert.Equal(0.25, closestParams30.Item2, DecimalPlaces);
        }

        [Fact]
        public void TestGetClosestParams_ParallelButNotCollinear()
        {
            const int DecimalPlaces = 6;

            LineSegment ls0 = new LineSegment(
                new Point(1.0, 2.0, 0.0),
                new Point(5.0, 2.0, 0.0)
            );

            // Collinear, non-overlapping with ls0
            LineSegment ls1 = new LineSegment(
                new Point(7.0, 2.0, 0.0),
                new Point(8.0, 2.0, 0.0)
            );

            // Collinear, partially overlapping with ls0
            LineSegment ls2 = new LineSegment(
                new Point(4.0, 2.0, 0.0),
                new Point(6.0, 2.0, 0.0)
            );

            // Collinear, contained by ls0
            LineSegment ls3 = new LineSegment(
                new Point(2.0, 2.0, 0.0),
                new Point(4.0, 2.0, 0.0)
            );

            var closestParams00 = LineSegment.GetClosestParams(ls0, ls0);

            Assert.Equal(0.0, closestParams00.Item1, DecimalPlaces);
            Assert.Equal(0.0, closestParams00.Item2, DecimalPlaces);

            var closestParams01 = LineSegment.GetClosestParams(ls0, ls1);

            Assert.Equal(1.0, closestParams01.Item1, DecimalPlaces);
            Assert.Equal(-2.0, closestParams01.Item2, DecimalPlaces);

            var closestParams10 = LineSegment.GetClosestParams(ls1, ls0);

            Assert.Equal(0.0, closestParams10.Item1, DecimalPlaces);
            Assert.Equal(1.5, closestParams10.Item2, DecimalPlaces);

            var closestParams02 = LineSegment.GetClosestParams(ls0, ls2);

            Assert.Equal(1.0, closestParams02.Item1, DecimalPlaces);
            Assert.Equal(0.5, closestParams02.Item2, DecimalPlaces);

            var closestParams20 = LineSegment.GetClosestParams(ls2, ls0);

            Assert.Equal(0.0, closestParams20.Item1, DecimalPlaces);
            Assert.Equal(0.75, closestParams20.Item2, DecimalPlaces);

            var closestParams03 = LineSegment.GetClosestParams(ls0, ls3);

            Assert.Equal(0.25, closestParams03.Item1, DecimalPlaces);
            Assert.Equal(0.0, closestParams03.Item2, DecimalPlaces);

            var closestParams30 = LineSegment.GetClosestParams(ls3, ls0);

            Assert.Equal(0.0, closestParams30.Item1, DecimalPlaces);
            Assert.Equal(0.25, closestParams30.Item2, DecimalPlaces);
        }

        [Fact]
        public void TestGetClosestParams_ZeroLength()
        {
            const int DecimalPlaces = 6;

            LineSegment ls0 = new LineSegment(
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 0.0, 0.0)
            );

            LineSegment ls1 = new LineSegment(
                new Point(2.0, 0.0, 0.0),
                new Point(2.0, 0.0, 0.0)
            );

            LineSegment ls2 = new LineSegment(
                new Point(3.0, 1.0, 0.0),
                new Point(5.0, 1.0, 0.0)
            );

            var closestParams00 = LineSegment.GetClosestParams(ls0, ls0);

            Assert.Equal(0.0, closestParams00.Item1, DecimalPlaces);
            Assert.Equal(0.0, closestParams00.Item2, DecimalPlaces);

            var closestParams01 = LineSegment.GetClosestParams(ls0, ls1);

            Assert.Equal(0.0, closestParams01.Item1, DecimalPlaces);
            Assert.Equal(0.0, closestParams01.Item2, DecimalPlaces);

            var closestParams02 = LineSegment.GetClosestParams(ls0, ls2);

            Assert.Equal(0.0, closestParams02.Item1, DecimalPlaces);
            Assert.Equal(-1.0, closestParams02.Item2, DecimalPlaces);

            var closestParams20 = LineSegment.GetClosestParams(ls2, ls0);

            Assert.Equal(0.0, closestParams20.Item1, DecimalPlaces);
            Assert.Equal(0.0, closestParams20.Item2, DecimalPlaces);
        }

        [Fact]
        public void TestIntersect()
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

            var intersection01A = LineSegment.Intersect(ls0, ls1, 1e-5);

            Assert.Equal(0.5, intersection01A.Param0, DecimalPlaces);
            Assert.Equal(0.25, intersection01A.Param1, DecimalPlaces);
            Assert.Equal(1e-6, intersection01A.Distance, DecimalPlaces);
            Assert.True(intersection01A.DoesIntersect);
            Assert.True(intersection01A.DoesIntersectBetween);

            var intersection01B = LineSegment.Intersect(ls0, ls1, 1e-7);

            Assert.Equal(0.5, intersection01B.Param0, DecimalPlaces);
            Assert.Equal(0.25, intersection01B.Param1, DecimalPlaces);
            Assert.Equal(1e-6, intersection01B.Distance, DecimalPlaces);
            Assert.False(intersection01B.DoesIntersect);
            Assert.False(intersection01B.DoesIntersectBetween);

            var intersection02 = LineSegment.Intersect(ls0, ls2, 1e-6);

            Assert.Equal(0.5, intersection02.Param0, DecimalPlaces);
            Assert.Equal(-1.0, intersection02.Param1, DecimalPlaces);
            Assert.Equal(0.0, intersection02.Distance, DecimalPlaces);
            Assert.True(intersection02.DoesIntersect);
            Assert.False(intersection02.DoesIntersectBetween);

            var intersection03 = LineSegment.Intersect(ls0, ls3, 1e-6);

            Assert.Equal(0.5, intersection03.Param0, DecimalPlaces);
            Assert.Equal(0.25, intersection03.Param1, DecimalPlaces);
            Assert.Equal(2.0, intersection03.Distance, DecimalPlaces);
            Assert.False(intersection03.DoesIntersect);
            Assert.False(intersection03.DoesIntersectBetween);
        }

        [Fact]
        public void TestGetBBox()
        {
            Assert.Equal(
                new BBox(),
                new LineSegment().GetBBox()
            );

            Assert.Equal(
                BBox.FromMinAndMax(
                    -7.8, 3.4, -10.11,
                    1.2, 9.0, 5.6
                ),
                new LineSegment(
                    new Point(1.2, 3.4, 5.6),
                    new Point(-7.8, 9.0, -10.11)
                ).GetBBox()
            );
        }

        [Theory]
        [MemberData(nameof(TestGetHashCodeAndEqualsAndOpsData))]
        public void TestGetHashCodeAndEqualsAndOps(
            LineSegment ls0, LineSegment ls1, bool equals
        )
        {
            Assert.Equal(ls0, ls0);
            Assert.Equal(ls0.GetHashCode(), ls0.GetHashCode());
            Assert.NotNull(ls0);

            Assert.True(ls0 == ls0);
            Assert.False(ls0 == null);
            Assert.False(null == ls0);

            Assert.False(ls0 != ls0);
            Assert.True(ls0 != null);
            Assert.True(null != ls0);

            if (equals)
            {
                Assert.Equal(ls0, ls1);
                Assert.Equal(ls1, ls0);
                Assert.Equal(ls0.GetHashCode(), ls1.GetHashCode());
                Assert.True(ls0 == ls1);
                Assert.True(ls1 == ls0);
                Assert.False(ls0 != ls1);
                Assert.False(ls1 != ls0);
            }
            else
            {
                Assert.NotEqual(ls0, ls1);
                Assert.NotEqual(ls1, ls0);
                Assert.NotEqual(ls0.GetHashCode(), ls1.GetHashCode());
                Assert.False(ls0 == ls1);
                Assert.False(ls1 == ls0);
                Assert.True(ls0 != ls1);
                Assert.True(ls1 != ls0);
            }
        }

        public static IEnumerable<object[]> TestGetHashCodeAndEqualsAndOpsData
        {
            get
            {
                LineSegment ls0 = new LineSegment();

                LineSegment ls1 = new LineSegment();

                LineSegment ls2 = new LineSegment(
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 3.0)
                );

                LineSegment ls3 = new LineSegment(
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 3.0)
                );

                LineSegment ls4 = new LineSegment(
                    new Point(1.2, 3.4, 5.6),
                    new Point(1.1, 2.2, 0.0)
                );

                return new[]
                {
                    new object[] { ls0, ls1, true },
                    new object[] { ls2, ls3, true },
                    new object[] { ls2, ls4, false }
                };
            }
        }

        [Fact]
        public void TestPoint0()
        {
            LineSegment ls = new LineSegment();

            Point point = new Point(1.0, 2.0, 3.0);

            ls.Point0 = point;

            Assert.Equal(point, ls.Point0);
        }

        [Fact]
        public void TestPoint0_SetNull()
        {
            LineSegment ls = new LineSegment();

            Assert.Throws<ArgumentNullException>(
                () => { ls.Point0 = null; }
            );

            Assert.Equal(new Point(), ls.Point0);
        }

        [Fact]
        public void TestPoint1()
        {
            LineSegment ls = new LineSegment();

            Point point = new Point(1.0, 2.0, 3.0);

            ls.Point1 = point;

            Assert.Equal(point, ls.Point1);
        }

        [Fact]
        public void TestPoint1_SetNull()
        {
            LineSegment ls = new LineSegment();

            Assert.Throws<ArgumentNullException>(
                () => { ls.Point1 = null; }
            );

            Assert.Equal(new Point(), ls.Point1);
        }

        [Fact]
        public void TestDirection()
        {
            const int DecimalPlaces = 6;

            LineSegment ls = new LineSegment(
                new Point(1.0, 2.0, 3.0),
                new Point(4.0, 6.0, 3.0)
            );

            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 0.6, 0.8, 0.0 }),
                ls.Direction,
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestDirection_ZeroLength()
        {
            LineSegment ls = new LineSegment();

            Assert.Equal(0.0, ls.Length);

            Assert.Equal(
                new DenseVector(new[] { 0.0, 0.0, 0.0 }),
                ls.Direction
            );
        }

        [Fact]
        public void TestLength()
        {
            const int DecimalPlaces = 6;

            LineSegment ls = new LineSegment(
                new Point(1.0, 2.0, 3.0),
                new Point(4.0, 6.0, 3.0)
            );

            Assert.Equal(5.0, ls.Length, DecimalPlaces);
        }

        [Fact]
        public void TestLength_ZeroLength()
        {
            LineSegment ls = new LineSegment(
                new Point(1.0, 2.0, 3.0),
                new Point(1.0, 2.0, 3.0)
            );

            Assert.Equal(0.0, ls.Length);
        }

        [Fact]
        public void TestSerialization()
        {
            var ls = new LineSegment(
                new Point(1.0, 2.0, 3.0),
                new Point(10.0, 20.0, 30.0)
            );


            var lsItem = new HelperWorkspaceItem<LineSegment>(ls);

            var workspace = new JsonWorkspace();
            workspace.Save(lsItem);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserialziedItem =
                workspaceCopy.Load<HelperWorkspaceItem<LineSegment>>(
                    lsItem.ItemInfo.Uid
                );

            Assert.Equal(ls, deserialziedItem.Data);
        }

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
