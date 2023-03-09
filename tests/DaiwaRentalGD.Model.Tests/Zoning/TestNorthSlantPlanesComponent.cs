using System;
using System.Collections.Generic;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Model.Zoning;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.Zoning
{
    public class TestNorthSlantPlanesComponent
    {
        [Fact]
        public void TestConstructor()
        {
            NorthSlantPlanesComponent nspc =
                new NorthSlantPlanesComponent();

            Assert.Equal(
                NorthSlantPlanesComponent.DefaultSlopeStartHeight,
                nspc.SlopeStartHeight
            );
            Assert.Equal(
                NorthSlantPlanesComponent.DefaultSlopeAngleTangent,
                nspc.SlopeAngleTangent
            );
        }

        [Fact]
        public void TestGetNorthPropertyEdgeIndices()
        {
            Site site = new Site();
            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(20.0, 0.0, 0.0),
                    new Point(10.0, 10.0, 0.0)
                }
            );
            site.SiteComponent.SetBoundaryEdgeType(0, SiteEdgeType.Property);
            site.SiteComponent.SetBoundaryEdgeType(1, SiteEdgeType.Road);
            site.SiteComponent.SetBoundaryEdgeType(2, SiteEdgeType.Property);

            NorthSlantPlanesComponent nspc = new NorthSlantPlanesComponent
            {
                Site = site
            };

            Assert.Equal(
                new[] { 2 },
                nspc.GetNorthPropertyEdgeIndices()
            );
        }

        [Fact]
        public void TestGetEdgeVerticalPlane()
        {
            Site site = new Site();
            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(20.0, 0.0, 0.0),
                    new Point(12.0, 10.0, 0.0)
                }
            );

            NorthSlantPlanesComponent nspc = new NorthSlantPlanesComponent
            {
                SlopeStartHeight = 5.0,
                Site = site
            };

            Assert.Equal(
                new Polygon(
                    new[]
                    {
                        new Point(20.0, 0.0, 0.0),
                        new Point(12.0, 10.0, 0.0),
                        new Point(12.0, 10.0, 5.0),
                        new Point(20.0, 0.0, 5.0)
                    }
                ),
                nspc.GetEdgeVerticalPlane(1)
            );

            Assert.Equal(
                new Polygon(
                    new[]
                    {
                        new Point(12.0, 10.0, 0.0),
                        new Point(0.0, 0.0, 0.0),
                        new Point(0.0, 0.0, 5.0),
                        new Point(12.0, 10.0, 5.0)
                    }
                ),
                nspc.GetEdgeVerticalPlane(2)
            );
        }

        [Fact]
        public void TestGetProjectedSlopeLength()
        {
            Site site = new Site();
            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(2.0, 1.0, 0.0),
                    new Point(40.0, 2.0, 0.0),
                    new Point(30.0, 30.0, 0.0),
                    new Point(2.0, 30.0, 0.0)
                }
            );

            NorthSlantPlanesComponent nspc = new NorthSlantPlanesComponent
            {
                SlopeAngleTangent = 4.0 / 3.0,
                Site = site
            };

            Assert.Equal(0.0, nspc.GetSlopeProjectedLength(0));
            Assert.Equal(1.0, nspc.GetSlopeProjectedLength(1));
            Assert.Equal(29.0, nspc.GetSlopeProjectedLength(2));
            Assert.Equal(29.0, nspc.GetSlopeProjectedLength(3));
        }

        [Fact]
        public void TestGetEdgeSlopePlane()
        {
            Site site = new Site();
            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(40.0, 0.0, 0.0),
                    new Point(0.0, 30.0, 0.0)
                }
            );

            NorthSlantPlanesComponent nspc = new NorthSlantPlanesComponent
            {
                SlopeStartHeight = 5.0,
                SlopeAngleTangent = 0.5,
                Site = site
            };

            Assert.Equal(
                new Polygon(
                    new[]
                    {
                        new Point(40.0, 0.0, 5.0),
                        new Point(0.0, 30.0, 5.0),
                        new Point(0.0, 0.0, 5.0 + 0.5 * 30.0),
                        new Point(40.0, 0.0, 5.0)
                    }
                ),
                nspc.GetEdgeSlopePlane(1)
            );
        }

        [Fact]
        public void TestGetEdgePlanes()
        {
            Site site = new Site();
            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(40.0, 0.0, 0.0),
                    new Point(0.0, 30.0, 0.0)
                }
            );
            site.SiteComponent.SetBoundaryEdgeType(0, SiteEdgeType.Property);
            site.SiteComponent.SetBoundaryEdgeType(1, SiteEdgeType.Property);
            site.SiteComponent.SetBoundaryEdgeType(2, SiteEdgeType.Road);

            NorthSlantPlanesComponent nspc = new NorthSlantPlanesComponent
            {
                SlopeStartHeight = 5.0,
                SlopeAngleTangent = 0.75,
                Site = site
            };

            Assert.Equal(
                new[]
                {
                    new Polygon(
                        new[]
                        {
                            new Point(40.0, 0.0, 0.0),
                            new Point(0.0, 30.0, 0.0),
                            new Point(0.0, 30.0, 5.0),
                            new Point(40.0, 0.0, 5.0)
                        }
                    ),
                    new Polygon(
                        new[]
                        {
                            new Point(40.0, 0.0, 5.0),
                            new Point(0.0, 30.0, 5.0),
                            new Point(0.0, 0.0, 5.0 + 0.75 * 30.0),
                            new Point(40.0, 0.0, 5.0)
                        }
                    )
                },
                nspc.GetEdgePlanes(1)
            );
        }

        [Fact]
        public void TestUpdatePlanes()
        {
            Site site = new Site();
            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(40.0, 0.0, 0.0),
                    new Point(0.0, 30.0, 0.0)
                }
            );
            site.SiteComponent.SetBoundaryEdgeType(0, SiteEdgeType.Property);
            site.SiteComponent.SetBoundaryEdgeType(1, SiteEdgeType.Property);
            site.SiteComponent.SetBoundaryEdgeType(2, SiteEdgeType.Road);

            NorthSlantPlanesComponent nspc = new NorthSlantPlanesComponent
            {
                SlopeStartHeight = 5.0,
                SlopeAngleTangent = 0.75,
                Site = site
            };

            nspc.UpdatePlanes();

            Assert.Equal(
                new Dictionary<int, IReadOnlyList<Polygon>>
                {
                    {
                        1,
                        new[]
                        {
                            new Polygon(
                                new[]
                                {
                                    new Point(40.0, 0.0, 0.0),
                                    new Point(0.0, 30.0, 0.0),
                                    new Point(0.0, 30.0, 5.0),
                                    new Point(40.0, 0.0, 5.0)
                                }
                            ),
                            new Polygon(
                                new[]
                                {
                                    new Point(40.0, 0.0, 5.0),
                                    new Point(0.0, 30.0, 5.0),
                                    new Point(0.0, 0.0, 5.0 + 0.75 * 30.0),
                                    new Point(40.0, 0.0, 5.0)
                                }
                            )
                        }
                    }
                },
                nspc.PlanesByEdge
            );
        }

        [Fact]
        public void TestIsPointInEdgePlanesRange()
        {
            Site site = new Site();
            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(40.0, 0.0, 0.0),
                    new Point(40.0, 30.0, 0.0),
                    new Point(15.0, 30.0, 0.0),
                    new Point(15.0, 20.0, 0.0),
                    new Point(0.0, 20.0, 0.0)
                }
            );

            NorthSlantPlanesComponent nspc = new NorthSlantPlanesComponent
            {
                Site = site
            };

            Assert.True(
                nspc.IsPointInEdgePlanesRange(2, new Point(20.0, 10.0, 0.0))
            );
            Assert.True(
                nspc.IsPointInEdgePlanesRange(2, new Point(40.0, 10.0, 0.0))
            );
            Assert.True(
                nspc.IsPointInEdgePlanesRange(2, new Point(40.0, 30.0, 0.0))
            );
            Assert.False(
                nspc.IsPointInEdgePlanesRange(2, new Point(10.0, 10.0, 0.0))
            );
            Assert.False(
                nspc.IsPointInEdgePlanesRange(2, new Point(10.0, 100.0, 0.0))
            );
            Assert.False(
                nspc.IsPointInEdgePlanesRange(2, new Point(100.0, 10.0, 0.0))
            );
            Assert.False(
                nspc.IsPointInEdgePlanesRange(2, new Point(100.0, 100.0, 0.0))
            );
            Assert.False(
                nspc.IsPointInEdgePlanesRange(2, new Point(20.0, 100.0, 0.0))
            );
            Assert.False(
                nspc.IsPointInEdgePlanesRange(2, new Point(15.0, 100.0, 0.0))
            );
        }

        [Fact]
        public void TestViolates()
        {
            Site site = new Site();
            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(40.0, 0.0, 0.0),
                    new Point(40.0, 30.0, 0.0),
                    new Point(15.0, 30.0, 0.0),
                    new Point(15.0, 20.0, 0.0),
                    new Point(0.0, 20.0, 0.0)
                }
            );
            site.SiteComponent.SetBoundaryEdgeType(0, SiteEdgeType.Road);
            site.SiteComponent.SetBoundaryEdgeType(1, SiteEdgeType.Road);
            site.SiteComponent.SetBoundaryEdgeType(2, SiteEdgeType.Road);
            site.SiteComponent.SetBoundaryEdgeType(3, SiteEdgeType.Property);
            site.SiteComponent.SetBoundaryEdgeType(4, SiteEdgeType.Property);
            site.SiteComponent.SetBoundaryEdgeType(5, SiteEdgeType.Road);

            NorthSlantPlanesComponent nspc = new NorthSlantPlanesComponent
            {
                SlopeStartHeight = 5.0,
                SlopeAngleTangent = 0.75,
                Site = site
            };

            nspc.UpdatePlanes();

            Assert.Empty(
                nspc.GetViolation(new Point(20.0, 25.0, 100.0))
                .NorthPropertyEdgeIndices
            );
            Assert.Empty(
                nspc.GetViolation(new Point(20.0, 40.0, 0.0))
                .NorthPropertyEdgeIndices
            );
            Assert.Empty(
                nspc.GetViolation(new Point(10.0, 10.0, 5.0))
                .NorthPropertyEdgeIndices
            );
            Assert.Equal(
                new[] { 4 },
                nspc.GetViolation(new Point(10.0, 10.0, 100.0))
                .NorthPropertyEdgeIndices
            );
            Assert.Empty(
                nspc.GetViolation(new Point(10.0, 100.0, 0.0))
                .NorthPropertyEdgeIndices
            );
        }

        [Fact]
        public void TestSlopeStartHeight()
        {
            NorthSlantPlanesComponent nspc =
                new NorthSlantPlanesComponent();

            nspc.SlopeStartHeight = 0.1;
            Assert.Equal(0.1, nspc.SlopeStartHeight);

            nspc.SlopeStartHeight = 12.0;
            Assert.Equal(12.0, nspc.SlopeStartHeight);

            nspc.SlopeStartHeight = 0.0;
            Assert.Equal(0.0, nspc.SlopeStartHeight);
        }

        [Fact]
        public void TestSlopeStartHeight_InvalidValue()
        {
            NorthSlantPlanesComponent nspc =
                new NorthSlantPlanesComponent();

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { nspc.SlopeStartHeight = -0.1; }
            );

            Assert.Equal(
                NorthSlantPlanesComponent.DefaultSlopeStartHeight,
                nspc.SlopeStartHeight
            );
        }

        [Fact]
        public void TestSlopeAngleTangent()
        {
            NorthSlantPlanesComponent nspc =
                new NorthSlantPlanesComponent();

            nspc.SlopeAngleTangent = 0.5;
            Assert.Equal(0.5, nspc.SlopeAngleTangent);

            nspc.SlopeAngleTangent = 1.0;
            Assert.Equal(1.0, nspc.SlopeAngleTangent);

            nspc.SlopeAngleTangent = 2.6;
            Assert.Equal(2.6, nspc.SlopeAngleTangent);
        }

        [Fact]
        public void TestSlopeAngleTangent_InvalidValue()
        {
            NorthSlantPlanesComponent nspc =
                new NorthSlantPlanesComponent();

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { nspc.SlopeAngleTangent = -0.1; }
            );

            Assert.Equal(
                NorthSlantPlanesComponent.DefaultSlopeAngleTangent,
                nspc.SlopeAngleTangent
            );
        }
    }
}
