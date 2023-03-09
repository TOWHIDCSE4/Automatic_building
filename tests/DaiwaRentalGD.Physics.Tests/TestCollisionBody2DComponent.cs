using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Physics.Tests
{
    public class TestCollisionBody2DComponent
    {
        #region Constructors

        public TestCollisionBody2DComponent(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConstructor()
        {
            var cbc = new CollisionBody2DComponent();

            Assert.Equal(
                new TrsTransform3D().Matrix,
                cbc.GetWorldTransform().Matrix
            );
            Assert.Empty(cbc.CollisionPolygons);
            Assert.Equal(
                CollisionBody2DComponent.DefaultEpsilon,
                cbc.Epsilon
            );
        }

        [Fact]
        public void TestDoesContain2D_Multiple()
        {
            var so0 = new SceneObject();
            so0.AddComponent(
                new TransformComponent
                {
                    Transform = new TrsTransform3D
                    {
                        Sx = 5.0,
                        Sy = 5.0
                    }
                }
            );
            var cbc0 = new CollisionBody2DComponent();
            cbc0.AddCollisionPolygon(
                new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(1.0, 0.0, 0.0),
                        new Point(1.0, 1.0, 0.0),
                        new Point(0.0, 1.0, 0.0)
                    }
                )
            );
            so0.AddComponent(cbc0);

            var so1 = new SceneObject();
            var cbc1 = new CollisionBody2DComponent();
            cbc1.AddCollisionPolygon(
                new Polygon(
                    new[]
                    {
                        new Point(1.0, 1.0, 0.0),
                        new Point(2.0, 1.0, 0.0),
                        new Point(1.5, 3.0, 0.0)
                    }
                )
            );
            so1.AddComponent(cbc1);

            var so2 = new SceneObject();
            var cbc2 = new CollisionBody2DComponent();
            cbc2.AddCollisionPolygon(
                new Polygon(
                    new[]
                    {
                        new Point(4.0, 0.5, 0.0),
                        new Point(6.0, 2.5, 0.0),
                        new Point(5.5, 0.5, 0.0)
                    }
                )
            );
            so2.AddComponent(cbc2);

            var so3 = new SceneObject();

            Assert.Equal(
                new[] { false, true, false, true },
                cbc0.DoesContain(new[] { so0, so1, so2, so3 })
            );
        }

        [Fact]
        public void TestDoesOverlap2D_Multiple()
        {
            var so0 = new SceneObject();
            so0.AddComponent(
                new TransformComponent
                {
                    Transform = new TrsTransform3D
                    {
                        Sx = 5.0,
                        Sy = 5.0
                    }
                }
            );
            var cbc0 = new CollisionBody2DComponent();
            cbc0.AddCollisionPolygon(
                new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(1.0, 0.0, 0.0),
                        new Point(1.0, 1.0, 0.0),
                        new Point(0.0, 1.0, 0.0)
                    }
                )
            );
            so0.AddComponent(cbc0);

            var so1 = new SceneObject();
            var cbc1 = new CollisionBody2DComponent();
            cbc1.AddCollisionPolygon(
                new Polygon(
                    new[]
                    {
                        new Point(1.0, 1.0, 0.0),
                        new Point(2.0, 1.0, 0.0),
                        new Point(1.5, 3.0, 0.0)
                    }
                )
            );
            so1.AddComponent(cbc1);

            var so2 = new SceneObject();
            var cbc2 = new CollisionBody2DComponent();
            cbc2.AddCollisionPolygon(
                new Polygon(
                    new[]
                    {
                        new Point(4.0, 0.5, 0.0),
                        new Point(6.0, 2.5, 0.0),
                        new Point(5.5, 0.5, 0.0)
                    }
                )
            );
            so2.AddComponent(
                new TransformComponent
                {
                    Transform = new TrsTransform3D
                    {
                        Tx = 10.0
                    }
                }
            );
            so2.AddComponent(cbc2);

            var so3 = new SceneObject();

            Assert.Equal(
                new[] { false, true, false, false },
                cbc0.DoesOverlap(new[] { so0, so1, so2, so3 })
            );
        }

        [Fact]
        public void TestWorldTransform()
        {
            var cbc0 = new CollisionBody2DComponent();

            var so1 = new SceneObject();
            var cbc1 = new CollisionBody2DComponent();
            so1.AddComponent(cbc1);

            var so2 = new SceneObject();
            var transform2 = new TrsTransform3D
            {
                Tx = 1.0,
                Ry = 2.0,
                Sz = 3.0
            };
            so2.AddComponent(
                new TransformComponent
                {
                    Transform = transform2
                }
            );
            var cbc2 = new CollisionBody2DComponent();
            so2.AddComponent(cbc2);

            Assert.Equal(
                new TrsTransform3D().Matrix,
                cbc0.GetWorldTransform().Matrix
            );
            Assert.Equal(
                new TrsTransform3D().Matrix,
                cbc1.GetWorldTransform().Matrix
            );
            Assert.Equal(
                transform2.Matrix,
                cbc2.GetWorldTransform().Matrix
            );
        }

        [Fact]
        public void TestSerialization()
        {
            var component = new CollisionBody2DComponent();

            component.AddCollisionPolygon(
                new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(1.0, 0.0, 0.0),
                        new Point(1.0, 1.0, 0.0),
                        new Point(0.0, 1.0, 0.0)
                    }
                )
            );
            component.AddCollisionPolygon(
                new Polygon(
                    new[]
                    {
                        new Point(50.0, 20.0, 0.0),
                        new Point(51.0, 20.0, 0.0),
                        new Point(51.0, 21.0, 0.0),
                        new Point(50.0, 21.0, 0.0)
                    }
                )
            );

            var workspace = new JsonWorkspace();
            workspace.Save(component);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedComponent =
                workspaceCopy.Load<CollisionBody2DComponent>(
                    component.ItemInfo.Uid
                );

            Assert.Equal(
                component.CollisionPolygons,
                deserializedComponent.CollisionPolygons
            );

            Assert.Equal(component.Epsilon, deserializedComponent.Epsilon);
        }

        #endregion

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
