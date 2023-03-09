using System;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Scene;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestFlatRoofComponent
    {
        [Fact]
        public void TestConstructor()
        {
            FlatRoofComponent frc = new FlatRoofComponent();

            Assert.Equal(
                FlatRoofComponent.DefaultHeight,
                frc.Height
            );

            Assert.Equal(
                FlatRoofComponent.DefaultEaveLength,
                frc.EaveLength
            );
        }

        [Fact]
        public void TestGetPlan()
        {
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

            FlatRoofComponent frc = new FlatRoofComponent
            {
                Height = 1.0,
                EaveLength = 1.0
            };

            Assert.Equal(
                new Polygon(
                    new[]
                    {
                        new Point(0.0, -1.0, 0.0),
                        new Point(5.0, -1.0, 0.0),
                        new Point(5.0, 10.0, 0.0),
                        new Point(0.0, 10.0, 0.0)
                    }
                ),
                frc.GetPlan()
            );
        }

        [Fact]
        public void TestGetPlan_NotAttachedToUnit()
        {
            FlatRoofComponent frc = new FlatRoofComponent();

            SceneObject nonunit = new SceneObject();
            nonunit.AddComponent(frc);

            Assert.ThrowsAny<InvalidOperationException>(
                () => { Polygon roofPlan = frc.GetPlan(); }
            );
        }

        [Fact]
        public void TestHeight()
        {
            FlatRoofComponent frc = new FlatRoofComponent();

            frc.Height = 1.2;
            Assert.Equal(1.2, frc.Height);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { frc.Height = 0.0; }
            );
            Assert.Equal(1.2, frc.Height);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { frc.Height = -1.0; }
            );
            Assert.Equal(1.2, frc.Height);
        }

        [Fact]
        public void TestEaveLength()
        {
            FlatRoofComponent frc = new FlatRoofComponent();

            frc.EaveLength = 0.0;
            Assert.Equal(0.0, frc.EaveLength);

            frc.EaveLength = 1.2;
            Assert.Equal(1.2, frc.EaveLength);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { frc.EaveLength = -1.0; }
            );
            Assert.Equal(1.2, frc.EaveLength);
        }
    }
}
