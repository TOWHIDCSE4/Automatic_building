using DaiwaRentalGD.Model.BuildingDesign;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestUnitCatalog
    {
        [Fact]
        public void TestConstructor()
        {
            UnitCatalog uc = new UnitCatalog();

            Assert.Equal(
                new[] { uc.UnitCatalogComponent },
                uc.Components
            );
        }
    }
}
