using System;
using DaiwaRentalGD.Model.BuildingDesign;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestCatalogUnitComponent
    {
        [Fact]
        public void TestConstructor_Default()
        {
            var uc = new CatalogUnitComponent();

            Assert.Equal(new UnitCatalogEntryName(), uc.EntryName);
        }

        [Fact]
        public void TestConstructor_Copy()
        {
            var uc0 = new CatalogUnitComponent
            {
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 1,
                    SizeXInP = 6,
                    SizeYInP = 8,
                    VariantType = 2
                }
            };

            var uc1 = new CatalogUnitComponent(uc0);

            Assert.Equal(uc0.EntryName, uc1.EntryName);
            Assert.NotSame(uc0.EntryName, uc1.EntryName);
        }

        [Fact]
        public void TestCopy()
        {
            var uc0 = new CatalogUnitComponent
            {
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 1,
                    SizeXInP = 6,
                    SizeYInP = 8,
                    VariantType = 2
                }
            };

            var uc1 = uc0.Copy();

            Assert.Equal(uc0.EntryName, uc1.EntryName);
            Assert.NotSame(uc0.EntryName, uc1.EntryName);
        }

        [Fact]
        public void TestEntryName()
        {
            var uc = new CatalogUnitComponent();

            var entryName = new UnitCatalogEntryName
            {
                MainType = 1,
                SizeXInP = 6,
                SizeYInP = 8,
                VariantType = 2
            };

            uc.EntryName = entryName;

            Assert.Equal(entryName, uc.EntryName);
        }

        [Fact]
        public void TestEntryName_NullValue()
        {
            var uc = new CatalogUnitComponent();

            var entryName = new UnitCatalogEntryName
            {
                MainType = 1,
                SizeXInP = 6,
                SizeYInP = 8,
                VariantType = 2
            };

            uc.EntryName = entryName;

            Assert.Throws<ArgumentNullException>(
                () => { uc.EntryName = null; }
            );

            Assert.Equal(entryName, uc.EntryName);
        }
    }
}
