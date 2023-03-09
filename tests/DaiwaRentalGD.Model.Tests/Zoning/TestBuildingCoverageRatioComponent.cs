using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Model.Zoning;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.Zoning
{
    public class TestBuildingCoverageRatioComponent
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
            var bcrc = new BuildingCoverageRatioComponent();

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

            Assert.Equal(150.0, bcrc.GetSiteArea(site));
        }

        [Fact]
        public void TestGetUnitArea()
        {
            var bcrc = new BuildingCoverageRatioComponent();

            Unit unit0 = CreateUnit();

            Assert.Equal(43.0, bcrc.GetUnitArea(unit0));

            Unit unit1 = CreateUnitWithWalkway();

            Assert.Equal(47.0, bcrc.GetUnitArea(unit1));

            Unit unit2 = CreateUnitWithStair();

            Assert.Equal(46.0, bcrc.GetUnitArea(unit2));
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

            var bcrc = new BuildingCoverageRatioComponent();

            Assert.Equal(93.0, bcrc.GetBuildingArea(building));
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

            var bcrc = new BuildingCoverageRatioComponent();

            Assert.Equal(46.0, bcrc.GetBuildingArea(building));
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

            var bcrc = new BuildingCoverageRatioComponent();

            Assert.Equal(93.0, bcrc.GetBuildingArea(building));
            Assert.Equal(200.0, bcrc.GetSiteArea(site));
            Assert.Equal(
                0.465, bcrc.GetBuildingCoverageRatio(site, building),
                DecimalPlaces
            );
        }
    }
}
