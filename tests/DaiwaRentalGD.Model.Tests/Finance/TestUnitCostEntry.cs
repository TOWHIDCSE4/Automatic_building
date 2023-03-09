using System;
using DaiwaRentalGD.Model.Finance;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.Finance
{
    public class TestUnitCostEntry
    {
        [Fact]
        public void TestConstructor()
        {
            UnitCostEntry entry = new UnitCostEntry();

            Assert.Equal(0, entry.NumOfBedrooms);
            Assert.Equal(0.0, entry.CostYen);
        }

        [Fact]
        public void TestNumOfBedrooms()
        {
            UnitCostEntry entry = new UnitCostEntry();

            entry.NumOfBedrooms = 1;

            Assert.Equal(1, entry.NumOfBedrooms);
        }

        [Fact]
        public void TestNumOfBedrooms_InvalidValue()
        {
            UnitCostEntry entry = new UnitCostEntry();

            entry.NumOfBedrooms = 1;

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entry.NumOfBedrooms = -2; }
            );

            Assert.Equal(1, entry.NumOfBedrooms);
        }

        [Fact]
        public void TestCostYen()
        {
            UnitCostEntry entry = new UnitCostEntry();

            entry.CostYen = 11000000.0;

            Assert.Equal(11000000.0, entry.CostYen);

            entry.CostYen = -123.0;

            Assert.Equal(-123.0, entry.CostYen);
        }
    }
}
