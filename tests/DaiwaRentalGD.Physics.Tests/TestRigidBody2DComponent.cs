using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Physics.Tests
{
    public class TestRigidBody2DComponent
    {
        #region Constructors

        public TestRigidBody2DComponent(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestSerialization()
        {
            var component = new RigidBody2DComponent
            {
                IsSpace = false,
                IsStatic = true,
                Mass = 0.123
            };

            var workspace = new JsonWorkspace();
            workspace.Save(component);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedComponent =
                workspaceCopy.Load<RigidBody2DComponent>(
                    component.ItemInfo.Uid
                );

            Assert.Equal(component.IsSpace, deserializedComponent.IsSpace);
            Assert.Equal(component.IsStatic, deserializedComponent.IsStatic);
            Assert.Equal(component.Mass, deserializedComponent.Mass);
        }

        #endregion

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
