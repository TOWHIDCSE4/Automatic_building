using System;
using DaiwaRentalGD.Model.BuildingDesign;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestUnit
    {
        [Fact]
        public void TestUnitComponent()
        {
            Unit unit = new Unit();

            UnitComponent uc = new UnitComponent();

            unit.UnitComponent = uc;

            Assert.Equal(uc, unit.UnitComponent);
            Assert.Contains(uc, unit.Components);
            Assert.Equal(unit, uc.SceneObject);
        }

        [Fact]
        public void TestUnitComponent_NullValue()
        {
            Unit unit = new Unit();

            UnitComponent uc = new UnitComponent();

            unit.UnitComponent = uc;

            Assert.Equal(uc, unit.UnitComponent);
            Assert.Contains(uc, unit.Components);
            Assert.Equal(unit, uc.SceneObject);

            Assert.Throws<ArgumentNullException>(
                () => { unit.UnitComponent = null; }
            );

            Assert.Equal(uc, unit.UnitComponent);
            Assert.Contains(uc, unit.Components);
            Assert.Equal(unit, uc.SceneObject);
        }
    }
}
