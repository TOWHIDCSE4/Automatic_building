using System;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestGableRoofComponent
    {
        [Fact]
        public void TestConstructor()
        {
            GableRoofComponent grc = new GableRoofComponent();

            Assert.Equal(
                GableRoofComponent.DefaultEaveHeight,
                grc.EaveHeight
            );
            Assert.Equal(
                GableRoofComponent.DefaultEaveLength,
                grc.EaveLength
            );
            Assert.Equal(
                GableRoofComponent.DefaultSlopeAngle,
                grc.SlopeAngle
            );
        }

        [Fact]
        public void TestGetSizes()
        {
            const int DecimalPlaces = 6;

            Unit unit = new Unit();

            unit.UnitComponent.AddRoomPlan(new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(5.0, 0.0, 0.0),
                    new Point(5.0, 9.0, 0.0),
                    new Point(0.0, 9.0, 0.0)
                }
            ));

            GableRoofComponent grc = new GableRoofComponent
            {
                EaveHeight = 0.5,
                EaveLength = 1.0,
                SlopeAngle = Math.PI / 4.0
            };

            Assert.True(new DenseVector(new[] { 5.0, 11.0, 6.0 }).AlmostEqual(
                grc.GetSizes(),
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestGetSizes_NotAttachedToUnit()
        {
            GableRoofComponent grc = new GableRoofComponent();

            Assert.ThrowsAny<InvalidOperationException>(
                () => { Vector<double> sizes = grc.GetSizes(); }
            );
        }

        [Fact]
        public void TestGetSection()
        {
            const int DecimalPlaces = 6;

            Unit unit = new Unit();

            unit.UnitComponent.AddRoomPlan(new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(5.0, 0.0, 0.0),
                    new Point(5.0, 9.0, 0.0),
                    new Point(0.0, 9.0, 0.0)
                }
            ));

            GableRoofComponent grc = new GableRoofComponent
            {
                EaveHeight = 0.5,
                EaveLength = 1.0,
                SlopeAngle = Math.PI / 4.0
            };

            Polygon expectedSection = new Polygon(
                new[]
                {
                    new Point(0.0, -1.0, 0.0),
                    new Point(0.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.5),
                    new Point(0.0, 4.5, 6.0),
                    new Point(0.0, -1.0, 0.5)
                }
            );

            Polygon section = grc.GetSection();

            Assert.Equal(
                expectedSection.Points.Count,
                section.Points.Count
            );

            for (
                int pointIndex = 0; pointIndex < section.Points.Count;
                ++pointIndex
            )
            {
                Point expectedPoint = expectedSection.Points[pointIndex];
                Point point = section.Points[pointIndex];

                Assert.True(expectedPoint.Vector.AlmostEqual(
                    point.Vector,
                    DecimalPlaces
                ));
            }
        }

        [Fact]
        public void TestGetSection_NotAttachedToUnit()
        {
            GableRoofComponent grc = new GableRoofComponent();

            Assert.ThrowsAny<InvalidOperationException>(
                () => { Polygon section = grc.GetSection(); }
            );
        }

        [Fact]
        public void TestEaveHeight()
        {
            GableRoofComponent grc = new GableRoofComponent();

            grc.EaveHeight = 0.0;
            Assert.Equal(0.0, grc.EaveHeight);

            grc.EaveHeight = 1.2;
            Assert.Equal(1.2, grc.EaveHeight);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { grc.EaveHeight = -1.0; }
            );
            Assert.Equal(1.2, grc.EaveHeight);
        }

        [Fact]
        public void TestEaveLength()
        {
            GableRoofComponent grc = new GableRoofComponent();

            grc.EaveLength = 0.0;
            Assert.Equal(0.0, grc.EaveLength);

            grc.EaveLength = 1.2;
            Assert.Equal(1.2, grc.EaveLength);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { grc.EaveLength = -1.0; }
            );
            Assert.Equal(1.2, grc.EaveLength);
        }

        [Fact]
        public void TestSlopeAngle()
        {
            GableRoofComponent grc = new GableRoofComponent();

            grc.SlopeAngle = 0.0;
            Assert.Equal(0.0, grc.SlopeAngle);

            grc.SlopeAngle = Math.PI / 4.0;
            Assert.Equal(Math.PI / 4.0, grc.SlopeAngle);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { grc.SlopeAngle = -1.0; }
            );
            Assert.Equal(Math.PI / 4.0, grc.SlopeAngle);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { grc.SlopeAngle = Math.PI / 2.0; }
            );
            Assert.Equal(Math.PI / 4.0, grc.SlopeAngle);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { grc.SlopeAngle = Math.PI * 0.75; }
            );
            Assert.Equal(Math.PI / 4.0, grc.SlopeAngle);
        }
    }
}
