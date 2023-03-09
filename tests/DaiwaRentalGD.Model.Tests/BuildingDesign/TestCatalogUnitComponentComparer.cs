using DaiwaRentalGD.Model.BuildingDesign;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestCatalogUnitComponentComparer
    {
        [Fact]
        public void TestCompare_Null()
        {
            CatalogUnitComponent uc = new CatalogUnitComponent
            {
                NumOfBedrooms = 0,
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 1,
                    SizeXInP = 6,
                    SizeYInP = 8,
                    VariantType = 2
                }
            };

            var comparer = new CatalogUnitComponentComparer();

            Assert.Equal(0, comparer.Compare(null, null));
            Assert.Equal(1, comparer.Compare(uc, null));
            Assert.Equal(-1, comparer.Compare(null, uc));
        }

        [Fact]
        public void TestCompare_NonNull()
        {
            CatalogUnitComponent uc0 = new CatalogUnitComponent
            {
                NumOfBedrooms = 1,
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 1,
                    SizeXInP = 6,
                    SizeYInP = 8,
                    VariantType = 2
                }
            };
            CatalogUnitComponent uc1 = new CatalogUnitComponent
            {
                NumOfBedrooms = 1,
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 2,
                    SizeXInP = 6,
                    SizeYInP = 8,
                    VariantType = 2
                }
            };
            CatalogUnitComponent uc2 = new CatalogUnitComponent
            {
                NumOfBedrooms = 2,
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 1,
                    SizeXInP = 6,
                    SizeYInP = 8,
                    VariantType = 2
                }
            };
            CatalogUnitComponent uc3 = new CatalogUnitComponent
            {
                NumOfBedrooms = 1,
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 1,
                    SizeXInP = 5,
                    SizeYInP = 8,
                    VariantType = 2
                }
            };
            CatalogUnitComponent uc4 = new CatalogUnitComponent
            {
                NumOfBedrooms = 1,
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 1,
                    SizeXInP = 6,
                    SizeYInP = 7,
                    VariantType = 2
                }
            };
            CatalogUnitComponent uc5 = new CatalogUnitComponent
            {
                NumOfBedrooms = 1,
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 1,
                    SizeXInP = 6,
                    SizeYInP = 8,
                    VariantType = 3
                }
            };
            CatalogUnitComponent uc6 = new CatalogUnitComponent
            {
                NumOfBedrooms = 1,
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 1,
                    SizeXInP = 6,
                    SizeYInP = 8,
                    VariantType = 2,
                    Index = 1
                }
            };

            var comparer = new CatalogUnitComponentComparer();

            Assert.Equal(0, comparer.Compare(uc0, uc0));

            Assert.Equal(-1, comparer.Compare(uc0, uc1));
            Assert.Equal(1, comparer.Compare(uc1, uc0));

            Assert.Equal(-1, comparer.Compare(uc0, uc2));
            Assert.Equal(1, comparer.Compare(uc2, uc0));

            Assert.Equal(1, comparer.Compare(uc0, uc3));
            Assert.Equal(-1, comparer.Compare(uc3, uc0));

            Assert.Equal(1, comparer.Compare(uc0, uc4));
            Assert.Equal(-1, comparer.Compare(uc4, uc0));

            Assert.Equal(-1, comparer.Compare(uc0, uc5));
            Assert.Equal(1, comparer.Compare(uc5, uc0));

            Assert.Equal(-1, comparer.Compare(uc0, uc6));
            Assert.Equal(1, comparer.Compare(uc6, uc0));
        }
    }
}
