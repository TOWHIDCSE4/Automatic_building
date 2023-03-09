//using DaiwaRentalGD.Geometries;
//using DaiwaRentalGD.Model.BuildingDesign;
//using DaiwaRentalGD.Model.Finance;
//using DaiwaRentalGD.Scene;
//using Xunit;

//namespace DaiwaRentalGD.Model.Tests.Finance
//{
//    public class TestSummaryFinanceComponent
//    {
//        private Unit CreateUnit()
//        {
//            Unit unit = new Unit();

//            UnitComponent uc = unit.UnitComponent;

//            uc.AddRoomPlan(new Polygon(
//                new[]
//                {
//                    new Point(0.0, 0.0, 0.0),
//                    new Point(4.0, 0.0, 0.0),
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
//                    new Point(4.0, 0.0, 0.0),
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
//                    new Point(4.0, 0.0, 0.0),
//                    new Point(4.0, 10.0, 0.0),
//                    new Point(0.0, 10.0, 0.0)
//                }
//            ));

//            return unit;
//        }

//        private UnitFinanceComponent GetUnitFinanceComponent()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            ufc.AddCostEntry(
//                new UnitCostEntry
//                {
//                    NumOfBedrooms = 0,
//                    CostYen = 10000000.0
//                }
//            );
//            ufc.AddCostEntry(
//                new UnitCostEntry
//                {
//                    NumOfBedrooms = 1,
//                    CostYen = 11000000.0
//                }
//            );
//            ufc.AddCostEntry(
//                new UnitCostEntry
//                {
//                    NumOfBedrooms = 2,
//                    CostYen = 12000000.0
//                }
//            );

//            ufc.AddRevenueEntry(
//                new UnitRevenueEntry
//                {
//                    NumOfBedrooms = 1,
//                    MinArea = 40.0,
//                    RevenueYenPerSqmPerMonth = 1900.0
//                }
//            );
//            ufc.AddRevenueEntry(
//                new UnitRevenueEntry
//                {
//                    NumOfBedrooms = 1,
//                    MinArea = 30.0,
//                    RevenueYenPerSqmPerMonth = 2000.0
//                }
//            );
//            ufc.AddRevenueEntry(
//                new UnitRevenueEntry
//                {
//                    NumOfBedrooms = 2,
//                    MinArea = 40.0,
//                    RevenueYenPerSqmPerMonth = 2000.0
//                }
//            );

//            return ufc;
//        }

//        private UnitFinanceComponent GetUnitFinanceComponentZeroCost()
//        {
//            UnitFinanceComponent ufc = new UnitFinanceComponent();

//            ufc.AddCostEntry(
//                new UnitCostEntry
//                {
//                    NumOfBedrooms = 0,
//                    CostYen = 0.0
//                }
//            );
//            ufc.AddCostEntry(
//                new UnitCostEntry
//                {
//                    NumOfBedrooms = 1,
//                    CostYen = 0.0
//                }
//            );
//            ufc.AddCostEntry(
//                new UnitCostEntry
//                {
//                    NumOfBedrooms = 2,
//                    CostYen = 0.0
//                }
//            );

//            ufc.AddRevenueEntry(
//                new UnitRevenueEntry
//                {
//                    NumOfBedrooms = 1,
//                    MinArea = 40.0,
//                    RevenueYenPerSqmPerMonth = 1900.0
//                }
//            );
//            ufc.AddRevenueEntry(
//                new UnitRevenueEntry
//                {
//                    NumOfBedrooms = 1,
//                    MinArea = 30.0,
//                    RevenueYenPerSqmPerMonth = 2000.0
//                }
//            );
//            ufc.AddRevenueEntry(
//                new UnitRevenueEntry
//                {
//                    NumOfBedrooms = 2,
//                    MinArea = 40.0,
//                    RevenueYenPerSqmPerMonth = 2000.0
//                }
//            );

//            return ufc;
//        }

//        [Fact]
//        public void TestConstructor()
//        {
//            SummaryFinanceComponent sfc = new SummaryFinanceComponent();

//            Assert.Null(sfc.UnitFinanceComponent);
//            Assert.Null(sfc.Building);
//        }

//        [Fact]
//        public void TestGetBuildingCostYen()
//        {
//            // Unit finance component

//            UnitFinanceComponent ufc = GetUnitFinanceComponent();

//            // Building

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

//            // Summary finance component

//            SummaryFinanceComponent sfc = new SummaryFinanceComponent
//            {
//                Building = building
//            };

//            SceneObject so = new SceneObject();
//            so.AddComponent(ufc);
//            so.AddComponent(sfc);

//            Assert.Equal(
//                23000000.0,
//                sfc.GetBuildingCostYen()
//            );
//        }

//        [Fact]
//        public void TestTotalCostYen()
//        {
//            // Unit finance component

//            UnitFinanceComponent ufc = GetUnitFinanceComponent();

//            // Building

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

//            // Summary finance component

//            SummaryFinanceComponent sfc = new SummaryFinanceComponent
//            {
//                Building = building
//            };

//            SceneObject so = new SceneObject();
//            so.AddComponent(ufc);
//            so.AddComponent(sfc);

//            Assert.Equal(
//                23000000.0,
//                sfc.GetTotalCostYen()
//            );
//        }

//        [Fact]
//        public void TestGetBuildingRevenueYenPerMonth()
//        {
//            // Unit finance component

//            UnitFinanceComponent ufc = GetUnitFinanceComponent();

//            // Building

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

//            // Summary finance component

//            SummaryFinanceComponent sfc = new SummaryFinanceComponent
//            {
//                Building = building
//            };

//            SceneObject so = new SceneObject();
//            so.AddComponent(ufc);
//            so.AddComponent(sfc);

//            Assert.Equal(
//                156000.0,
//                sfc.GetBuildingRevenueYenPerMonth()
//            );
//        }

//        [Fact]
//        public void TestGetTotalRevenueYenPerMonthAndPerYear()
//        {
//            // Unit finance component

//            UnitFinanceComponent ufc = GetUnitFinanceComponent();

//            // Building

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

//            // Summary finance component

//            SummaryFinanceComponent sfc = new SummaryFinanceComponent
//            {
//                Building = building
//            };

//            SceneObject so = new SceneObject();
//            so.AddComponent(ufc);
//            so.AddComponent(sfc);

//            Assert.Equal(
//                156000.0,
//                sfc.GetTotalRevenueYenPerMonth()
//            );
//            Assert.Equal(
//                1872000.0,
//                sfc.GetTotalRevenueYenPerYear()
//            );
//        }

//        [Fact]
//        public void TestGetGrossRorPerMonthAndYear()
//        {
//            // Unit finance component

//            UnitFinanceComponent ufc = GetUnitFinanceComponent();

//            // Building

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

//            // Summary finance component

//            SummaryFinanceComponent sfc = new SummaryFinanceComponent
//            {
//                Building = building
//            };

//            SceneObject so = new SceneObject();
//            so.AddComponent(ufc);
//            so.AddComponent(sfc);

//            Assert.Equal(
//                23000000.0,
//                sfc.GetTotalCostYen()
//            );
//            Assert.Equal(
//                156000.0,
//                sfc.GetTotalRevenueYenPerMonth()
//            );
//            Assert.Equal(
//                156000.0 / 23000000.0,
//                sfc.GetGrossRorPerMonth()
//            );
//            Assert.Equal(
//                156000.0 / 23000000.0 * 12,
//                sfc.GetGrossRorPerYear()
//            );
//        }

//        [Fact]
//        public void TestGetGrossRorPerMonthAndYear_ZeroCostNonzeroRevenue()
//        {
//            // Unit finance component

//            UnitFinanceComponent ufc = GetUnitFinanceComponentZeroCost();

//            // Building

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

//            // Summary finance component

//            SummaryFinanceComponent sfc = new SummaryFinanceComponent
//            {
//                Building = building
//            };

//            SceneObject so = new SceneObject();
//            so.AddComponent(ufc);
//            so.AddComponent(sfc);

//            Assert.Equal(
//                0.0,
//                sfc.GetTotalCostYen()
//            );
//            Assert.Equal(
//                156000.0,
//                sfc.GetTotalRevenueYenPerMonth()
//            );
//            Assert.Equal(
//                double.PositiveInfinity,
//                sfc.GetGrossRorPerMonth()
//            );
//            Assert.Equal(
//                double.PositiveInfinity,
//                sfc.GetGrossRorPerYear()
//            );
//        }

//        [Fact]
//        public void TestGetGrossRorPerMonthAndYear_ZeroCostZeroRevenue()
//        {
//            SummaryFinanceComponent sfc = new SummaryFinanceComponent();

//            Assert.Equal(0.0, sfc.GetTotalCostYen());
//            Assert.Equal(0.0, sfc.GetTotalRevenueYenPerMonth());
//            Assert.Equal(0.0, sfc.GetTotalRevenueYenPerYear());

//            Assert.True(double.IsNaN(sfc.GetGrossRorPerMonth()));
//            Assert.True(double.IsNaN(sfc.GetGrossRorPerYear()));
//        }

//        [Fact]
//        public void TestUnitFinanceComponent()
//        {
//            SummaryFinanceComponent sfc = new SummaryFinanceComponent();

//            Assert.Null(sfc.UnitFinanceComponent);

//            SceneObject so = new SceneObject();
//            so.AddComponent(sfc);

//            Assert.Null(sfc.UnitFinanceComponent);

//            UnitFinanceComponent ufc = new UnitFinanceComponent();
//            so.AddComponent(ufc);

//            Assert.Equal(ufc, sfc.UnitFinanceComponent);
//        }
//    }
//}
