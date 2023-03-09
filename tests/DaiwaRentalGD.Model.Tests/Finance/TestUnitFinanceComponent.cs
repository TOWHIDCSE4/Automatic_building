//using System;
//using DaiwaRentalGD.Geometries;
//using DaiwaRentalGD.Model.BuildingDesign;
//using DaiwaRentalGD.Model.Finance;
//using Xunit;

//namespace DaiwaRentalGD.Model.Tests.Finance
//{
//    public class TestUnitFinanceComponent
//    {
//        private Unit CreateUnit()
//        {
//            Unit unit = new Unit();

//            UnitComponent uc = unit.UnitComponent;

//            uc.AddRoomPlan(new Polygon(
//                new[]
//                {
//                    new Point(0.0, 0.0, 0.0),
//                    new Point(5.0, 0.0, 0.0),
//                    new Point(5.0, 3.0, 0.0),
//                    new Point(4.0, 3.0, 0.0),
//                    new Point(4.0, 10.0, 0.0),
//                    new Point(0.0, 10.0, 0.0)
//                }
//            ));

//            return unit;
//        }

//        private Unit CreateUnitWithWalkway()
//        {
//            Unit unit = new Unit();

//            UnitComponent uc = unit.UnitComponent;

//            uc.AddRoomPlan(new Polygon(
//                new[]
//                {
//                    new Point(0.0, 0.0, 0.0),
//                    new Point(5.0, 0.0, 0.0),
//                    new Point(5.0, 3.0, 0.0),
//                    new Point(4.0, 3.0, 0.0),
//                    new Point(4.0, 10.0, 0.0),
//                    new Point(0.0, 10.0, 0.0)
//                }
//            ));

//            return unit;
//        }

//        private Unit CreateUnitWithStair()
//        {
//            Unit unit = new Unit();

//            UnitComponent uc = unit.UnitComponent;

//            uc.AddRoomPlan(new Polygon(
//                new[]
//                {
//                    new Point(0.0, 0.0, 0.0),
//                    new Point(5.0, 0.0, 0.0),
//                    new Point(5.0, 3.0, 0.0),
//                    new Point(4.0, 3.0, 0.0),
//                    new Point(4.0, 10.0, 0.0),
//                    new Point(0.0, 10.0, 0.0)
//                }
//            ));

//            return unit;
//        }

//        [Fact]
//        public void TestConstructor()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            // Assert.Empty(ufc.GetCostEntries());
//            // Assert.Empty(ufc.GetRevenueEntries());
//        }

//        [Fact]
//        public void TestGetUnitArea()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            Unit unit0 = CreateUnit();

//            // Assert.Equal(43.0, ufc.GetUnitArea(unit0));

//            Unit unit1 = CreateUnitWithWalkway();

//            // Assert.Equal(43.0, ufc.GetUnitArea(unit1));

//            Unit unit2 = CreateUnitWithStair();

//            // Assert.Equal(43.0, ufc.GetUnitArea(unit2));
//        }

//        [Fact]
//        public void TestAddCostEntry()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitCostEntry entry0 = new UnitCostEntry
//            {
//                NumOfBedrooms = 0,
//                CostYen = 10000000.0
//            };
//            UnitCostEntry entry1 = new UnitCostEntry
//            {
//                NumOfBedrooms = 1,
//                CostYen = 11000000.0
//            };
//            UnitCostEntry entry2 = new UnitCostEntry
//            {
//                NumOfBedrooms = 2,
//                CostYen = 12000000.0
//            };

//            ufc.AddCostEntry(entry0);
//            ufc.AddCostEntry(entry2);
//            ufc.AddCostEntry(entry1);

//            // Assert.Equal(
//            //     new[] { entry0, entry1, entry2 },
//            //     ufc.GetCostEntries()
//            // );
//        }

//        [Fact]
//        public void TestAddCostEntry_EntryExists()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitCostEntry entry0 = new UnitCostEntry
//            {
//                NumOfBedrooms = 0,
//                CostYen = 10000000.0
//            };
//            UnitCostEntry entry1 = new UnitCostEntry
//            {
//                NumOfBedrooms = 0,
//                CostYen = 11000000.0
//            };

//            ufc.AddCostEntry(entry0);

//            Assert.ThrowsAny<ArgumentException>(
//                () => { ufc.AddCostEntry(entry1); }
//            );

//            // Assert.Equal(
//            //     new[] { entry0 },
//            //     ufc.GetCostEntries()
//            // );
//        }

//        [Fact]
//        public void TestRemoveCostEntry()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitCostEntry entry0 = new UnitCostEntry
//            {
//                NumOfBedrooms = 0,
//                CostYen = 10000000.0
//            };
//            UnitCostEntry entry1 = new UnitCostEntry
//            {
//                NumOfBedrooms = 1,
//                CostYen = 11000000.0
//            };
//            UnitCostEntry entry2 = new UnitCostEntry
//            {
//                NumOfBedrooms = 2,
//                CostYen = 12000000.0
//            };

//            ufc.AddCostEntry(entry0);
//            ufc.AddCostEntry(entry2);
//            ufc.AddCostEntry(entry1);

//            // ufc.RemoveCostEntry(1);

//            // Assert.Equal(
//            //     new[] { entry0, entry2 },
//            //     ufc.GetCostEntries()
//            // );
//        }

//        [Fact]
//        public void TestRemoveCostEntry_EntryDoesNotExist()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitCostEntry entry0 = new UnitCostEntry
//            {
//                NumOfBedrooms = 0,
//                CostYen = 10000000.0
//            };
//            UnitCostEntry entry1 = new UnitCostEntry
//            {
//                NumOfBedrooms = 1,
//                CostYen = 11000000.0
//            };
//            UnitCostEntry entry2 = new UnitCostEntry
//            {
//                NumOfBedrooms = 2,
//                CostYen = 12000000.0
//            };

//            ufc.AddCostEntry(entry0);
//            ufc.AddCostEntry(entry2);
//            ufc.AddCostEntry(entry1);

//            // Assert.ThrowsAny<ArgumentException>(
//            //     () => { ufc.RemoveCostEntry(3); }
//            // );

//            // Assert.Equal(
//            //     new[] { entry0, entry1, entry2 },
//            //     ufc.GetCostEntries()
//            // );
//        }

//        [Fact]
//        public void TestGetCostEntry()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitCostEntry entry0 = new UnitCostEntry
//            {
//                NumOfBedrooms = 0,
//                CostYen = 10000000.0
//            };
//            UnitCostEntry entry1 = new UnitCostEntry
//            {
//                NumOfBedrooms = 1,
//                CostYen = 11000000.0
//            };
//            UnitCostEntry entry2 = new UnitCostEntry
//            {
//                NumOfBedrooms = 2,
//                CostYen = 12000000.0
//            };

//            ufc.AddCostEntry(entry0);
//            ufc.AddCostEntry(entry2);
//            ufc.AddCostEntry(entry1);

//            Assert.Equal(entry0, ufc.GetCostEntry(0));
//            Assert.Equal(entry1, ufc.GetCostEntry(1));
//            Assert.Equal(entry2, ufc.GetCostEntry(2));
//        }

//        [Fact]
//        public void TestGetCostEntry_EntryNotFound()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitCostEntry entry0 = new UnitCostEntry
//            {
//                NumOfBedrooms = 0,
//                CostYen = 10000000.0
//            };
//            UnitCostEntry entry1 = new UnitCostEntry
//            {
//                NumOfBedrooms = 1,
//                CostYen = 11000000.0
//            };
//            UnitCostEntry entry2 = new UnitCostEntry
//            {
//                NumOfBedrooms = 2,
//                CostYen = 12000000.0
//            };

//            ufc.AddCostEntry(entry0);
//            ufc.AddCostEntry(entry2);
//            ufc.AddCostEntry(entry1);

//            Assert.ThrowsAny<ArgumentException>(
//                () => { ufc.GetCostEntry(3); }
//            );
//        }

//        [Fact]
//        public void TestGetApplicableCostEntry()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitCostEntry entry0 = new UnitCostEntry
//            {
//                NumOfBedrooms = 0,
//                CostYen = 10000000.0
//            };
//            UnitCostEntry entry1 = new UnitCostEntry
//            {
//                NumOfBedrooms = 1,
//                CostYen = 11000000.0
//            };
//            UnitCostEntry entry2 = new UnitCostEntry
//            {
//                NumOfBedrooms = 2,
//                CostYen = 12000000.0
//            };

//            ufc.AddCostEntry(entry0);
//            ufc.AddCostEntry(entry2);
//            ufc.AddCostEntry(entry1);

//            Unit unit0 = CreateUnit();
//            unit0.UnitComponent.NumOfBedrooms = 0;

//            Unit unit1 = CreateUnit();
//            unit1.UnitComponent.NumOfBedrooms = 1;

//            Assert.Equal(entry0, ufc.GetCostEntry(unit0));
//            Assert.Equal(entry1, ufc.GetCostEntry(unit1));
//        }

//        [Fact]
//        public void TestGetUnitCostYen()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitCostEntry entry0 = new UnitCostEntry
//            {
//                NumOfBedrooms = 0,
//                CostYen = 10000000.0
//            };
//            UnitCostEntry entry1 = new UnitCostEntry
//            {
//                NumOfBedrooms = 1,
//                CostYen = 11000000.0
//            };
//            UnitCostEntry entry2 = new UnitCostEntry
//            {
//                NumOfBedrooms = 2,
//                CostYen = 12000000.0
//            };

//            ufc.AddCostEntry(entry0);
//            ufc.AddCostEntry(entry2);
//            ufc.AddCostEntry(entry1);

//            Unit unit0 = CreateUnit();
//            unit0.UnitComponent.NumOfBedrooms = 0;

//            Unit unit1 = CreateUnit();
//            unit1.UnitComponent.NumOfBedrooms = 1;

//            // Assert.Equal(10000000.0, ufc.GetUnitCostYen(unit0));
//            // Assert.Equal(11000000.0, ufc.GetUnitCostYen(unit1));
//        }

//        [Fact]
//        public void TestGetBulidingCostYen()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitCostEntry entry0 = new UnitCostEntry
//            {
//                NumOfBedrooms = 0,
//                CostYen = 10000000.0
//            };
//            UnitCostEntry entry1 = new UnitCostEntry
//            {
//                NumOfBedrooms = 1,
//                CostYen = 11000000.0
//            };
//            UnitCostEntry entry2 = new UnitCostEntry
//            {
//                NumOfBedrooms = 2,
//                CostYen = 12000000.0
//            };

//            ufc.AddCostEntry(entry0);
//            ufc.AddCostEntry(entry2);
//            ufc.AddCostEntry(entry1);

//            Building building = new Building();

//            BuildingComponent bc = building.BuildingComponent;

//            bc.AddFloor(3.0);
//            bc.AddFloor(3.0);
//            bc.AddStack();
//            bc.AddStack();

//            Unit unit0 = CreateUnitWithWalkway();
//            unit0.UnitComponent.NumOfBedrooms = 1;

//            Unit unit1 = CreateUnitWithStair();
//            unit1.UnitComponent.NumOfBedrooms = 2;

//            bc.SetUnit(1, 0, unit0);
//            bc.SetUnit(0, 1, unit1);

//            Assert.Equal(
//                23000000,
//                ufc.GetBuildingCostYen(building)
//            );
//        }

//        [Fact]
//        public void TestAddRevenueEntry()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 1900.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry2 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 2,
//                MinArea = 50.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };

//            ufc.AddRevenueEntry(entry0);
//            ufc.AddRevenueEntry(entry1);
//            ufc.AddRevenueEntry(entry2);

//            // Assert.Equal(
//            //     new[] { entry1, entry0, entry2 },
//            //     ufc.GetRevenueEntries()
//            // );
//        }

//        [Fact]
//        public void TestAddRevenueEntry_EntryExists()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2100.0
//            };

//            ufc.AddRevenueEntry(entry0);

//            Assert.ThrowsAny<ArgumentException>(
//                () => { ufc.AddRevenueEntry(entry1); }
//            );

//            // Assert.Equal(
//            //     new[] { entry0 },
//            //     ufc.GetRevenueEntries()
//            // );
//        }

//        [Fact]
//        public void TestRemoveRevenueEntry()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 1900.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry2 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 2,
//                MinArea = 50.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };

//            ufc.AddRevenueEntry(entry0);
//            ufc.AddRevenueEntry(entry1);
//            ufc.AddRevenueEntry(entry2);

//            // ufc.RemoveRevenueEntry(1, 30.0);
//            // ufc.RemoveRevenueEntry(2, 50.0);

//            // Assert.Equal(
//            //     new[] { entry0 },
//            //     ufc.GetRevenueEntries()
//            // );
//        }

//        [Fact]
//        public void TestRemoveRevenueEntry_EntryDoesNotExist()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 1900.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry2 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 2,
//                MinArea = 50.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };

//            ufc.AddRevenueEntry(entry0);
//            ufc.AddRevenueEntry(entry1);
//            ufc.AddRevenueEntry(entry2);

//            // Assert.ThrowsAny<ArgumentException>(
//            //     () => { ufc.RemoveRevenueEntry(3, 50.0); }
//            // );

//            // Assert.ThrowsAny<ArgumentException>(
//            //     () => { ufc.RemoveRevenueEntry(1, 60.0); }
//            // );

//            // Assert.Equal(
//            //     new[] { entry1, entry0, entry2 },
//            //     ufc.GetRevenueEntries()
//            // );
//        }

//        [Fact]
//        public void TestGetRevenueEntry()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 1900.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry2 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 2,
//                MinArea = 50.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };

//            ufc.AddRevenueEntry(entry0);
//            ufc.AddRevenueEntry(entry1);
//            ufc.AddRevenueEntry(entry2);

//            // Assert.Equal(entry0, ufc.GetRevenueEntry(1, 40.0));
//            // Assert.Equal(entry1, ufc.GetRevenueEntry(1, 30.0));
//            // Assert.Equal(entry2, ufc.GetRevenueEntry(2, 50.0));
//        }

//        [Fact]
//        public void TestGetRevenueEntry_EntryNotFound()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 1900.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry2 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 2,
//                MinArea = 50.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };

//            ufc.AddRevenueEntry(entry0);
//            ufc.AddRevenueEntry(entry1);
//            ufc.AddRevenueEntry(entry2);

//            // Assert.ThrowsAny<ArgumentException>(
//            //     () => { ufc.GetRevenueEntry(3, 30.0); }
//            // );
//            // Assert.ThrowsAny<ArgumentException>(
//            //     () => { ufc.GetRevenueEntry(1, 25.0); }
//            // );
//        }

//        [Fact]
//        public void TestGetApplicableRevenueEntry()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 1900.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry2 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 2,
//                MinArea = 50.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };

//            ufc.AddRevenueEntry(entry0);
//            ufc.AddRevenueEntry(entry1);
//            ufc.AddRevenueEntry(entry2);

//            Assert.Equal(entry0, ufc.GetRevenueEntry(1, 40.0));
//            Assert.Equal(entry0, ufc.GetRevenueEntry(1, 45.0));
//            Assert.Equal(entry1, ufc.GetRevenueEntry(1, 30.0));
//            Assert.Equal(entry1, ufc.GetRevenueEntry(1, 39.0));
//            Assert.Equal(entry2, ufc.GetRevenueEntry(2, 50.0));
//            Assert.Equal(entry2, ufc.GetRevenueEntry(2, 55.0));
//        }

//        [Fact]
//        public void TestGetApplicableRevenueEntry_EntryNotFound()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 1900.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry2 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 2,
//                MinArea = 50.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };

//            ufc.AddRevenueEntry(entry0);
//            ufc.AddRevenueEntry(entry1);
//            ufc.AddRevenueEntry(entry2);

//            Assert.ThrowsAny<ArgumentException>(
//                () => { ufc.GetRevenueEntry(1, 25.0); }
//            );
//            Assert.ThrowsAny<ArgumentException>(
//                () => { ufc.GetRevenueEntry(2, 30.0); }
//            );
//            Assert.ThrowsAny<ArgumentException>(
//                () => { ufc.GetRevenueEntry(3, 30.0); }
//            );
//        }

//        [Fact]
//        public void TestGetApplicableRevenueEntry_WithUnit()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 1900.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry2 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 2,
//                MinArea = 50.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };

//            ufc.AddRevenueEntry(entry0);
//            ufc.AddRevenueEntry(entry1);
//            ufc.AddRevenueEntry(entry2);

//            Unit unit = CreateUnit();
//            unit.UnitComponent.NumOfBedrooms = 1;

//            // Assert.Equal(43.0, ufc.GetUnitArea(unit));

//            Assert.Equal(entry0, ufc.GetRevenueEntry(unit));
//        }

//        [Fact]
//        public void TestGetUnitRevenueYenPerMonthAndPerYear()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 1900.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry2 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 2,
//                MinArea = 50.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };

//            ufc.AddRevenueEntry(entry0);
//            ufc.AddRevenueEntry(entry1);
//            ufc.AddRevenueEntry(entry2);

//            Unit unit = CreateUnit();
//            unit.UnitComponent.NumOfBedrooms = 1;

//            // Assert.Equal(43.0, ufc.GetUnitArea(unit));
//            Assert.Equal(entry0, ufc.GetRevenueEntry(unit));
//            Assert.Equal(81700.0, ufc.GetUnitRevenueYenPerMonth(unit));
//            Assert.Equal(980400.0, ufc.GetUnitRevenueYenPerYear(unit));
//        }

//        [Fact]
//        public void TestGetBulidingRevenueYenPerMonthAndPerYear()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            UnitRevenueEntry entry0 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 1900.0
//            };
//            UnitRevenueEntry entry1 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 1,
//                MinArea = 30.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };
//            UnitRevenueEntry entry2 = new UnitRevenueEntry
//            {
//                NumOfBedrooms = 2,
//                MinArea = 40.0,
//                RevenueYenPerSqmPerMonth = 2000.0
//            };

//            ufc.AddRevenueEntry(entry0);
//            ufc.AddRevenueEntry(entry1);
//            ufc.AddRevenueEntry(entry2);

//            Building building = new Building();

//            BuildingComponent bc = building.BuildingComponent;

//            bc.AddFloor(3.0);
//            bc.AddFloor(3.0);
//            bc.AddStack();
//            bc.AddStack();

//            Unit unit0 = CreateUnitWithWalkway();
//            unit0.UnitComponent.NumOfBedrooms = 1;

//            Unit unit1 = CreateUnitWithStair();
//            unit1.UnitComponent.NumOfBedrooms = 2;

//            bc.SetUnit(1, 0, unit0);
//            bc.SetUnit(0, 1, unit1);

//            Assert.Equal(
//                167700.0,
//                ufc.GetBuildingRevenueYenPerMonth(building)
//            );
//            Assert.Equal(
//                2012400.0,
//                ufc.GetBuildingRevenueYenPerYear(building)
//            );
//        }
//    }
//}
