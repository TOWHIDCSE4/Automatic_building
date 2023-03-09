using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Model.Zoning;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.Zoning
{
    public class TestFloorAreaRatioComponent
    {
        private Unit CreateUnit()
        {
            Unit unit = new Unit();

            UnitComponent uc = unit.UnitComponent;

            uc.AddRoomPlan(new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(4.0, 0.0, 0.0),
                    new Point(4.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            ));

            return unit;
        }

        private Unit CreateUnitWithWalkway()
        {
            Unit unit = new Unit();

            UnitComponent uc = unit.UnitComponent;

            uc.AddRoomPlan(new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(4.0, 0.0, 0.0),
                    new Point(4.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            ));

            return unit;
        }

        private Unit CreateUnitWithStair()
        {
            Unit unit = new Unit();

            UnitComponent uc = unit.UnitComponent;

            uc.AddRoomPlan(new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(4.0, 0.0, 0.0),
                    new Point(4.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            ));

            return unit;
        }

        [Fact]
        public void TestGetSiteArea()
        {
            var farc = new FloorAreaRatioComponent();

            Site site = new Site();

            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(20.0, 0.0, 0.0),
                    new Point(20.0, 10.0, 0.0),
                    new Point(10.0, 10.0, 0.0)
                }
            );

            Assert.Equal(150.0, farc.GetSiteArea(site));
        }

        [Fact]
        public void TestGetUnitArea()
        {
            var farc = new FloorAreaRatioComponent();

            Unit unit0 = CreateUnit();

            Assert.Equal(40.0, farc.GetUnitArea(unit0));

            Unit unit1 = CreateUnitWithWalkway();

            Assert.Equal(44.0, farc.GetUnitArea(unit1));

            Unit unit2 = CreateUnitWithStair();

            Assert.Equal(43.0, farc.GetUnitArea(unit2));
        }

        [Fact]
        public void TestGetTotalFloorArea()
        {
            Building building = new Building();

            BuildingComponent bc = building.BuildingComponent;

            bc.AddFloor(3.0);
            bc.AddFloor(3.0);
            bc.AddStack();
            bc.AddStack();

            bc.SetUnit(0, 0, CreateUnitWithWalkway());
            bc.SetUnit(1, 0, CreateUnitWithWalkway());
            bc.SetUnit(0, 1, CreateUnitWithStair());
            bc.SetUnit(1, 1, CreateUnitWithStair());

            var farc = new FloorAreaRatioComponent();

            Assert.Equal(174.0, farc.GetTotalFloorArea(building));
        }

        [Fact]
        public void TestGetTotalFloorArea_WithNullUnits()
        {
            Building building = new Building();

            BuildingComponent bc = building.BuildingComponent;

            bc.AddFloor(3.0);
            bc.AddFloor(3.0);
            bc.AddStack();
            bc.AddStack();

            bc.SetUnit(1, 0, CreateUnitWithWalkway());
            bc.SetUnit(0, 1, CreateUnitWithStair());

            var farc = new FloorAreaRatioComponent();

            Assert.Equal(87.0, farc.GetTotalFloorArea(building));
        }

        [Fact]
        public void TestGetFloorAreaRatio()
        {
            const int DecimalPlaces = 6;

            Site site = new Site();

            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(20.0, 0.0, 0.0),
                    new Point(20.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            );

            Building building = new Building();

            BuildingComponent bc = building.BuildingComponent;

            bc.AddFloor(3.0);
            bc.AddFloor(3.0);
            bc.AddStack();
            bc.AddStack();

            bc.SetUnit(0, 0, CreateUnitWithWalkway());
            bc.SetUnit(1, 0, CreateUnitWithWalkway());
            bc.SetUnit(0, 1, CreateUnitWithStair());
            bc.SetUnit(1, 1, CreateUnitWithStair());

            var farc = new FloorAreaRatioComponent();

            Assert.Equal(174.0, farc.GetTotalFloorArea(building));
            Assert.Equal(200.0, farc.GetSiteArea(site));
            Assert.Equal(
                0.87, farc.GetFloorAreaRatio(site, building),
                DecimalPlaces
            );
        }
    }
}
