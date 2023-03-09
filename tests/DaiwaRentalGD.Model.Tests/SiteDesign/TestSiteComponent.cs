using System;
using System.Collections.Generic;
using System.Linq;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Model.Tests.SiteDesign
{
    public class TestSiteComponent
    {
        #region Constructors

        public TestSiteComponent(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConstructor()
        {
            SiteComponent sc = new SiteComponent();

            Assert.Equal(new Polygon(), sc.Boundary);
            Assert.Empty(sc.BoundaryEdgeTypes);

            Assert.Empty(sc.PropertyEdges);
            Assert.Empty(sc.PropertyEdgeIndices);

            Assert.Empty(sc.RoadEdges);
            Assert.Empty(sc.RoadEdgeIndices);

            Assert.Empty(sc.OppositeRoadEdges);

            Assert.Equal(0.0, sc.TrueNorthAngle);
            Assert.Equal(
                new DenseVector(new[] { 0.0, 1.0, 0.0 }),
                sc.TrueNorthDirection
            );
        }

        [Fact]
        public void TestSetEdgeType()
        {
            SiteComponent sc = new SiteComponent
            {
                Boundary = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(20.0, 0.0, 0.0),
                        new Point(15.0, 10.0, 0.0),
                        new Point(0.0, 10.0, 0.0)
                    }
                )
            };

            Assert.Equal(
                Enumerable.Repeat(SiteEdgeType.Unknown, 4),
                sc.BoundaryEdgeTypes
            );

            sc.SetBoundaryEdgeType(0, SiteEdgeType.Road);
            sc.SetBoundaryEdgeType(1, SiteEdgeType.Property);
            sc.SetBoundaryEdgeType(3, SiteEdgeType.Property);

            Assert.Equal(
                new[]
                {
                    SiteEdgeType.Road,
                    SiteEdgeType.Property,
                    SiteEdgeType.Unknown,
                    SiteEdgeType.Property
                },
                sc.BoundaryEdgeTypes
            );

            Assert.Equal(
                new[] { 0 },
                sc.RoadEdgeIndices
            );
            Assert.Equal(
                new[] { sc.Boundary.Edges[0] },
                sc.RoadEdges
            );

            Assert.Equal(
                new[] { 1, 3 },
                sc.PropertyEdgeIndices
            );
            Assert.Equal(
                new[] { sc.Boundary.Edges[1], sc.Boundary.Edges[3] },
                sc.PropertyEdges
            );
        }

        [Fact]
        public void TestInsertOppositeRoadEdge()
        {
            SiteComponent sc = new SiteComponent();

            sc.AddOppositeRoadEdge(new LineSegment(
                new Point(1.0, 0.0, 0.0),
                new Point(10.0, 1.0, 0.0)
            ));
            sc.AddOppositeRoadEdge(new LineSegment(
                new Point(10.0, 2.0, 0.0),
                new Point(11.0, 20.0, 0.0)
            ));
            sc.InsertOppositeRoadEdge(1, new LineSegment(
                new Point(0.0, 0.5, 0.0),
                new Point(1.0, 6.0, 0.0)
            ));

            Assert.Equal(
                new[]
                {
                    new LineSegment(
                        new Point(1.0, 0.0, 0.0),
                        new Point(10.0, 1.0, 0.0)
                    ),
                    new LineSegment(
                        new Point(0.0, 0.5, 0.0),
                        new Point(1.0, 6.0, 0.0)
                    ),
                    new LineSegment(
                        new Point(10.0, 2.0, 0.0),
                        new Point(11.0, 20.0, 0.0)
                    )
                },
                sc.OppositeRoadEdges
            );
        }

        [Fact]
        public void TestInsertOppositeRoadEdge_NullEdge()
        {
            SiteComponent sc = new SiteComponent();

            sc.AddOppositeRoadEdge(new LineSegment(
                new Point(1.0, 0.0, 0.0),
                new Point(10.0, 1.0, 0.0)
            ));

            Assert.Throws<ArgumentNullException>(
                () => { sc.AddOppositeRoadEdge(null); }
            );

            Assert.Equal(
                new[]
                {
                    new LineSegment(
                        new Point(1.0, 0.0, 0.0),
                        new Point(10.0, 1.0, 0.0)
                    )
                },
                sc.OppositeRoadEdges
            );
        }

        [Fact]
        public void TestRemoveOppositeRoadEdge()
        {
            SiteComponent sc = new SiteComponent();

            sc.AddOppositeRoadEdge(new LineSegment(
                new Point(1.0, 0.0, 0.0),
                new Point(10.0, 1.0, 0.0)
            ));
            sc.AddOppositeRoadEdge(new LineSegment(
                new Point(10.0, 2.0, 0.0),
                new Point(11.0, 20.0, 0.0)
            ));
            sc.InsertOppositeRoadEdge(1, new LineSegment(
                new Point(0.0, 0.5, 0.0),
                new Point(1.0, 6.0, 0.0)
            ));

            sc.RemoveOppositeRoadEdge(0);

            Assert.Equal(
                new[]
                {
                    new LineSegment(
                        new Point(0.0, 0.5, 0.0),
                        new Point(1.0, 6.0, 0.0)
                    ),
                    new LineSegment(
                        new Point(10.0, 2.0, 0.0),
                        new Point(11.0, 20.0, 0.0)
                    )
                },
                sc.OppositeRoadEdges
            );
        }

        [Fact]
        public void TestGetOppositeRoadEdgeNormal()
        {
            const int DecimalPlaces = 6;

            SiteComponent sc = new SiteComponent
            {
                Boundary = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(10.0, 0.0, 0.0),
                        new Point(8.0, 8.0, 0.0),
                        new Point(1.0, 8.0, 0.0)
                    }
                )
            };
            sc.AddOppositeRoadEdge(new LineSegment(
                new Point(-1.0, -5.0, 0.0),
                new Point(12.0, -5.0, 0.0)
            ));
            sc.AddOppositeRoadEdge(new LineSegment(
                new Point(12.0, -5.0, 0.0),
                new Point(-1.0, -5.0, 0.0)
            ));
            sc.AddOppositeRoadEdge(new LineSegment(
                new Point(-1.0, 0.0, 0.0),
                new Point(-1.0, 10.0, 0.0)
            ));

            Assert.True(new DenseVector(new[] { 0.0, 1.0, 0.0 }).AlmostEqual(
                sc.GetOppositeRoadEdgeNormal(0),
                DecimalPlaces
            ));
            Assert.True(new DenseVector(new[] { 0.0, 1.0, 0.0 }).AlmostEqual(
                sc.GetOppositeRoadEdgeNormal(1),
                DecimalPlaces
            ));
            Assert.True(new DenseVector(new[] { 1.0, 0.0, 0.0 }).AlmostEqual(
                sc.GetOppositeRoadEdgeNormal(2),
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestIsNorthEdge_GetNorthEdgeIndices()
        {
            SiteComponent sc = new SiteComponent
            {
                Boundary = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(10.0, 0.0, 0.0),
                        new Point(9.99, 10.0, 0.0),
                        new Point(0.0, 5.0, 0.0)
                    }
                )
            };

            Assert.False(sc.IsNorthEdge(0, true));
            Assert.False(sc.IsNorthEdge(0, false));

            Assert.True(sc.IsNorthEdge(1, true));
            Assert.True(sc.IsNorthEdge(1, false));

            Assert.True(sc.IsNorthEdge(2, true));
            Assert.True(sc.IsNorthEdge(2, false));

            Assert.False(sc.IsNorthEdge(3, true));
            Assert.True(sc.IsNorthEdge(3, false));

            Assert.Equal(
                new[] { 1, 2 },
                sc.GetNorthEdgeIndices(true)
            );
            Assert.Equal(
                new[] { 1, 2, 3 },
                sc.GetNorthEdgeIndices(false)
            );
        }

        [Fact]
        public void TestGetBBox()
        {
            SiteComponent sc = new SiteComponent();

            sc.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(20.0, 0.0, 0.0),
                    new Point(15.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            );

            sc.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(-1.0, -4.0, 0.0),
                    new Point(22.0, -5.0, 0.0)
                )
            );

            Assert.Equal(
                BBox.FromMinAndMax(
                    -1.0, -5.0, 0.0,
                    22.0, 10.0, 0.0
                ),
                sc.GetBBox()
            );
        }

        [Fact]
        public void TestBoundary()
        {
            SiteComponent sc = new SiteComponent();

            Polygon boundary0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(20.0, 0.0, 0.0),
                    new Point(15.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            );

            sc.Boundary = boundary0;

            Assert.Equal(boundary0, sc.Boundary);
            Assert.Equal(
                Enumerable.Repeat(SiteEdgeType.Unknown, 4),
                sc.BoundaryEdgeTypes
            );

            sc.SetBoundaryEdgeType(1, SiteEdgeType.Property);
            Assert.Equal(
                new[]
                {
                    SiteEdgeType.Unknown,
                    SiteEdgeType.Property,
                    SiteEdgeType.Unknown,
                    SiteEdgeType.Unknown
                },
                sc.BoundaryEdgeTypes
            );

            Polygon boundary1 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(15.0, 5.0, 0.0),
                    new Point(5.0, 10.0, 0.0)
                }
            );

            sc.Boundary = boundary1;

            Assert.Equal(boundary1, sc.Boundary);
            Assert.Equal(
                Enumerable.Repeat(SiteEdgeType.Unknown, 3),
                sc.BoundaryEdgeTypes
            );
        }

        [Fact]
        public void TestBoundary_SetNull()
        {
            SiteComponent sc = new SiteComponent();

            Polygon boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(20.0, 0.0, 0.0),
                    new Point(15.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            );

            sc.Boundary = boundary;
            Assert.Equal(boundary, sc.Boundary);

            sc.SetBoundaryEdgeType(1, SiteEdgeType.Property);
            Assert.Equal(
                new[]
                {
                    SiteEdgeType.Unknown,
                    SiteEdgeType.Property,
                    SiteEdgeType.Unknown,
                    SiteEdgeType.Unknown
                },
                sc.BoundaryEdgeTypes
            );


            Assert.Throws<ArgumentNullException>(
                () => { sc.Boundary = null; }
            );

            Assert.Equal(boundary, sc.Boundary);
            Assert.Equal(
                new[]
                {
                    SiteEdgeType.Unknown,
                    SiteEdgeType.Property,
                    SiteEdgeType.Unknown,
                    SiteEdgeType.Unknown
                },
                sc.BoundaryEdgeTypes
            );
        }

        [Theory]
        [MemberData(nameof(TestTrueNorthAngleData))]
        public void TestTrueNorthAngle(
            double trueNorthAngle, Vector<double> trueNorthDir
        )
        {
            const int DecimalPlaces = 6;

            var sc = new SiteComponent();

            sc.TrueNorthAngle = trueNorthAngle;

            Assert.Equal(
                trueNorthAngle,
                sc.TrueNorthAngle,
                DecimalPlaces
            );
            Assert.True(trueNorthDir.AlmostEqual(
                sc.TrueNorthDirection,
                DecimalPlaces
            ));
        }

        public static IEnumerable<object[]> TestTrueNorthAngleData
        {
            get => new[]
            {
                new object[]
                {
                    -Math.PI / 3.0,
                    new DenseVector(new[] { Math.Sqrt(3.0) / 2.0, 0.5, 0.0 })
                },
                new object[]
                {
                    0.0,
                    new DenseVector(new[] { 0.0, 1.0, 0.0 })
                },
                new object[]
                {
                    Math.PI / 3.0,
                    new DenseVector(new[] { -Math.Sqrt(3.0) / 2.0, 0.5, 0.0 })
                },
                new object[]
                {
                    Math.PI / 3.0 * 2.0,
                    new DenseVector(
                        new[] { -Math.Sqrt(3.0) / 2.0, -0.5, 0.0 }
                    )
                },
                new object[]
                {
                    Math.PI,
                    new DenseVector(new[] { 0.0, -1.0, 0.0 })
                },
                new object[]
                {
                    Math.PI / 3.0 * 4.0,
                    new DenseVector(
                        new[] { Math.Sqrt(3.0) / 2.0, -0.5, 0.0 }
                    )
                },
                new object[]
                {
                    -Math.PI / 3.0 * 2.0,
                    new DenseVector(
                        new[] { Math.Sqrt(3.0) / 2.0, -0.5, 0.0 }
                    )
                },
            };
        }

        [Theory]
        [MemberData(nameof(TestTrueNorthDirectionData))]
        public void TestTrueNorthDirection(
            Vector<double> trueNorthDir, double trueNorthAngle
        )
        {
            const int DecimalPlaces = 6;

            var sc = new SiteComponent();

            sc.TrueNorthDirection = trueNorthDir;

            Assert.True(trueNorthDir.Normalize(2.0).AlmostEqual(
                sc.TrueNorthDirection,
                DecimalPlaces
            ));
            Assert.Equal(
                trueNorthAngle,
                sc.TrueNorthAngle,
                DecimalPlaces
            );
        }

        public static IEnumerable<object[]> TestTrueNorthDirectionData
        {
            get => new[]
            {
                new object[]
                {
                    new DenseVector(new[] { Math.Sqrt(3.0) / 2.0, 0.5, 0.0 }),
                    -Math.PI / 3.0
                },
                new object[]
                {
                    new DenseVector(new[] { Math.Sqrt(3.0), 1.0, 0.0 }),
                    -Math.PI / 3.0
                },
                new object[]
                {
                    new DenseVector(new[] { 0.0, 1.0, 0.0 }),
                    0.0
                },
                new object[]
                {
                    new DenseVector(
                        new[] { -Math.Sqrt(3.0) / 2.0, 0.5, 0.0 }
                    ),
                    Math.PI / 3.0
                },
                new object[]
                {
                    new DenseVector(
                        new[] { -Math.Sqrt(3.0) / 2.0, -0.5, 0.0 }
                    ),
                    Math.PI / 3.0 * 2.0
                },
                new object[]
                {
                    new DenseVector(new[] { 0.0, -1.0, 0.0 }),
                    Math.PI
                },
                new object[]
                {
                    new DenseVector(
                        new[] { Math.Sqrt(3.0) / 2.0, -0.5, 0.0 }
                    ),
                    -Math.PI / 3.0 * 2.0
                }
            };
        }

        [Fact]
        public void TestTrueNorthDirection_ZeroVector()
        {
            const int DecimalPlaces = 6;

            var sc = new SiteComponent();

            sc.TrueNorthDirection = new DenseVector(new[] { 1.0, 2.0, 0.0 });

            Assert.ThrowsAny<ArgumentException>(
                () =>
                {
                    sc.TrueNorthDirection =
                        new DenseVector(new[] { 0.0, 0.0, 0.0 });
                }
            );

            Assert.True(new DenseVector(new[] { 1.0, 2.0, 0.0 }).Normalize(2.0).AlmostEqual(
                sc.TrueNorthDirection,
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestSerialization()
        {
            SiteComponent siteComponent = new SiteComponent
            {
                Boundary = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(20.0, 0.0, 0.0),
                        new Point(15.0, 10.0, 0.0),
                        new Point(0.0, 10.0, 0.0)
                    }
                ),
                TrueNorthAngle = Math.PI / 6.0
            };

            siteComponent.SetBoundaryEdgeType(1, SiteEdgeType.Property);

            siteComponent.AddOppositeRoadEdge(new LineSegment(
                new Point(30.0, 0.0, 0.0),
                new Point(25.0, 10.0, 0.0)
            ));

            var workspace = new JsonWorkspace();
            workspace.Save(siteComponent);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var siteComponentCopy =
                workspaceCopy.Load<SiteComponent>(siteComponent.ItemInfo.Uid);

            Assert.Equal(
                siteComponent.Boundary,
                siteComponentCopy.Boundary
            );

            Assert.Equal(
                siteComponent.BoundaryEdgeTypes,
                siteComponentCopy.BoundaryEdgeTypes
            );

            Assert.Equal(
                siteComponent.OppositeRoadEdges,
                siteComponentCopy.OppositeRoadEdges
            );

            Assert.Equal(
                siteComponent.TrueNorthAngle,
                siteComponentCopy.TrueNorthAngle
            );
        }

        #endregion

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
