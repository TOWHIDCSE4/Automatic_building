using System;
using DaiwaRentalGD.Scene;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestTranformComponent
    {
        #region Constructors

        public TestTranformComponent(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConstructor()
        {
            TransformComponent tfComp = new TransformComponent();

            Assert.Equal(
                new TrsTransform3D(),
                tfComp.Transform
            );
        }

        [Fact]
        public void GetWorldTransform()
        {
            SceneObject so0 = new SceneObject(
                new[]
                {
                    new TransformComponent
                    {
                        Transform = new TrsTransform3D { Tx = 1.2 }
                    }
                }
            );

            SceneObject so1 = new SceneObject(
                new[]
                {
                    new TransformComponent
                    {
                        Transform = new TrsTransform3D { Sy = 3.4 }
                    }
                }
            );

            SceneObject so2 = new SceneObject(
                new[]
                {
                    new TransformComponent
                    {
                        Transform = new TrsTransform3D { Rz = 5.6 }
                    }
                }
            );

            so2.AddChild(so1);
            so1.AddChild(so0);

            Assert.Equal(
                new CompositeTransform3D(
                    new[]
                    {
                        new TrsTransform3D { Tx = 1.2 },
                        new TrsTransform3D { Sy = 3.4 },
                        new TrsTransform3D { Rz = 5.6 },
                    }
                ),
                so0.GetComponent<TransformComponent>().GetWorldTransform()
            );

            Assert.Equal(
                new CompositeTransform3D(
                    new[]
                    {
                        new TrsTransform3D { Sy = 3.4 },
                        new TrsTransform3D { Rz = 5.6 },
                    }
                ),
                so1.GetComponent<TransformComponent>().GetWorldTransform()
            );

            Assert.Equal(
                new CompositeTransform3D(
                    new[]
                    {
                        new TrsTransform3D { Rz = 5.6 },
                    }
                ),
                so2.GetComponent<TransformComponent>().GetWorldTransform()
            );
        }

        [Fact]
        public void GetWorldTransform_NoSceneObject()
        {
            TransformComponent tfComp = new TransformComponent
            {
                Transform = new TrsTransform3D
                { Tx = 1.2, Ry = 3.4, Sz = 5.6 }
            };

            Assert.Equal(
                new CompositeTransform3D(
                    new[]
                    {
                        new TrsTransform3D
                        { Tx = 1.2, Ry = 3.4, Sz = 5.6 }
                    }
                ),
                tfComp.GetWorldTransform()
            );
        }

        [Fact]
        public void GetWorldTransform_AncestorSceneObjectHasNoTransform()
        {
            SceneObject so0 = new SceneObject(
                new[]
                {
                    new TransformComponent
                    {
                        Transform = new TrsTransform3D { Tx = 1.2 }
                    }
                }
            );

            SceneObject so1 = new SceneObject(
                new[]
                {
                    new TransformComponent
                    {
                        Transform = new TrsTransform3D { Ry = 3.4 }
                    }
                }
            );

            SceneObject so2 = new SceneObject();

            SceneObject so3 = new SceneObject(
                new[]
                {
                    new TransformComponent
                    {
                        Transform = new TrsTransform3D { Sz = 5.6 }
                    }
                }
            );

            so3.AddChild(so2);
            so2.AddChild(so1);
            so1.AddChild(so0);

            Assert.Equal(
                new CompositeTransform3D(
                    new[]
                    {
                        new TrsTransform3D { Tx = 1.2 },
                        new TrsTransform3D { Ry = 3.4 },
                        new TrsTransform3D { Sz = 5.6 },
                    }
                ),
                so0.GetComponent<TransformComponent>().GetWorldTransform()
            );
        }

        [Fact]
        public void TestTransform_NullTransform()
        {
            TransformComponent tfComp = new TransformComponent();

            Assert.Throws<ArgumentNullException>(
                () => { tfComp.Transform = null; }
            );

            Assert.Equal(
                new TrsTransform3D(),
                tfComp.Transform
            );
        }

        [Fact]
        public void TestSerialization()
        {
            var component = new TransformComponent
            {
                Transform = new TrsTransform3D { Tx = 1.2 }
            };

            var workspace = new JsonWorkspace();
            workspace.Save(component);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedComponent =
                workspaceCopy.Load<TransformComponent>(
                    component.ItemInfo.Uid
                );

            Assert.Equal(
                component.Transform, deserializedComponent.Transform
            );
        }

        #endregion

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
