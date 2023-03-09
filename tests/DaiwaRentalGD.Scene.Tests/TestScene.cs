using System;
using System.Data;
using System.Runtime.Serialization;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Scene.Tests
{
    public class TestScene
    {
        #region Constructors

        public TestScene(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConstructor()
        {
            Scene scene = new Scene();

            Assert.Empty(scene.SceneObjects);
            Assert.Empty(scene.RootSceneObjects);
        }

        [Fact]
        public void TestInsertSceneObject()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();
            SceneObject so1_0 = new SceneObject();
            SceneObject so1_1 = new SceneObject();

            scene.AddSceneObject(so1_0);
            scene.AddSceneObject(so1_1);
            scene.InsertSceneObject(0, so0);
            scene.InsertSceneObject(1, so1);

            Assert.Same(scene, so0.Scene);
            Assert.Same(scene, so1.Scene);
            Assert.Same(scene, so1_0.Scene);
            Assert.Same(scene, so1_1.Scene);

            so1.AddChild(so1_0);
            so1.AddChild(so1_1);

            Assert.Equal(
                new[] { so0, so1, so1_0, so1_1 },
                scene.SceneObjects
            );

            Assert.Equal(
                new[] { so0, so1 },
                scene.RootSceneObjects
            );
        }

        [Fact]
        public void TestInsertSceneObject_NullSceneObject()
        {
            Scene scene = new Scene();

            Assert.Throws<ArgumentNullException>(
                () => { scene.InsertSceneObject(0, null); }
            );

            Assert.Empty(scene.SceneObjects);
        }

        [Fact]
        public void TestInsertSceneObject_SceneObjectInSameScene()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so0_0 = new SceneObject();

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so0_0);

            so0.AddChild(so0_0);

            Assert.Same(scene, so0.Scene);
            Assert.Same(scene, so0_0.Scene);

            Assert.ThrowsAny<ArgumentException>(
                () => { scene.InsertSceneObject(0, so0); }
            );
            Assert.ThrowsAny<ArgumentException>(
                () => { scene.AddSceneObject(so0_0); }
            );

            Assert.Equal(new[] { so0, so0_0 }, scene.SceneObjects);
        }

        [Fact]
        public void TestInsertSceneObject_SceneObjectInDiffScene()
        {
            Scene scene0 = new Scene();
            Scene scene1 = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();

            scene0.AddSceneObject(so0);
            scene1.AddSceneObject(so1);

            Assert.Same(scene0, so0.Scene);
            Assert.Same(scene1, so1.Scene);

            Assert.ThrowsAny<ArgumentException>(
                () => { scene0.InsertSceneObject(0, so1); }
            );

            Assert.Equal(new[] { so0 }, scene0.SceneObjects);
            Assert.Equal(new[] { so1 }, scene1.SceneObjects);

            Assert.Same(scene0, so0.Scene);
            Assert.Same(scene1, so1.Scene);
        }

        [Fact]
        public void TestReplaceSceneObject_RootSceneObject()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();
            SceneObject so2 = new SceneObject();

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so1);
            scene.AddSceneObject(so2);

            Assert.Equal(new[] { so0, so1, so2 }, scene.SceneObjects);

            SceneObject so3 = new SceneObject();
            SceneObject so4 = new SceneObject();
            SceneObject so5 = new SceneObject();

            Assert.True(scene.ReplaceSceneObject(so1, so3));
            Assert.Equal(new[] { so0, so3, so2 }, scene.SceneObjects);
            Assert.Null(so1.Scene);
            Assert.Same(scene, so3.Scene);

            Assert.False(scene.ReplaceSceneObject(so4, so5));
            Assert.Equal(new[] { so0, so3, so2 }, scene.SceneObjects);
            Assert.Null(so4.Scene);
            Assert.Null(so5.Scene);

            Assert.False(scene.ReplaceSceneObject(null, so4));
            Assert.Equal(new[] { so0, so3, so2 }, scene.SceneObjects);
            Assert.Null(so4.Scene);
        }

        [Fact]
        public void TestReplaceSceneObject_ChildSceneObject()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so0_0 = new SceneObject();
            SceneObject so0_1 = new SceneObject();
            SceneObject so0_2 = new SceneObject();
            SceneObject so0_3 = new SceneObject();
            SceneObject so0_2_0 = new SceneObject();
            SceneObject so0_2_1 = new SceneObject();

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so0_0);
            scene.AddSceneObject(so0_1);
            scene.AddSceneObject(so0_2_0);
            scene.AddSceneObject(so0_2);
            scene.AddSceneObject(so0_3);
            scene.AddSceneObject(so0_2_1);

            so0.AddChild(so0_0);
            so0.AddChild(so0_1);
            so0.AddChild(so0_2);
            so0.AddChild(so0_3);
            so0_2.AddChild(so0_2_0);
            so0_2.AddChild(so0_2_1);

            Assert.Equal(
                new[]
                {
                    so0, so0_0, so0_1, so0_2_0, so0_2, so0_3, so0_2_1
                },
                scene.SceneObjects
            );

            SceneObject so0_2_new = new SceneObject();

            Assert.True(scene.ReplaceSceneObject(so0_2, so0_2_new));

            Assert.Equal(
                new[]
                {
                    so0, so0_0, so0_1, so0_2_0, so0_2_new, so0_3, so0_2_1
                },
                scene.SceneObjects
            );

            Assert.Null(so0_2.Scene);
            Assert.Null(so0_2.Parent);
            Assert.Empty(so0_2.Children);

            Assert.Equal(scene, so0_2_new.Scene);
            Assert.Equal(so0, so0_2_new.Parent);
            Assert.Equal(new[] { so0_2_0, so0_2_1 }, so0_2_new.Children);

            Assert.Equal(
                new[] { so0_0, so0_1, so0_2_new, so0_3 },
                so0.Children
            );

            Assert.Equal(so0_2_new, so0_2_0.Parent);
            Assert.Equal(so0_2_new, so0_2_1.Parent);
        }

        [Fact]
        public void TestReplaceSceneObject_NullNewSceneObject()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();
            SceneObject so2 = new SceneObject();

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so1);
            scene.AddSceneObject(so2);

            Assert.Equal(new[] { so0, so1, so2 }, scene.SceneObjects);

            SceneObject so3 = new SceneObject();

            Assert.Throws<ArgumentNullException>(
                () => { scene.ReplaceSceneObject(so0, null); }
            );
            Assert.Throws<ArgumentNullException>(
                () => { scene.ReplaceSceneObject(so3, null); }
            );

            Assert.Equal(new[] { so0, so1, so2 }, scene.SceneObjects);
            Assert.Same(scene, so0.Scene);
            Assert.Null(so3.Scene);
        }

        [Fact]
        public void TestReplaceSceneObject_NewSceneObjectInAScene()
        {
            Scene scene0 = new Scene();

            SceneObject so0_0 = new SceneObject();
            SceneObject so0_1 = new SceneObject();

            scene0.AddSceneObject(so0_0);
            scene0.AddSceneObject(so0_1);

            Scene scene1 = new Scene();

            SceneObject so1_0 = new SceneObject();
            SceneObject so1_1 = new SceneObject();

            scene1.AddSceneObject(so1_0);
            scene1.AddSceneObject(so1_1);

            SceneObject so2_0 = new SceneObject();

            Assert.Equal(new[] { so0_0, so0_1 }, scene0.SceneObjects);
            Assert.Equal(new[] { so1_0, so1_1 }, scene1.SceneObjects);

            Assert.ThrowsAny<ArgumentException>(
                () => { scene0.ReplaceSceneObject(so0_0, so1_1); }
            );
            Assert.ThrowsAny<ArgumentException>(
                () => { scene0.ReplaceSceneObject(so0_0, so0_1); }
            );
            Assert.ThrowsAny<ArgumentException>(
                () => { scene0.ReplaceSceneObject(so2_0, so1_1); }
            );

            Assert.Equal(new[] { so0_0, so0_1 }, scene0.SceneObjects);
            Assert.Equal(new[] { so1_0, so1_1 }, scene1.SceneObjects);
            Assert.Same(scene0, so0_0.Scene);
            Assert.Same(scene1, so1_1.Scene);
            Assert.Null(so2_0.Scene);
        }

        [Fact]
        public void TestRemoveSceneObject_RootSceneObject()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();
            SceneObject so0_0 = new SceneObject();
            SceneObject so0_1 = new SceneObject();
            SceneObject so0_0_0 = new SceneObject();

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so1);
            scene.AddSceneObject(so0_0);
            scene.AddSceneObject(so0_1);
            scene.AddSceneObject(so0_0_0);

            so0.AddChild(so0_0);
            so0.AddChild(so0_1);
            so0_0.AddChild(so0_0_0);

            bool isRemoved = scene.RemoveSceneObject(so0);

            Assert.True(isRemoved);

            Assert.Equal(
                new[] { so1 },
                scene.RootSceneObjects
            );
            Assert.Equal(
                new[] { so1 },
                scene.SceneObjects
            );

            Assert.Equal(
                new[] { so0_0, so0_1 },
                so0.Children
            );

            Assert.Null(so0.Scene);
            Assert.Null(so0_0.Scene);
            Assert.Null(so0_1.Scene);
            Assert.Null(so0_0_0.Scene);

            Assert.Null(so0.Parent);
            Assert.Same(so0, so0_0.Parent);
            Assert.Same(so0, so0_1.Parent);
            Assert.Same(so0_0, so0_0_0.Parent);
        }

        [Fact]
        public void TestRemoveSceneObject_ParentedSceneObject()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();
            SceneObject so0_0 = new SceneObject();
            SceneObject so0_1 = new SceneObject();
            SceneObject so0_0_0 = new SceneObject();

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so1);
            scene.AddSceneObject(so0_0);
            scene.AddSceneObject(so0_1);
            scene.AddSceneObject(so0_0_0);

            so0.AddChild(so0_0);
            so0.AddChild(so0_1);
            so0_0.AddChild(so0_0_0);

            bool isRemoved = scene.RemoveSceneObject(so0_0);

            Assert.True(isRemoved);

            Assert.Equal(
                new[] { so0, so1 },
                scene.RootSceneObjects
            );
            Assert.Equal(
                new[] { so0, so1, so0_1 },
                scene.SceneObjects
            );

            Assert.Equal(
                new[] { so0_1 },
                so0.Children
            );

            Assert.Null(so0_0.Scene);
            Assert.Null(so0_0_0.Scene);

            Assert.Null(so0_0.Parent);
            Assert.Same(so0_0, so0_0_0.Parent);
        }

        [Fact]
        public void TestRemoveSceneObject_SceneObjectNotFound()
        {
            Scene scene0 = new Scene();
            Scene scene1 = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();
            SceneObject so1_0 = new SceneObject();

            scene0.AddSceneObject(so0);
            scene1.AddSceneObject(so1);
            scene1.AddSceneObject(so1_0);

            so1.AddChild(so1_0);

            bool isRemoved = scene0.RemoveSceneObject(so1_0);

            Assert.False(isRemoved);

            Assert.Same(scene1, so1_0.Scene);
            Assert.Same(so1, so1_0.Parent);
        }

        [Fact]
        public void TestRemoveSceneObject_NullSceneObject()
        {
            Scene scene = new Scene();

            bool isRemoved = scene.RemoveSceneObject(null);

            Assert.False(isRemoved);
        }

        [Fact]
        public void TestSerialization()
        {
            // Set up test scene

            var scene = new Scene { Name = "Scene" };

            var so0 = new SceneObject(
                new[]
                {
                    new Component { Name = "Component 0.0" },
                    new Component { Name = "Component 0.1" }
                }
            )
            {
                Name = "Scene Object 0"
            };

            var so1 = new SceneObject { Name = "Scene Object 1" };

            var so2 = new SceneObject { Name = "Scene Object 2" };

            so1.AddChild(so2);

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so1);

            // Serialize

            var workspace = new JsonWorkspace();
            workspace.Save(scene);

            OutputHelper.WriteLine(workspace.ToJson());

            // Deserialize

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedScene =
                workspaceCopy.Load<Scene>(scene.ItemInfo.Uid);

            // Check deserialzied scene

            Assert.Equal(scene.Name, deserializedScene.Name);
            Assert.Equal(scene.IsEnabled, deserializedScene.IsEnabled);
            Assert.Equal(
                scene.IsExecutingUpdate, deserializedScene.IsExecutingUpdate
            );

            var deserializedSceneObjects = deserializedScene.SceneObjects;
            Assert.Equal(
                scene.SceneObjects.Count,
                deserializedSceneObjects.Count
            );

            var deseiralizedSo0 = deserializedSceneObjects[0];
            Assert.Equal(so0.Name, deseiralizedSo0.Name);
            Assert.Equal(
                so0.Components.Count,
                deseiralizedSo0.Components.Count
            );
            Assert.Equal(
                so0.Components[0].Name,
                deseiralizedSo0.Components[0].Name
            );
            Assert.Equal(
                so0.Components[1].Name,
                deseiralizedSo0.Components[1].Name
            );
            Assert.Null(so0.Parent);
            Assert.Empty(so0.Children);

            var deseiralizedSo1 = deserializedSceneObjects[1];
            var deseiralizedSo2 = deserializedSceneObjects[2];

            Assert.Equal(
                new[] { deseiralizedSo2 },
                deseiralizedSo1.Children
            );
            Assert.Same(deseiralizedSo1, deseiralizedSo2.Parent);
        }

        #endregion

        #region Fields

        private readonly ITestOutputHelper OutputHelper;

        #endregion
    }
}
