using System;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestBulidingEntrance
    {
        [Fact]
        public void TestConstructor()
        {
            BuildingEntrance entrance = new BuildingEntrance();

            Assert.Equal(
                BuildingEntrance.DefaultEntrancePoint,
                entrance.EntrancePoint
            );
            Assert.Equal(
                BuildingEntrance.DefaultEntranceDirection,
                entrance.EntranceDirection
            );
        }

        [Fact]
        public void TestEntrancePoint()
        {
            BuildingEntrance entrance = new BuildingEntrance();

            Point entrancePoint = new Point(1.0, 2.0, 3.0);
            entrance.EntrancePoint = entrancePoint;

            Assert.Equal(entrancePoint, entrance.EntrancePoint);
        }

        [Fact]
        public void TestEntrancePoint_SetNull()
        {
            BuildingEntrance entrance = new BuildingEntrance();

            Point entrancePoint = new Point(1.0, 2.0, 3.0);
            entrance.EntrancePoint = entrancePoint;

            Assert.Throws<ArgumentNullException>(
                () => { entrance.EntrancePoint = null; }
            );

            Assert.Equal(entrancePoint, entrance.EntrancePoint);
        }

        [Fact]
        public void TestEntranceDirection()
        {
            BuildingEntrance entrance = new BuildingEntrance();

            Vector<double> entranceDirection =
                new DenseVector(new[] { 0.0, 1.0, 0.0 });
            entrance.EntranceDirection = entranceDirection;

            Assert.Equal(entranceDirection, entrance.EntranceDirection);
        }

        [Fact]
        public void TestEntranceDirection_SetNull()
        {
            BuildingEntrance entrance = new BuildingEntrance();

            Vector<double> entranceDirection =
                new DenseVector(new[] { 0.0, 1.0, 0.0 });
            entrance.EntranceDirection = entranceDirection;

            Assert.Throws<ArgumentNullException>(
                () => { entrance.EntranceDirection = null; }
            );

            Assert.Equal(entranceDirection, entrance.EntranceDirection);
        }
    }
}
