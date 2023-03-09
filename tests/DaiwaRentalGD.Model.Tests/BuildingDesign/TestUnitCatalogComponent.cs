using System;
using DaiwaRentalGD.Model.BuildingDesign;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestUnitCatalogComponent
    {
        [Fact]
        public void TestConstructor()
        {
            var ucc = new UnitCatalogComponent();

            Assert.Empty(ucc.Entries);
        }

        [Fact]
        public void AddEntry()
        {
            var ucc = new UnitCatalogComponent();

            var entry = new CatalogUnitComponent
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

            ucc.AddEntry(entry);

            Assert.Equal(new[] { entry }, ucc.Entries);
        }

        [Fact]
        public void AddEntry_NullEntry()
        {
            var ucc = new UnitCatalogComponent();

            var entry = new CatalogUnitComponent
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

            ucc.AddEntry(entry);

            Assert.Throws<ArgumentNullException>(
                () => { ucc.AddEntry(null); }
            );

            Assert.Equal(new[] { entry }, ucc.Entries);
        }

        [Fact]
        public void RemoveEntry()
        {
            var ucc = new UnitCatalogComponent();

            var entry0 = new CatalogUnitComponent
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
            var entry1 = new CatalogUnitComponent
            {
                NumOfBedrooms = 2,
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 2,
                    SizeXInP = 8,
                    SizeYInP = 10,
                    VariantType = 1
                }
            };

            ucc.AddEntry(entry0);

            Assert.False(ucc.RemoveEntry(entry1));
            Assert.Equal(new[] { entry0 }, ucc.Entries);

            Assert.True(ucc.RemoveEntry(entry0));
            Assert.Empty(ucc.Entries);

            Assert.False(ucc.RemoveEntry(null));
            Assert.Empty(ucc.Entries);
        }

        [Fact]
        public void Clear()
        {
            var ucc = new UnitCatalogComponent();

            var entry0 = new CatalogUnitComponent
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
            var entry1 = new CatalogUnitComponent
            {
                NumOfBedrooms = 2,
                EntryName = new UnitCatalogEntryName
                {
                    MainType = 2,
                    SizeXInP = 8,
                    SizeYInP = 10,
                    VariantType = 1
                }
            };

            ucc.AddEntry(entry0);
            ucc.AddEntry(entry1);

            ucc.Clear();

            Assert.Empty(ucc.Entries);
        }

        [Fact]
        public void TestEntries()
        {
            CatalogUnitComponent entry0 = new CatalogUnitComponent
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
            CatalogUnitComponent entry1 = new CatalogUnitComponent
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
            CatalogUnitComponent entry2 = new CatalogUnitComponent
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
            CatalogUnitComponent entry3 = new CatalogUnitComponent
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
            CatalogUnitComponent entry4 = new CatalogUnitComponent
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
            CatalogUnitComponent entry5 = new CatalogUnitComponent
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

            var ucc = new UnitCatalogComponent();

            ucc.AddEntry(entry0);
            ucc.AddEntry(entry1);
            ucc.AddEntry(entry2);
            ucc.AddEntry(entry3);
            ucc.AddEntry(entry4);
            ucc.AddEntry(entry5);

            Assert.Equal(
                new[] { entry3, entry4, entry0, entry5, entry2, entry1 },
                ucc.Entries
            );
        }
    }
}
