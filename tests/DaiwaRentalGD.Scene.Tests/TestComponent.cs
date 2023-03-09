using Xunit;

namespace DaiwaRentalGD.Scene.Tests
{
    public class TestComponent
    {
        [Fact]
        public void TestConstructor()
        {
            Component component = new Component();

            Assert.Null(component.SceneObject);
        }
    }
}
