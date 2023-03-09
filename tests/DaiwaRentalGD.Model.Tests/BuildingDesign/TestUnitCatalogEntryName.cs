using System;
using DaiwaRentalGD.Model.BuildingDesign;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestUnitCatalogEntryName
    {
        [Fact]
        public void TestConstructor_Default()
        {
            var entryName = new UnitCatalogEntryName();

            Assert.Equal(0, entryName.MainType);
            Assert.Equal(0, entryName.SizeXInP);
            Assert.Equal(0, entryName.SizeYInP);
            Assert.Equal(0, entryName.VariantType);
            Assert.Equal(0, entryName.Index);
            Assert.Equal("0-0000-0", entryName.FullName);
        }

        [Fact]
        public void TestConstructor_Copy()
        {
            UnitCatalogEntryName entryName0 = new UnitCatalogEntryName
            {
                MainType = 1,
                SizeXInP = 6,
                SizeYInP = 8,
                VariantType = 3
            };

            UnitCatalogEntryName entryName1 =
                new UnitCatalogEntryName(entryName0);

            Assert.Equal(entryName0, entryName1);
        }

        [Theory]
        [InlineData("1-0608-1", 1, 6, 8, 1)]
        [InlineData("3-0711-2", 3, 7, 11, 2)]
        [InlineData("15-1009-12", 15, 10, 9, 12)]
        public void TestParse(
            string fullName,
            int expectedMainType,
            int expectedSizeXInP,
            int expectedSizeYInP,
            int expectedVariantType
        )
        {
            var entryName = UnitCatalogEntryName.Parse(fullName);

            Assert.Equal(expectedMainType, entryName.MainType);
            Assert.Equal(expectedSizeXInP, entryName.SizeXInP);
            Assert.Equal(expectedSizeYInP, entryName.SizeYInP);
            Assert.Equal(expectedVariantType, entryName.VariantType);
            Assert.Equal(0, entryName.Index);
        }

        [Theory]
        [InlineData("15-1009--12")]
        [InlineData("15-100912")]
        [InlineData("15-10095-12")]
        [InlineData("15-1009-")]
        [InlineData("-1009-12")]
        [InlineData("15-1009-xx")]
        [InlineData("")]
        public void TestParse_InvalidFullNameFormat(string fullName)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => { UnitCatalogEntryName.Parse(fullName); }
            );
        }

        [Theory]
        [InlineData(1, 6, 8, 1, "1-0608-1")]
        [InlineData(3, 7, 11, 2, "3-0711-2")]
        [InlineData(15, 10, 9, 12, "15-1009-12")]
        public void TestFullName(
            int mainType, int sizeXInP, int sizeYInP, int variantType,
            string expectedFullName
        )
        {
            var entryName = new UnitCatalogEntryName
            {
                MainType = mainType,
                SizeXInP = sizeXInP,
                SizeYInP = sizeYInP,
                VariantType = variantType
            };

            Assert.Equal(expectedFullName, entryName.FullName);
        }

        [Fact]
        public void TestMainType()
        {
            var entryName = new UnitCatalogEntryName();

            entryName.MainType = 1;
            Assert.Equal(1, entryName.MainType);

            entryName.MainType = 2;
            Assert.Equal(2, entryName.MainType);

            entryName.MainType = 23;
            Assert.Equal(23, entryName.MainType);

            entryName.MainType = 0;
            Assert.Equal(0, entryName.MainType);
        }

        [Fact]
        public void TestMainType_InvalidValue()
        {
            var entryName = new UnitCatalogEntryName
            {
                MainType = 2
            };

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entryName.MainType = -1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entryName.MainType = -2; }
            );

            Assert.Equal(2, entryName.MainType);
        }

        [Fact]
        public void TestSizeXInPAndSizeYInP()
        {
            var entryName = new UnitCatalogEntryName();

            entryName.SizeXInP = 9;
            Assert.Equal(9, entryName.SizeXInP);

            entryName.SizeXInP = 12;
            Assert.Equal(12, entryName.SizeXInP);

            entryName.SizeXInP = 0;
            Assert.Equal(0, entryName.SizeXInP);

            entryName.SizeXInP = 99;
            Assert.Equal(99, entryName.SizeXInP);

            entryName.SizeYInP = 7;
            Assert.Equal(7, entryName.SizeYInP);

            entryName.SizeYInP = 11;
            Assert.Equal(11, entryName.SizeYInP);

            entryName.SizeYInP = 0;
            Assert.Equal(0, entryName.SizeYInP);

            entryName.SizeYInP = 99;
            Assert.Equal(99, entryName.SizeYInP);
        }

        [Fact]
        public void TestSizeXInPAndSizeYInP_InvalidSize()
        {
            var entryName = new UnitCatalogEntryName
            {
                SizeXInP = 7,
                SizeYInP = 10
            };

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entryName.SizeXInP = -1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entryName.SizeXInP = 100; }
            );
            Assert.Equal(7, entryName.SizeXInP);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entryName.SizeYInP = -1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entryName.SizeYInP = 100; }
            );
            Assert.Equal(10, entryName.SizeYInP);
        }

        [Fact]
        public void TestVariantType()
        {
            var entryName = new UnitCatalogEntryName();

            entryName.VariantType = 1;
            Assert.Equal(1, entryName.VariantType);

            entryName.VariantType = 2;
            Assert.Equal(2, entryName.VariantType);

            entryName.VariantType = 12;
            Assert.Equal(12, entryName.VariantType);

            entryName.VariantType = 0;
            Assert.Equal(0, entryName.VariantType);
        }

        [Fact]
        public void TestVariantType_InvalidValue()
        {
            var entryName = new UnitCatalogEntryName
            {
                VariantType = 3
            };

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entryName.VariantType = -1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entryName.VariantType = -2; }
            );

            Assert.Equal(3, entryName.VariantType);
        }

        [Fact]
        public void TestIndex()
        {
            var entryName = new UnitCatalogEntryName();

            entryName.Index = 2;
            Assert.Equal(2, entryName.Index);

            entryName.Index = 1;
            Assert.Equal(1, entryName.Index);

            entryName.Index = 0;
            Assert.Equal(0, entryName.Index);
        }

        [Fact]
        public void TestIndex_InvalidValue()
        {
            var entryName = new UnitCatalogEntryName();

            entryName.Index = 2;
            Assert.Equal(2, entryName.Index);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { entryName.Index = -1; }
            );

            Assert.Equal(2, entryName.Index);
        }

        [Fact]
        public void TestGetHashCodeAndEqualsAndOps()
        {
            var entryName0 = UnitCatalogEntryName.Parse("1-0608-1");
            var entryName1 = UnitCatalogEntryName.Parse("1-0608-1");
            var entryName2 = UnitCatalogEntryName.Parse("3-0711-1");

            Assert.True(entryName0.Equals(entryName0));
            Assert.True(entryName0 == entryName0);
            Assert.False(entryName0 != entryName0);
            Assert.Equal(
                entryName0.GetHashCode(), entryName0.GetHashCode()
            );

            Assert.True(entryName0.Equals(entryName1));
            Assert.True(entryName1.Equals(entryName0));
            Assert.True(entryName0 == entryName1);
            Assert.True(entryName1 == entryName0);
            Assert.False(entryName0 != entryName1);
            Assert.False(entryName1 != entryName0);
            Assert.Equal(
                entryName0.GetHashCode(), entryName1.GetHashCode()
            );

            entryName1.Index = 1;

            Assert.False(entryName0.Equals(entryName1));
            Assert.False(entryName1.Equals(entryName0));
            Assert.False(entryName0 == entryName1);
            Assert.False(entryName1 == entryName0);
            Assert.True(entryName0 != entryName1);
            Assert.True(entryName1 != entryName0);
            Assert.NotEqual(
                entryName0.GetHashCode(), entryName1.GetHashCode()
            );


            Assert.False(entryName0.Equals(entryName2));
            Assert.False(entryName2.Equals(entryName0));
            Assert.False(entryName0 == entryName2);
            Assert.False(entryName2 == entryName0);
            Assert.True(entryName0 != entryName2);
            Assert.True(entryName2 != entryName0);
            Assert.NotEqual(
                entryName0.GetHashCode(), entryName2.GetHashCode()
            );

            Assert.False(entryName0.Equals(null));
            Assert.False(entryName0 == null);
            Assert.False(null == entryName0);
            Assert.True(entryName0 != null);
            Assert.True(null != entryName0);
        }
    }
}
