using System;
using DaiwaRentalGD.Model.Finance;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.Finance
{
    public class TestUnitRevenueEntry
    {
        [Fact]
        public void TestConstructor()
        {
            UnitRevenueEntry entry = new UnitRevenueEntry();

            Assert.Equal(0, entry.NumOfBedrooms);
            Assert.Equal(0.0, entry.MinArea);
            Assert.Equal(0.0, entry.RevenueYenPerSqmPerMonth);
        }

        [Fact]
        public void TestNumOfBedrooms()
        {
            UnitRevenueEntry entry = new UnitRevenueEntry();

            entry.NumOfBedrooms = 1;

            Assert.Equal(1, entry.NumOfBedrooms);
        }

        [Fact]
        public void TestNumOfBedrooms_InvalidValue()
        {
            UnitRevenueEntry entry = new UnitRevenueEntry();

            entry.NumOfBedrooms = 1;

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entry.NumOfBedrooms = -2; }
            );

            Assert.Equal(1, entry.NumOfBedrooms);
        }

        [Fact]
        public void TestMinArea()
        {
            UnitRevenueEntry entry = new UnitRevenueEntry();

            entry.MinArea = 30.0;

            Assert.Equal(30.0, entry.MinArea);

            entry.MinArea = 0.0;

            Assert.Equal(0.0, entry.MinArea);
        }

        [Fact]
        public void TestMinArea_InvalidValue()
        {
            UnitRevenueEntry entry = new UnitRevenueEntry();

            entry.MinArea = 30.0;

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entry.MinArea = -1.0; }
            );

            Assert.Equal(30.0, entry.MinArea);
        }

        [Fact]
        public void TestRevenueYenPerSqmPerMonth()
        {
            UnitRevenueEntry entry = new UnitRevenueEntry();

            entry.RevenueYenPerSqmPerMonth = 2520.0;

            Assert.Equal(2520.0, entry.RevenueYenPerSqmPerMonth);

            entry.RevenueYenPerSqmPerMonth = -123.0;

            Assert.Equal(-123.0, entry.RevenueYenPerSqmPerMonth);
        }
    }
}
