using System;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestPolygon
    {
        #region Constructors

        public TestPolygon(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConstructor_Default()
        {
            Polygon polygon = new Polygon();

            Assert.Empty(polygon.Points);
        }

        [Fact]
        public void TestConstructor_Points()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 0.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(0.5, 3.0, 0.0)
            };

            Polygon polygon = new Polygon(points);

            Assert.Equal(points, polygon.Points);
        }

        [Fact]
        public void TestConstructor_Copy()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 0.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(0.5, 3.0, 0.0)
            };

            Polygon polygon0 = new Polygon(points);
            Polygon polygon1 = new Polygon(polygon0);

            Assert.Equal(polygon0.Points, polygon1.Points);
            Assert.NotSame(polygon0.Points, polygon1.Points);

            for (
                int pointIndex = 0; pointIndex < polygon0.Points.Count;
                ++pointIndex)
            {
                Assert.NotSame(
                    polygon0.Points[pointIndex],
                    polygon1.Points[pointIndex]
                );
            }
        }

        [Fact]
        public void TestAddPoint()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 0.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(0.5, 3.0, 0.0)
            };

            Polygon polygon = new Polygon(points);

            polygon.AddPoint(new Point(-0.5, 1.2, 0.0));

            Assert.Equal(
                new List<Point>
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(0.5, 3.0, 0.0),
                    new Point(-0.5, 1.2, 0.0)
                },
                polygon.Points
            );
        }

        [Fact]
        public void TestAddPoint_NullPoint()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 0.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(0.5, 3.0, 0.0)
            };

            Polygon polygon = new Polygon(points);

            Assert.Throws<ArgumentNullException>(
                () => { polygon.AddPoint(null); }
            );

            Assert.Equal(
                new List<Point>
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(0.5, 3.0, 0.0)
                },
                polygon.Points
            );
        }

        [Fact]
        public void TestInsertPoint()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 0.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(0.5, 3.0, 0.0)
            };

            Polygon polygon = new Polygon(points);

            polygon.InsertPoint(2, new Point(-0.5, 1.2, 0.0));

            Assert.Equal(
                new List<Point>
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(-0.5, 1.2, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(0.5, 3.0, 0.0)
                },
                polygon.Points
            );
        }

        [Fact]
        public void TestInsertPoint_NullPoint()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 0.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(0.5, 3.0, 0.0)
            };

            Polygon polygon = new Polygon(points);

            Assert.Throws<ArgumentNullException>(
                () => { polygon.InsertPoint(2, null); }
            );

            Assert.Equal(
                new List<Point>
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(0.5, 3.0, 0.0)
                },
                polygon.Points
            );
        }

        [Fact]
        public void TestInsertPoint_InvalidIndex()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 0.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(0.5, 3.0, 0.0)
            };

            Polygon polygon = new Polygon(points);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { polygon.InsertPoint(100, new Point(-0.5, 1.2, 0.0)); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { polygon.InsertPoint(-1, new Point(-0.5, 1.2, 0.0)); }
            );

            Assert.Equal(
                new List<Point>
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(0.5, 3.0, 0.0)
                },
                polygon.Points
            );
        }

        [Fact]
        public void TestRemovePoint()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 0.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(0.5, 3.0, 0.0)
            };

            Polygon polygon = new Polygon(points);

            Point removedPoint = polygon.RemovePoint(2);

            Assert.Equal(new Point(1.0, 2.0, 0.0), removedPoint);

            Assert.Equal(
                new List<Point>
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(0.5, 3.0, 0.0)
                },
                polygon.Points
            );
        }

        [Fact]
        public void TestRemovePoint_InvalidIndex()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 0.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(0.5, 3.0, 0.0)
            };

            Polygon polygon = new Polygon(points);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { polygon.RemovePoint(100); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { polygon.RemovePoint(-2); }
            );

            Assert.Equal(
                new List<Point>
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(0.5, 3.0, 0.0)
                },
                polygon.Points
            );
        }

        [Fact]
        public void TestGetEdgeNormal_OnePoint()
        {
            Polygon polygon = new Polygon(
                new[] { new Point(1.0, 2.0, 3.0) }
            );

            Assert.Equal(
                new DenseVector(3),
                polygon.GetEdgeNormal(0)
            );
        }

        [Fact]
        public void TestGetEdgeNormal_TwoPoints()
        {
            Polygon polygon = new Polygon(
                new[] {
                    new Point(1.0, 2.0, 3.0),
                    new Point(0.5, 1.0, 1.5)
                }
            );

            Assert.Equal(
                new DenseVector(3),
                polygon.GetEdgeNormal(0)
            );
            Assert.Equal(
                new DenseVector(3),
                polygon.GetEdgeNormal(1)
            );
        }

        [Fact]
        public void TestGetEdgeNormal_ThreeAndMorePoints()
        {
            const int DecimalPlaces = 6;

            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.6, 0.0, 0.0),
                    new Point(0.6, 1.2, 0.0),
                    new Point(0.0, 0.0, 0.8)
                }
            );

            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 0.6, 0.0, -0.8 }),
                polygon0.GetEdgeNormal(0),
                DecimalPlaces
            ));

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 1.0, 0.0),
                    new Point(0.5, 2.0, 0.0)
                }
            );

            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 0.0, -1.0, 0.0 }),
                polygon1.GetEdgeNormal(0),
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 1.0, 0.0, 0.0 }),
                polygon1.GetEdgeNormal(1),
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 2.0, 1.0, 0.0 }) / Math.Sqrt(5.0),
                polygon1.GetEdgeNormal(2),
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { -4.0, 1.0, 0.0 }) / Math.Sqrt(17.0),
                polygon1.GetEdgeNormal(3),
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestGetEdgeNormal_ConcavePolygon()
        {
            const int DecimalPlaces = 6;

            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(10.0, 0.0, 0.0),
                    new Point(10.0, 10.0, 0.0),
                    new Point(3.0, 10.0, 0.0),
                    new Point(3.0, 6.0, 0.0),
                    new Point(0.0, 6.0, 0.0)
                }
            );

            Assert.Equal(
                new LineSegment(
                    new Point(3.0, 10.0, 0.0),
                    new Point(3.0, 6.0, 0.0)
                ),
                polygon.GetEdge(3)
            );
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { -1.0, 0.0, 0.0 }),
                polygon.GetEdgeNormal(3),
                DecimalPlaces
            ));

            Assert.Equal(
                new LineSegment(
                    new Point(3.0, 6.0, 0.0),
                    new Point(0.0, 6.0, 0.0)
                ),
                polygon.GetEdge(4)
            );
            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 0.0, 1.0, 0.0 }),
                polygon.GetEdgeNormal(4),
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestGetInteriorAngle()
        {
            const int DecimalPlaces = 6;

            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0 + Math.Sqrt(3.0) / 3.0, 1.0, 0.0),
                    new Point(2.0, 1.0, 0.0),
                    new Point(2.0, 2.0, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(1.0, 1.0, 0.0)
                }
            );

            Assert.Equal(
                Math.PI * 0.25, polygon.GetInteriorAngle(0),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI * 2.0 / 3.0, polygon.GetInteriorAngle(1),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI / 3.0, polygon.GetInteriorAngle(2),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI * 1.5, polygon.GetInteriorAngle(3),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI * 0.5, polygon.GetInteriorAngle(4),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI * 0.5, polygon.GetInteriorAngle(5),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI * 1.25, polygon.GetInteriorAngle(6),
                DecimalPlaces
            );
        }

        [Fact]
        public void TestPointPlaneRelations_ZeroPointPolygon()
        {
            Polygon polygon = new Polygon();

            Assert.False(polygon.IsPointAbove(new Point(0.0, 0.0, 0.0)));
            Assert.False(polygon.IsPointAbove(new Point(1.0, 2.0, 3.0)));

            Assert.True(polygon.IsPointCoplanar(new Point(0.0, 0.0, 0.0)));
            Assert.True(polygon.IsPointCoplanar(new Point(1.0, 2.0, 3.0)));

            Assert.False(polygon.IsPointBelow(new Point(0.0, 0.0, 0.0)));
            Assert.False(polygon.IsPointBelow(new Point(1.0, 2.0, 3.0)));
        }

        [Fact]
        public void TestPointPlaneRelations_OnePointPolygon()
        {
            Polygon polygon = new Polygon(new[] { new Point(1.0, 2.0, 3.0) });

            Assert.False(polygon.IsPointAbove(new Point(0.2, 10.0, 8.6)));
            Assert.False(polygon.IsPointAbove(new Point(1.0, 2.0, 3.0)));

            Assert.True(polygon.IsPointCoplanar(new Point(0.2, 10.0, 8.6)));
            Assert.True(polygon.IsPointCoplanar(new Point(1.0, 2.0, 3.0)));

            Assert.False(polygon.IsPointBelow(new Point(0.2, 10.0, 8.6)));
            Assert.False(polygon.IsPointBelow(new Point(1.0, 2.0, 3.0)));
        }

        [Fact]
        public void TestPointPlaneRelations_TwoPointPolygon()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 5.0, 0.0)
                }
            );

            Assert.False(polygon.IsPointAbove(new Point(1.0, 2.0, 3.0)));
            Assert.False(polygon.IsPointAbove(new Point(1.0, 0.0, 0.0)));

            Assert.True(polygon.IsPointCoplanar(new Point(1.0, 2.0, 3.0)));
            Assert.True(polygon.IsPointCoplanar(new Point(1.0, 0.0, 0.0)));

            Assert.False(polygon.IsPointBelow(new Point(1.0, 2.0, 3.0)));
            Assert.False(polygon.IsPointBelow(new Point(1.0, 0.0, 0.0)));
        }

        [Fact]
        public void TestPointPlaneRelations_ThreeAndMorePointPolygon()
        {
            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 3.0, 0.0),
                    new Point(0.0, 0.0, 2.0)
                }
            );

            Assert.True(polygon0.IsPointAbove(new Point(1.0, 2.0, 10.0)));

            Assert.True(polygon0.IsPointCoplanar(new Point(1.0, 3.0, 0.0)));
            Assert.True(polygon0.IsPointCoplanar(new Point(0.7, 0.2, 0.6)));
            Assert.True(polygon0.IsPointCoplanar(new Point(0.8, -1.0, 0.4)));

            Assert.True(polygon0.IsPointBelow(new Point(1.0, 2.0, -10.0)));

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(3.0, 0.0, 0.0),
                    new Point(3.0, 2.0, 0.0),
                    new Point(0.0, 2.0, 0.0)
                }
            );

            Assert.True(polygon1.IsPointAbove(new Point(1.0, 0.0, 10.0)));

            Assert.True(polygon1.IsPointCoplanar(new Point(10.0, 32.0, 0.0)));
            Assert.True(polygon1.IsPointCoplanar(new Point(0.7, 0.2, 0.0)));

            Assert.True(polygon1.IsPointBelow(new Point(0.0, 2.0, -10.0)));
        }

        [Fact]
        public void TestIsPointInside2D_ZeroPointPolygon()
        {
            Polygon polygon = new Polygon();

            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(1.0, 2.0, 0.0)
            ));
        }

        [Fact]
        public void TestIsPointInside2D_OnePointPolygon()
        {
            Polygon polygon = new Polygon(
                new[] { new Point(1.0, 2.0, 0.0) }
            );

            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(1.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(2.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(3.0, 2.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(3.0, 4.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(1.0, 3.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 3.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 2.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(1.0, 2.0, 0.0)
            ));
        }

        [Fact]
        public void TestIsPointInside2D_TwoPointPolygon()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(1.0, 2.0, 0.0),
                    new Point(1.5, 2.5, 0.0)
                }
            );

            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(1.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(2.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(3.0, 2.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(3.0, 4.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(1.0, 3.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 3.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 2.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(1.0, 2.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(1.5, 2.5, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(1.25, 2.25, 0.0)
            ));
        }

        [Fact]
        public void TestIsPointInside2D_ThreeAndMorePointPolygonCase0()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(4.0, 0.0, 0.0),
                    new Point(4.0, 3.0, 0.0),
                    new Point(0.0, 3.0, 0.0)
                }
            );

            // On edge

            Assert.True(polygon.IsPointInside2D(
                new Point(0.0, 0.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(1.5, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(4.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(4.0, 2.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(4.0, 3.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(3.0, 3.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 3.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(0.0, 2.0, 0.0)
            ));

            // Inside

            Assert.True(polygon.IsPointInside2D(
                new Point(1.0, 2.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(3.9, 2.9, 0.0)
            ));

            // Outside

            Assert.False(polygon.IsPointInside2D(
                new Point(-1.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(-2.0, -1.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, -1.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(2.0, -1.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(4.0, -1.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(5.0, -2.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(5.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(5.0, 1.5, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(5.0, 3.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(5.0, 4.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(4.0, 6.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(3.0, 6.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 6.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(-4.0, 5.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(-4.0, 3.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(-4.0, 2.5, 0.0)
            ));
        }

        [Fact]
        public void TestIsPointInside2D_ThreeAndMorePointPolygonCase1()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(5.0, 1.0, 0.0),
                    new Point(5.0, 2.0, 0.0),
                    new Point(3.0, 3.0, 0.0),
                    new Point(3.0, 5.0, 0.0),
                    new Point(0.0, 5.0, 0.0),
                    new Point(0.0, 3.0, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(0.0, 1.0, 0.0)
                }
            );

            // On edge

            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 0.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(5.0, 1.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(3.0, 5.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 5.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(0.0, 3.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(1.0, 2.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(0.0, 1.0, 0.0)
            ));

            // Inside

            Assert.True(polygon.IsPointInside2D(
                new Point(1.0, 1.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(3.0, 1.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(2.0, 2.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(4.0, 2.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(2.0, 2.5, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(1.0, 3.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(0.5, 4.0, 0.0)
            ));
            Assert.True(polygon.IsPointInside2D(
                new Point(2.5, 4.5, 0.0)
            ));

            // Outside

            Assert.False(polygon.IsPointInside2D(
                new Point(5.0, 0.5, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(4.0, 3.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(-1.0, 3.5, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(0.5, 2.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(0.0, 2.0, 0.0)
            ));
            Assert.False(polygon.IsPointInside2D(
                new Point(-1.0, 1.0, 0.0)
            ));
        }

        [Fact]
        public void TestTriangulate_PolygonWithLessThan3Points()
        {
            Polygon polygon0 = new Polygon();

            Assert.Empty(polygon0.Trianglate());

            Polygon polygon1 = new Polygon(
                new[] { new Point(0.1, 2.3, 4.5) }
            );

            Assert.Empty(polygon1.Trianglate());

            Polygon polygon2 = new Polygon(
                new[]
                {
                    new Point(0.1, 2.3, 4.5),
                    new Point(6.7, 8.9, 1.0)
                }
            );

            Assert.Empty(polygon2.Trianglate());
        }

        [Fact]
        public void TestTriangulate_Triangle()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.2, 0.0),
                    new Point(0.5, 1.0, 0.0)
                }
            );

            Assert.Equal(
                new[] { new Tuple<int, int, int>(0, 1, 2) },
                polygon.Trianglate()
            );
        }

        [Fact]
        public void TestTriangulate_ConvexPolygonWithMoreThan3Points()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(10.0, 0.0, 0.0),
                    new Point(10.0, 5.0, 0.0),
                    new Point(1.0, 5.0, 0.0)
                }
            );

            Assert.Equal(
                new[]
                {
                    new Tuple<int, int, int>(3, 0, 1),
                    new Tuple<int, int, int>(1, 2, 3)
                },
                polygon.Trianglate()
            );
        }

        [Fact]
        public void TestTriangulate_ConcavePolygonCase0()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(10.0, 0.0, 0.0),
                    new Point(10.0, 10.0, 0.0),
                    new Point(8.0, 2.0, 0.0)
                }
            );

            Assert.Equal(
                new[]
                {
                    new Tuple<int, int, int>(3, 0, 1),
                    new Tuple<int, int, int>(1, 2, 3)
                },
                polygon.Trianglate()
            );
        }

        [Fact]
        public void TestTriangulate_ConcavePolygonCase1()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(40.0, 0.0, 0.0),
                    new Point(40.0, 35.0, 0.0),
                    new Point(16.0, 30.0, 0.0),
                    new Point(15.0, 20.0, 0.0),
                    new Point(0.0, 20.0, 0.0)
                }
            );

            Assert.Equal(
                new[]
                {
                    new Tuple<int, int, int>(5, 0, 1),
                    new Tuple<int, int, int>(1, 2, 3),
                    new Tuple<int, int, int>(1, 3, 4),
                    new Tuple<int, int, int>(1, 4, 5)
                },
                polygon.Trianglate()
            );
        }

        [Fact]
        public void TestTriangulate_ConcavePolygonCase2()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(-0.5, 0.0, 0.0),
                    new Point(-0.5, -15, 0.0),
                    new Point(-5, -30, 0.0),
                    new Point(5, -30, 0.0),
                    new Point(0.5, -15, 0.0),
                    new Point(0.5, 0.0, 0.0)
                }
            );

            Assert.Equal(
                new[]
                {
                    new Tuple<int, int, int>(5, 0, 1),
                    new Tuple<int, int, int>(1, 2, 3),
                    new Tuple<int, int, int>(1, 3, 4),
                    new Tuple<int, int, int>(1, 4, 5)
                },
                polygon.Trianglate()
            );
        }

        [Fact]
        public void TestDoesContain2D_Convex()
        {
            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(10.0, 0.0, 0.0),
                    new Point(10.0, 5.0, 0.0),
                    new Point(0.0, 5.0, 0.0)
                }
            );
            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(1.0, 1.0, 0.0),
                    new Point(9.0, 1.0, 0.0),
                    new Point(9.0, 4.0, 0.0),
                    new Point(1.0, 4.0, 0.0)
                }
            );
            Polygon polygon2 = new Polygon(
                new[]
                {
                    new Point(2.0, -1.0, 0.0),
                    new Point(8.0, -1.0, 0.0),
                    new Point(8.0, 20.0, 0.0),
                    new Point(2.0, 20.0, 0.0)
                }
            );

            Assert.False(polygon0.DoesContain2D(polygon0));

            Assert.True(polygon0.DoesContain2D(polygon1));
            Assert.False(polygon1.DoesContain2D(polygon0));

            Assert.False(polygon0.DoesContain2D(polygon2));
            Assert.False(polygon2.DoesContain2D(polygon0));
        }

        [Fact]
        public void TestDoesContain2D_Concave()
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

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(1.0, 1.0, 0.0),
                    new Point(3.0, 1.0, 0.0),
                    new Point(3.0, 4.0, 0.0),
                    new Point(1.0, 4.0, 0.0)
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

            Assert.False(polygon0.DoesContain2D(polygon0));

            Assert.True(polygon0.DoesContain2D(polygon1));
            Assert.False(polygon1.DoesContain2D(polygon0));

            Assert.All(
                polygon2.Points,
                point => polygon0.IsPointInside2D(point)
            );
            Assert.True(
                Polygon.EdgeIntersect(polygon0, polygon2).DoesIntersect
            );
            Assert.False(polygon0.DoesContain2D(polygon2));
            Assert.False(polygon2.DoesContain2D(polygon0));
        }

        [Fact]
        public void TestDoesContain2D_ZeroPointPolygon()
        {
            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(10.0, 0.0, 0.0),
                    new Point(10.0, 5.0, 0.0),
                    new Point(0.0, 5.0, 0.0)
                }
            );
            Polygon polygon1 = new Polygon();
            Polygon polygon2 = new Polygon();

            Assert.True(polygon0.DoesContain2D(polygon1));
            Assert.False(polygon1.DoesContain2D(polygon0));

            Assert.True(polygon1.DoesContain2D(polygon2));
        }

        [Fact]
        public void TestDoesOverlap2D()
        {
            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 1.0, 0.0),
                    new Point(0.0, 1.0, 0.0),
                }
            );

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(1.0, 0.5, 0.0),
                    new Point(2.5, 0.5, 0.0),
                    new Point(2.5, 0.75, 0.0),
                    new Point(1.0, 0.75, 0.0)
                }
            );

            Polygon polygon2 = new Polygon(
                new[]
                {
                    new Point(1.0, 0.5, 0.0),
                    new Point(2.5, 0.5, 0.0),
                    new Point(2.5, 1.5, 0.0),
                    new Point(1.0, 1.5, 0.0)
                }
            );

            Polygon polygon3 = new Polygon(
                new[]
                {
                    new Point(3.0, 0.0, 0.0),
                    new Point(5.0, 0.0, 0.0),
                    new Point(5.0, 1.0, 0.0),
                    new Point(3.0, 1.0, 0.0),
                }
            );

            Assert.True(polygon0.DoesOverlap2D(polygon0));

            Assert.True(polygon0.DoesOverlap2D(polygon1));
            Assert.True(polygon1.DoesOverlap2D(polygon0));

            Assert.True(polygon0.DoesOverlap2D(polygon2));
            Assert.True(polygon2.DoesOverlap2D(polygon0));

            Assert.False(polygon0.DoesOverlap2D(polygon3));
            Assert.False(polygon3.DoesOverlap2D(polygon0));
        }

        [Fact]
        public void TestDoesOverlap2D_Contained()
        {
            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 1.0, 0.0),
                    new Point(0.0, 1.0, 0.0),
                }
            );

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(1.0, 0.5, 0.0),
                    new Point(1.5, 0.5, 0.0),
                    new Point(1.5, 0.75, 0.0),
                    new Point(1.0, 0.75, 0.0)
                }
            );

            Assert.True(polygon0.DoesContain2D(polygon1));

            Assert.True(polygon0.DoesOverlap2D(polygon1));
            Assert.True(polygon1.DoesOverlap2D(polygon0));
        }

        [Fact]
        public void TestDoesOverlap2D_EdgeIntersectionsOnly()
        {
            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 1.0, 0.0),
                    new Point(0.0, 1.0, 0.0),
                }
            );

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(0.5, -1.0, 0.0),
                    new Point(1.5, -1.0, 0.0),
                    new Point(1.5, 2.0, 0.0),
                    new Point(0.5, 2.0, 0.0),
                }
            );

            Assert.All(
                polygon0.Points,
                point => Assert.False(polygon1.IsPointInside2D(point))
            );
            Assert.All(
                polygon1.Points,
                point => Assert.False(polygon0.IsPointInside2D(point))
            );

            Assert.True(polygon0.DoesOverlap2D(polygon1));
            Assert.True(polygon1.DoesOverlap2D(polygon0));
        }

        [Fact]
        public void TestGetClosestPointOnEdge_ZeroPointPolygon()
        {
            Point point = new Point();

            Polygon polygon = new Polygon();

            var closestPointInfo = polygon.GetClosestPointOnEdge(point);

            Assert.Equal(
                new Tuple<int, double, Point>(
                    Polygon.InvalidEdgeIndex, 0.0, null
                ),
                closestPointInfo
            );
        }

        [Fact]
        public void TestGetClosestPointOnEdge()
        {
            const int DecimalPlaces = 6;

            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(5.0, 0.0, 0.0),
                    new Point(2.0, 3.0, 0.0),
                    new Point(0.0, 3.0, 0.0)
                }
            );

            // On polygon edge

            Point point0 = new Point(1.0, 0.0, 0.0);
            var closestPointInfo0 = polygon.GetClosestPointOnEdge(point0);
            Assert.Equal(0, closestPointInfo0.Item1);
            Assert.Equal(0.2, closestPointInfo0.Item2, DecimalPlaces);
            Assert.True(Precision.AlmostEqual(
                new Point(1.0, 0.0, 0.0).Vector,
                closestPointInfo0.Item3.Vector,
                DecimalPlaces
            ));

            Point point1 = new Point(3.0, 2.0, 0.0);
            var closestPointInfo1 = polygon.GetClosestPointOnEdge(point1);
            Assert.Equal(1, closestPointInfo1.Item1);
            Assert.Equal(2.0 / 3.0, closestPointInfo1.Item2, DecimalPlaces);
            Assert.True(Precision.AlmostEqual(
                new Point(3.0, 2.0, 0.0).Vector,
                closestPointInfo1.Item3.Vector,
                DecimalPlaces
            ));

            // Outside polygon

            Point point2 = new Point(5.0, 1.0, 0.0);
            var closestPointInfo2 = polygon.GetClosestPointOnEdge(point2);
            Assert.Equal(1, closestPointInfo2.Item1);
            Assert.Equal(1.0 / 6.0, closestPointInfo2.Item2, DecimalPlaces);
            Assert.True(Precision.AlmostEqual(
                new Point(4.5, 0.5, 0.0).Vector,
                closestPointInfo2.Item3.Vector,
                DecimalPlaces
            ));

            Point point3 = new Point(-1.0, 1.5, 0.0);
            var closestPointInfo3 = polygon.GetClosestPointOnEdge(point3);
            Assert.Equal(3, closestPointInfo3.Item1);
            Assert.Equal(0.5, closestPointInfo3.Item2, DecimalPlaces);
            Assert.True(Precision.AlmostEqual(
                new Point(0.0, 1.5, 0.0).Vector,
                closestPointInfo3.Item3.Vector,
                DecimalPlaces
            ));

            // Inside polygon
            Point point4 = new Point(2.0, 1.0, 0.0);
            var closestPointInfo4 = polygon.GetClosestPointOnEdge(point4);
            Assert.Equal(0, closestPointInfo4.Item1);
            Assert.Equal(0.4, closestPointInfo4.Item2, DecimalPlaces);
            Assert.True(Precision.AlmostEqual(
                new Point(2.0, 0.0, 0.0).Vector,
                closestPointInfo4.Item3.Vector,
                DecimalPlaces
            ));

            // Inside polygon
            Point point5 = new Point(2.0, 2.0, 0.0);
            var closestPointInfo5 = polygon.GetClosestPointOnEdge(point5);
            Assert.Equal(1, closestPointInfo5.Item1);
            Assert.Equal(5.0 / 6.0, closestPointInfo5.Item2, DecimalPlaces);
            Assert.True(Precision.AlmostEqual(
                new Point(2.5, 2.5, 0.0).Vector,
                closestPointInfo5.Item3.Vector,
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestFlip()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(0.0, 2.0, 0.0)
                }
            );

            polygon.Flip();

            Assert.Equal(
                new Polygon(
                    new[]
                    {
                        new Point(0.0, 2.0, 0.0),
                        new Point(1.0, 2.0, 0.0),
                        new Point(1.0, 0.0, 0.0),
                        new Point(0.0, 0.0, 0.0)
                    }
                ),
                polygon
            );
        }

        [Fact]
        public void TestOffsetEdges()
        {
            const int DecimalPlaces = 6;

            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 2.0, 0.0),
                    new Point(0.0, 2.0, 0.0)
                }
            );

            polygon0.OffsetEdges(1.0);

            Assert.Equal(
                new Polygon(
                    new[]
                    {
                        new Point(-1.0, -1.0, 0.0),
                        new Point(2.0, -1.0, 0.0),
                        new Point(2.0, 3.0, 0.0),
                        new Point(-1.0, 3.0, 0.0)
                    }
                ),
                polygon0
            );

            polygon0.OffsetEdges(-1.25);

            Assert.Equal(
                new Polygon(
                    new[]
                    {
                        new Point(0.25, 0.25, 0.0),
                        new Point(0.75, 0.25, 0.0),
                        new Point(0.75, 1.75, 0.0),
                        new Point(0.25, 1.75, 0.0)
                    }
                ),
                polygon0
            );

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(Math.Sqrt(12.0), 0.0, 0.0),
                    new Point(0.0, 2.0, 0.0)
                }
            );

            polygon1.OffsetEdges(-0.5);

            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 0.5, 0.5, 0.0 }),
                polygon1.Points[0].Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new DenseVector(
                    new[]
                    {
                        Math.Sqrt(12.0) - 0.5 / Math.Tan(Math.PI / 12.0),
                        0.5,
                        0.0
                    }
                ),
                polygon1.Points[1].Vector,
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                new DenseVector(
                    new[]
                    {
                        0.5,
                        2.0 - 0.5 * Math.Sqrt(3.0),
                        0.0
                    }
                ),
                polygon1.Points[2].Vector,
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestIntersect_HasIntersections()
        {
            const int DecimalPlaces = 6;

            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(5.0, 0.0, 0.0),
                    new Point(5.0, 1.0, 0.0),
                    new Point(0.0, 1.0, 0.0)
                }
            );

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(1.0, -1.0, 0.0),
                    new Point(2.0, -1.0, 0.0),
                    new Point(2.0, 3.0, 0.0),
                    new Point(1.0, 3.0, 0.0)
                }
            );

            var ppi = Polygon.EdgeIntersect(polygon0, polygon1);

            Assert.True(ppi.DoesIntersect);

            Assert.Equal(
                new[]
                {
                    (0, 1),
                    (0, 3),
                    (2, 1),
                    (2, 3),
                },
                ppi.IntersectingEdgeIndexPairs
            );

            Assert.Equal(4, ppi.EdgeIntersections.Count);
            Assert.True(Precision.AlmostEqual(
                ppi.EdgeIntersections[0].Point0.Vector,
                new DenseVector(new[] { 2.0, 0.0, 0.0 }),
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                ppi.EdgeIntersections[1].Point0.Vector,
                new DenseVector(new[] { 1.0, 0.0, 0.0 }),
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                ppi.EdgeIntersections[2].Point0.Vector,
                new DenseVector(new[] { 2.0, 1.0, 0.0 }),
                DecimalPlaces
            ));
            Assert.True(Precision.AlmostEqual(
                ppi.EdgeIntersections[3].Point0.Vector,
                new DenseVector(new[] { 1.0, 1.0, 0.0 }),
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestIntersect_NoIntersection()
        {
            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(10.0, 0.0, 0.0),
                    new Point(10.0, 5.0, 0.0),
                    new Point(0.0, 5.0, 0.0)
                }
            );

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(1.0, 1.0, 0.0),
                    new Point(5.0, 2.0, 0.0),
                    new Point(1.0, 3.0, 0.0)
                }
            );

            var ppi01 = Polygon.EdgeIntersect(polygon0, polygon1);

            Assert.False(ppi01.DoesIntersect);
            Assert.Equal(polygon0, ppi01.Polygon0);
            Assert.Equal(polygon1, ppi01.Polygon1);
            Assert.Empty(ppi01.IntersectingEdgeIndexPairs);
            Assert.Empty(ppi01.EdgeIntersections);

            var ppi10 = Polygon.EdgeIntersect(polygon1, polygon0);

            Assert.False(ppi10.DoesIntersect);
            Assert.Equal(polygon1, ppi10.Polygon0);
            Assert.Equal(polygon0, ppi10.Polygon1);
            Assert.Empty(ppi10.IntersectingEdgeIndexPairs);
            Assert.Empty(ppi10.EdgeIntersections);
        }

        [Fact]
        public void TestGetBBox()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 1.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(0.5, 3.0, -1.0)
            };

            Polygon polygon = new Polygon(points);

            BBox bbox = polygon.GetBBox();

            Assert.Equal(0.0, bbox.MinX);
            Assert.Equal(1.0, bbox.MaxX);
            Assert.Equal(0.0, bbox.MinY);
            Assert.Equal(3.0, bbox.MaxY);
            Assert.Equal(-1.0, bbox.MinZ);
            Assert.Equal(1.0, bbox.MaxZ);
        }

        [Fact]
        public void TestGetBBox_ZeroPointPolygon()
        {
            Assert.Equal(
                new BBox(),
                (new Polygon()).GetBBox()
            );
        }

        [Fact]
        public void TestTransform()
        {
            List<Point> points = new List<Point>
            {
                new Point(0.0, 0.0, 1.0),
                new Point(1.0, 0.0, 0.0),
                new Point(1.0, 2.0, 0.0),
                new Point(-1.0, 3.0, -1.0)
            };

            Polygon polygon = new Polygon(points);

            TrsTransform3D tf = new TrsTransform3D
            {
                Tz = 2.0,
                Sx = 3.0
            };

            polygon.Transform(tf);

            Assert.Equal(
                new List<Point>
                {
                    new Point(0.0, 0.0, 3.0),
                    new Point(3.0, 0.0, 2.0),
                    new Point(3.0, 2.0, 2.0),
                    new Point(-3.0, 3.0, 1.0)
                },
                polygon.Points
            );
        }

        [Theory]
        [MemberData(nameof(TestGetHashCodeAndEqualsAndOpsData))]
        public void TestGetHashCodeAndEqualsAndOps(
            Polygon pg0, Polygon pg1, bool equals
        )
        {
            Assert.Equal(pg0, pg0);
            Assert.Equal(pg0.GetHashCode(), pg0.GetHashCode());
            Assert.NotNull(pg0);

            Assert.True(pg0 == pg0);
            Assert.False(pg0 == null);
            Assert.False(null == pg0);

            Assert.False(pg0 != pg0);
            Assert.True(pg0 != null);
            Assert.True(null != pg0);

            if (equals)
            {
                Assert.Equal(pg0, pg1);
                Assert.Equal(pg1, pg0);
                Assert.Equal(pg0.GetHashCode(), pg1.GetHashCode());
                Assert.True(pg0 == pg1);
                Assert.True(pg1 == pg0);
                Assert.False(pg0 != pg1);
                Assert.False(pg1 != pg0);
            }
            else
            {
                Assert.NotEqual(pg0, pg1);
                Assert.NotEqual(pg1, pg0);
                Assert.NotEqual(pg0.GetHashCode(), pg1.GetHashCode());
                Assert.False(pg0 == pg1);
                Assert.False(pg1 == pg0);
                Assert.True(pg0 != pg1);
                Assert.True(pg1 != pg0);
            }
        }

        public static IEnumerable<object[]> TestGetHashCodeAndEqualsAndOpsData
        {
            get
            {
                Polygon pg0 = new Polygon();

                Polygon pg1 = new Polygon();

                Polygon pg2 = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(1.0, 0.0, 0.0),
                        new Point(1.0, 1.0, 0.0),
                        new Point(0.0, 1.0, 0.0)
                    }
                );

                Polygon pg3 = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(1.0, 0.0, 0.0),
                        new Point(1.0, 1.0, 0.0),
                        new Point(0.0, 1.0, 0.0)
                    }
                );

                Polygon pg4 = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(1.1, 0.0, 0.0),
                        new Point(1.0, 0.8, 0.0)
                    }
                );

                return new[]
                {
                    new object[] { pg0, pg1, true },
                    new object[] { pg2, pg3, true },
                    new object[] { pg2, pg4, false }
                };
            }
        }

        [Fact]
        public void TestEdges_ZeroPoint()
        {
            Polygon polygon = new Polygon();

            Assert.Empty(polygon.Edges);
            Assert.Equal(0, polygon.NumOfEdges);
        }

        [Fact]
        public void TestEdges_OnePoint()
        {
            Polygon polygon = new Polygon(
                new[] { new Point(1.2, 3.4, 5.6) }
            );

            Assert.Equal(
                new[]
                {
                    new LineSegment(
                        new Point(1.2, 3.4, 5.6),
                        new Point(1.2, 3.4, 5.6)
                    )
                },
                polygon.Edges
            );
            Assert.Equal(1, polygon.NumOfEdges);
        }

        [Fact]
        public void TestEdges_TwoPoints()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(1.0, 2.0, 3.0),
                    new Point(4.0, 5.0, 6.0)
                }
            );

            Assert.Equal(
                new[]
                {
                    new LineSegment(
                        new Point(1.0, 2.0, 3.0),
                        new Point(4.0, 5.0, 6.0)
                    ),
                    new LineSegment(
                        new Point(4.0, 5.0, 6.0),
                        new Point(1.0, 2.0, 3.0)
                    )
                },
                polygon.Edges
            );
            Assert.Equal(2, polygon.NumOfEdges);
        }

        [Fact]
        public void TestEdges_ThreeAndMorePoints()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 1.0, 0.0),
                    new Point(1.0, 1.5, 0.0)
                }
            );

            Assert.Equal(
                new[]
                {
                    new LineSegment(
                        new Point(0.0, 0.0, 0.0),
                        new Point(2.0, 0.0, 0.0)
                    ),
                    new LineSegment(
                        new Point(2.0, 0.0, 0.0),
                        new Point(2.0, 1.0, 0.0)
                    ),
                    new LineSegment(
                        new Point(2.0, 1.0, 0.0),
                        new Point(1.0, 1.5, 0.0)
                    ),
                    new LineSegment(
                        new Point(1.0, 1.5, 0.0),
                        new Point(0.0, 0.0, 0.0)
                    )
                },
                polygon.Edges
            );

            Assert.Equal(4, polygon.NumOfEdges);
        }

        [Fact]
        public void TestNormal_ZeroPoint()
        {
            Polygon polygon = new Polygon();

            Assert.Equal(
                new DenseVector(3),
                polygon.Normal
            );
        }

        [Fact]
        public void TestNormal_OnePoint()
        {
            Polygon polygon = new Polygon(
                new[] { new Point(1.0, 2.0, 3.0) }
            );

            Assert.Equal(
                new DenseVector(3),
                polygon.Normal
            );
        }

        [Fact]
        public void TestNormal_TwoPoints()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(1.0, 2.0, 3.0),
                    new Point(1.3, 2.2, 3.1)
                }
            );

            Assert.Equal(
                new DenseVector(3),
                polygon.Normal
            );
        }

        [Fact]
        public void TestNormal_ThreeAndMorePoints()
        {
            const int DecimalPlaces = 6;

            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.6, 0.0, 0.0),
                    new Point(0.6, 1.2, 0.0),
                    new Point(0.0, 0.0, 0.8)
                }
            );

            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 0.8, 0.0, 0.6 }),
                polygon0.Normal,
                DecimalPlaces
            ));

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(1.0, 0.0, 0.0),
                    new Point(1.0, 1.0, 0.0),
                    new Point(0.5, 2.0, 0.0)
                }
            );

            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 0.0, 0.0, 1.0 }),
                polygon1.Normal,
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestCentroid_ZeroPoint()
        {
            Polygon polygon = new Polygon();

            Assert.Equal(
                new Point(0.0, 0.0, 0.0),
                polygon.Centroid
            );
        }

        [Fact]
        public void TestCentroid_OnePoint()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(1.2, 3.4, 5.6)
                }
            );

            Assert.Equal(
                new Point(1.2, 3.4, 5.6),
                polygon.Centroid
            );
        }

        [Fact]
        public void TestCentroid_MoreThanOnePoint()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(20.0, 0.0, 0.0),
                    new Point(20.0, 16.0, 4.0),
                    new Point(0.0, 16.0, 0.0)
                }
            );

            Assert.Equal(
                new Point(10.0, 8.0, 1.0),
                polygon.Centroid
            );
        }

        [Fact]
        public void TestArea_ZeroPoint()
        {
            Polygon polygon = new Polygon();

            Assert.Equal(0.0, polygon.Area);
        }

        [Fact]
        public void TestArea_OnePoint()
        {
            Polygon polygon = new Polygon(
                new[] { new Point(1.2, 3.4, 5.6) }
            );

            Assert.Equal(0.0, polygon.Area);
        }

        [Fact]
        public void TestArea_TwoPoints()
        {
            Polygon polygon = new Polygon(
                new[]
                {
                    new Point(1.0, 2.0, 3.0),
                    new Point(5.0, 8.0, 0.0)
                }
            );

            Assert.Equal(0.0, polygon.Area);
        }

        [Fact]
        public void TestArea_ThreeAndMorePoints()
        {
            const int DecimalPlaces = 6;

            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(1.0, 2.0, 0.0),
                    new Point(4.0, 3.0, 0.0),
                    new Point(2.0, 4.0, 0.0)
                }
            );

            Assert.Equal(2.5, polygon0.Area, DecimalPlaces);

            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(0.0, 1.0, 1.0),
                    new Point(0.0, 3.0, 1.0),
                    new Point(0.0, 4.0, 1.0),
                    new Point(0.0, 5.0, 3.0),
                    new Point(0.0, 3.0, 4.0)
                }
            );

            Assert.Equal(7.0, polygon1.Area, DecimalPlaces);

            Polygon polygon2 = new Polygon(
                new[]
                {
                    new Point(2.0, 1.0, 0.0),
                    new Point(5.0, 1.0, 0.0),
                    new Point(4.0, 2.0, 0.0),
                    new Point(5.0, 3.0, 0.0),
                    new Point(1.0, 3.0, 0.0)
                }
            );

            Assert.Equal(6.0, polygon2.Area, DecimalPlaces);
        }

        [Fact]
        public void TestArea_ConcavePolygon()
        {
            const int DecimalPlaces = 6;

            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(1.0, 0.0, 0.0),
                    new Point(6.0, 0.0, 0.0),
                    new Point(6.0, 9.0, 0.0),
                    new Point(2.0, 9.0, 0.0),
                    new Point(2.0, 6.0, 0.0),
                    new Point(1.0, 6.0, 0.0)
                }
            );

            Assert.Equal(42.0, polygon0.Area, DecimalPlaces);
        }

        [Fact]
        public void TestSerialization()
        {
            var polygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 1.0, 0.0),
                    new Point(0.0, 0.5, 0.0)
                }
            );

            var polygonItem = new HelperWorkspaceItem<Polygon>(polygon);

            var workspace = new JsonWorkspace();
            workspace.Save(polygonItem);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedItem =
                workspaceCopy.Load<HelperWorkspaceItem<Polygon>>(
                    polygonItem.ItemInfo.Uid
                );

            Assert.Equal(polygon, deserializedItem.Data);
        }

        #endregion

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
