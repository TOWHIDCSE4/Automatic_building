using System;
using Xunit;

namespace DaiwaRentalGD.Scene.Tests
{
    public class TestSceneObject
    {
        private class ComponentA : Component { }
        private class ComponentB : Component { }
        private class ComponentC : Component { }

        [Fact]
        public void TestConstructor()
        {
            SceneObject so = new SceneObject();

            Assert.Null(so.Name);
            Assert.Null(so.Scene);
            Assert.Null(so.Parent);
            Assert.Empty(so.Children);
            Assert.Empty(so.Descendants);
            Assert.Empty(so.Components);
        }

        [Fact]
        public void TestAddChild()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so0_0 = new SceneObject();
            SceneObject so0_1 = new SceneObject();
            SceneObject so0_1_0 = new SceneObject();

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so0_0);
            scene.AddSceneObject(so0_1);
            scene.AddSceneObject(so0_1_0);

            so0.AddChild(so0_0);
            so0.AddChild(so0_1);
            so0_1.AddChild(so0_1_0);

            Assert.Same(scene, so0.Scene);
            Assert.Null(so0.Parent);
            Assert.Equal(
                new[] { so0_0, so0_1 },
                so0.Children
            );
            Assert.Equal(
                new[] { so0_0, so0_1, so0_1_0 },
                so0.Descendants
            );

            Assert.Same(scene, so0_0.Scene);
            Assert.Same(so0, so0_0.Parent);
            Assert.Empty(so0_0.Children);
            Assert.Empty(so0_0.Descendants);

            Assert.Same(scene, so0_1.Scene);
            Assert.Same(so0, so0_1.Parent);
            Assert.Equal(
                new[] { so0_1_0 },
                so0_1.Children
            );
            Assert.Equal(
                new[] { so0_1_0 },
                so0_1.Descendants
            );

            Assert.Same(scene, so0_1_0.Scene);
            Assert.Same(so0_1, so0_1_0.Parent);
            Assert.Empty(so0_1_0.Children);
            Assert.Empty(so0_1_0.Descendants);
        }

        [Fact]
        public void TestAddChild_ParentChildNotInScene()
        {
            SceneObject so0 = new SceneObject();
            SceneObject so0_0 = new SceneObject();
            SceneObject so0_1 = new SceneObject();
            SceneObject so0_1_0 = new SceneObject();

            so0.AddChild(so0_0);
            so0.AddChild(so0_1);
            so0_1.AddChild(so0_1_0);

            Assert.Null(so0.Scene);
            Assert.Null(so0.Parent);
            Assert.Equal(
                new[] { so0_0, so0_1 },
                so0.Children
            );
            Assert.Equal(
                new[] { so0_0, so0_1, so0_1_0 },
                so0.Descendants
            );

            Assert.Null(so0_0.Scene);
            Assert.Same(so0, so0_0.Parent);
            Assert.Empty(so0_0.Children);
            Assert.Empty(so0_0.Descendants);

            Assert.Null(so0_1.Scene);
            Assert.Same(so0, so0_1.Parent);
            Assert.Equal(
                new[] { so0_1_0 },
                so0_1.Children
            );
            Assert.Equal(
                new[] { so0_1_0 },
                so0_1.Descendants
            );

            Assert.Null(so0_1_0.Scene);
            Assert.Same(so0_1, so0_1_0.Parent);
            Assert.Empty(so0_1_0.Children);
            Assert.Empty(so0_1_0.Descendants);
        }

        [Fact]
        public void TestAddChild_NullChild()
        {
            SceneObject so = new SceneObject();

            Assert.Throws<ArgumentNullException>(
                () => { so.AddChild(null); }
            );
        }

        [Fact]
        public void TestAddChild_ChildIsSelf()
        {
            SceneObject so = new SceneObject();

            Assert.ThrowsAny<ArgumentException>(
                () => { so.AddChild(so); }
            );

            Assert.Null(so.Parent);
            Assert.Empty(so.Children);
        }

        [Fact]
        public void TestAddChild_ChildIsAncestor()
        {
            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();
            SceneObject so2 = new SceneObject();

            so0.AddChild(so1);
            so1.AddChild(so2);

            Assert.ThrowsAny<ArgumentException>(
                () => { so1.AddChild(so0); }
            );
            Assert.Null(so0.Parent);
            Assert.Equal(so0, so1.Parent);
            Assert.Equal(new[] { so1 }, so0.Children);
            Assert.Equal(new[] { so2 }, so1.Children);

            Assert.ThrowsAny<ArgumentException>(
                () => { so2.AddChild(so0); }
            );
            Assert.Null(so0.Parent);
            Assert.Equal(so1, so2.Parent);
            Assert.Equal(new[] { so1 }, so0.Children);
            Assert.Empty(so2.Children);

            Assert.ThrowsAny<ArgumentException>(
                () => { so2.AddChild(so1); }
            );
            Assert.Equal(so0, so1.Parent);
            Assert.Equal(so1, so2.Parent);
            Assert.Equal(new[] { so2 }, so1.Children);
            Assert.Empty(so2.Children);
        }

        [Fact]
        public void TestAddChild_ChildNotInScene()
        {
            Scene scene0 = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();

            scene0.AddSceneObject(so0);

            Assert.NotSame(so0.Scene, so1.Scene);
            Assert.Null(so1.Scene);

            so0.AddChild(so1);

            Assert.Equal(new[] { so1 }, so0.Children);
            Assert.Equal(scene0, so1.Scene);
            Assert.Equal(so0, so1.Parent);
        }

        [Fact]
        public void TestAddChild_ChildInAnotherScene()
        {
            Scene scene0 = new Scene();
            Scene scene1 = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();

            scene0.AddSceneObject(so0);
            scene1.AddSceneObject(so1);

            Assert.NotSame(so0.Scene, so1.Scene);

            Assert.ThrowsAny<ArgumentException>(
                () => { so0.AddChild(so1); }
            );
            Assert.Empty(so0.Children);
            Assert.Same(scene1, so1.Scene);
            Assert.Null(so1.Parent);
        }

        [Fact]
        public void TestAddChild_ChildHasParent()
        {
            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();
            SceneObject so0_0 = new SceneObject();
            SceneObject so1_0 = new SceneObject();

            so0.AddChild(so0_0);
            so1.AddChild(so1_0);

            Assert.ThrowsAny<ArgumentException>(
                () => { so0.AddChild(so0_0); }
            );
            Assert.Equal(
                new[] { so0_0 },
                so0.Children
            );
            Assert.Same(so0, so0_0.Parent);

            Assert.ThrowsAny<ArgumentException>(
                () => { so0.AddChild(so1_0); }
            );
            Assert.Equal(
                new[] { so0_0 },
                so0.Children
            );
            Assert.Same(so1, so1_0.Parent);
        }

        [Fact]
        public void TestInsertChild_InvalidIndex()
        {
            SceneObject so = new SceneObject();

            SceneObject so0 = new SceneObject();
            so.AddChild(so0);

            SceneObject so1 = new SceneObject();
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { so.InsertChild(-1, so1); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { so.InsertChild(10, so1); }
            );

            Assert.Equal(new[] { so0 }, so.Children);
            Assert.Null(so1.Parent);
        }

        [Fact]
        public void TestRemoveChild()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so0_0 = new SceneObject();
            SceneObject so0_1 = new SceneObject();
            SceneObject so0_2 = new SceneObject();
            SceneObject so0_1_0 = new SceneObject();

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so0_0);
            scene.AddSceneObject(so0_1);
            scene.AddSceneObject(so0_2);
            scene.AddSceneObject(so0_1_0);

            so0.AddChild(so0_0);
            so0.AddChild(so0_1);
            so0.AddChild(so0_2);
            so0_1.AddChild(so0_1_0);

            Assert.Equal(
                new[] { so0, so0_0, so0_1, so0_2, so0_1_0 },
                scene.SceneObjects
            );
            Assert.Equal(
                new[] { so0 },
                scene.RootSceneObjects
            );

            Assert.Equal(
                new[] { so0_0, so0_1, so0_2 },
                so0.Children
            );

            so0.RemoveChild(so0_1);

            Assert.Equal(
                new[] { so0, so0_0, so0_1, so0_2, so0_1_0 },
                scene.SceneObjects
            );
            Assert.Equal(
                new[] { so0, so0_1 },
                scene.RootSceneObjects
            );

            Assert.Equal(
                new[] { so0_0, so0_2 },
                so0.Children
            );

            Assert.Same(scene, so0_1.Scene);
            Assert.Null(so0_1.Parent);
            Assert.Equal(
                new[] { so0_1_0 },
                so0_1.Children
            );

            Assert.Same(scene, so0_1_0.Scene);
            Assert.Same(so0_1, so0_1_0.Parent);
            Assert.Empty(so0_1_0.Children);
        }

        [Fact]
        public void TestRemoveChild_ChildNotFound()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so0_0 = new SceneObject();
            SceneObject so1 = new SceneObject();
            SceneObject so1_0 = new SceneObject();

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so1);
            scene.AddSceneObject(so0_0);
            scene.AddSceneObject(so1_0);

            so0.AddChild(so0_0);
            so1.AddChild(so1_0);

            bool isRemoved = so0.RemoveChild(so1_0);

            Assert.False(isRemoved);

            Assert.Equal(
                new[] { so0_0 },
                so0.Children
            );

            Assert.Same(scene, so1_0.Scene);
            Assert.Same(so1, so1_0.Parent);
        }

        [Fact]
        public void TestRemoveChild_NullChild()
        {
            Scene scene = new Scene();

            SceneObject so0 = new SceneObject();
            SceneObject so0_0 = new SceneObject();

            scene.AddSceneObject(so0);
            scene.AddSceneObject(so0_0);

            so0.AddChild(so0_0);

            bool isRemoved = so0.RemoveChild(null);

            Assert.False(isRemoved);

            Assert.Equal(
                new[] { so0_0 },
                so0.Children
            );
        }

        [Fact]
        public void TestInsertComponent()
        {
            SceneObject so0 = new SceneObject();

            Component c0_0 = new Component();
            Component c0_1 = new Component();

            so0.InsertComponent(0, c0_1);
            so0.InsertComponent(0, c0_0);

            Assert.Equal(
                new[] { c0_0, c0_1 },
                so0.Components
            );

            Assert.Same(so0, c0_0.SceneObject);
            Assert.Same(so0, c0_1.SceneObject);
        }

        [Fact]
        public void TestInsertComponent_InvalidIndex()
        {
            SceneObject so = new SceneObject();

            Component c0 = new Component();
            so.AddComponent(c0);

            Component c1 = new Component();
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { so.InsertComponent(-1, c1); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { so.InsertComponent(10, c1); }
            );

            Assert.Equal(new[] { c0 }, so.Components);
        }

        [Fact]
        public void TestInsertComponent_NullComponent()
        {
            SceneObject so0 = new SceneObject();

            Assert.Throws<ArgumentNullException>(
                () => { so0.InsertComponent(0, null); }
            );

            Assert.Empty(so0.Components);
        }

        [Fact]
        public void TestInsertComponent_ComponentIsAttached()
        {
            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();

            Component c0_0 = new Component();
            Component c1_0 = new Component();

            so0.AddComponent(c0_0);
            so1.AddComponent(c1_0);

            Assert.ThrowsAny<ArgumentException>(
                () => { so0.InsertComponent(0, c0_0); }
            );
            Assert.Equal(
                new[] { c0_0 },
                so0.Components
            );
            Assert.Same(so0, c0_0.SceneObject);

            Assert.ThrowsAny<ArgumentException>(
                () => { so0.InsertComponent(0, c1_0); }
            );
            Assert.Equal(
                new[] { c0_0 },
                so0.Components
            );
            Assert.Same(so1, c1_0.SceneObject);
        }

        [Fact]
        public void TestReplaceComponent()
        {
            SceneObject so = new SceneObject();

            Component c0 = new Component();
            Component c1 = new Component();
            Component c2 = new Component();
            Component c3 = new Component();
            Component c4 = new Component();

            so.AddComponent(c0);
            so.AddComponent(c1);

            Assert.True(so.ReplaceComponent(c0, c2));

            Assert.Equal(new[] {c2, c1}, so.Components);
            Assert.Null(c0.SceneObject);
            Assert.Equal(so, c2.SceneObject);

            Assert.False(so.ReplaceComponent(c3, c4));

            Assert.Equal(new[] { c2, c1 }, so.Components);
            Assert.Null(c3.SceneObject);
            Assert.Null(c4.SceneObject);

            Assert.False(so.ReplaceComponent(null, c4));

            Assert.Equal(new[] { c2, c1 }, so.Components);
            Assert.Null(c4.SceneObject);
        }

        [Fact]
        public void TestReplaceComponent_NullNewComponent()
        {
            SceneObject so = new SceneObject();

            Component c0 = new Component();
            Component c1 = new Component();
            Component c2 = new Component();

            so.AddComponent(c0);
            so.AddComponent(c1);

            Assert.Throws<ArgumentNullException>(
                () => { so.ReplaceComponent(c0, null); }
            );
            Assert.Throws<ArgumentNullException>(
                () => { so.ReplaceComponent(c2, null); }
            );
            Assert.Throws<ArgumentNullException>(
                () => { so.ReplaceComponent(null, null); }
            );

            Assert.Equal(new[] { c0, c1 }, so.Components);
            Assert.Equal(so, c0.SceneObject);
            Assert.Null(c2.SceneObject);
        }

        [Fact]
        public void TestReplaceComponent_NewComponentAlreadyAttached()
        {
            SceneObject so0 = new SceneObject();

            Component c0_0 = new Component();
            Component c0_1 = new Component();

            so0.AddComponent(c0_0);
            so0.AddComponent(c0_1);

            SceneObject so1 = new SceneObject();

            Component c1_0 = new Component();

            so1.AddComponent(c1_0);

            Component c = new Component();

            Assert.ThrowsAny<ArgumentException>(
                () => { so0.ReplaceComponent(c0_0, c1_0); }
            );
            Assert.ThrowsAny<ArgumentException>(
                () => { so0.ReplaceComponent(c, c1_0); }
            );
            Assert.ThrowsAny<ArgumentException>(
                () => { so0.ReplaceComponent(null, c1_0); }
            );

            Assert.Equal(new[] { c0_0, c0_1 }, so0.Components);
            Assert.Equal(so0, c0_0.SceneObject);
            Assert.Equal(new[] { c1_0 }, so1.Components);
            Assert.Equal(so1, c1_0.SceneObject);
            Assert.Null(c.SceneObject);

            Assert.ThrowsAny<ArgumentException>(
                () => { so0.ReplaceComponent(c0_0, c0_0); }
            );
            Assert.ThrowsAny<ArgumentException>(
                () => { so0.ReplaceComponent(c, c0_0); }
            );
            Assert.ThrowsAny<ArgumentException>(
                () => { so0.ReplaceComponent(null, c0_0); }
            );
            Assert.Equal(new[] { c0_0, c0_1 }, so0.Components);
            Assert.Equal(so0, c0_0.SceneObject);
            Assert.Null(c.SceneObject);
        }

        [Fact]
        public void TestRemoveComponent()
        {
            SceneObject so0 = new SceneObject();

            Component c0_0 = new Component();
            Component c0_1 = new Component();

            so0.AddComponent(c0_0);
            so0.AddComponent(c0_1);

            Assert.Equal(
                new[] { c0_0, c0_1 },
                so0.Components
            );

            bool isRemoved = so0.RemoveComponent(c0_1);

            Assert.True(isRemoved);

            Assert.Equal(
                new[] { c0_0 },
                so0.Components
            );

            Assert.Null(c0_1.SceneObject);
        }

        [Fact]
        public void TestRemoveComponent_ComponentNotFound()
        {
            SceneObject so0 = new SceneObject();
            SceneObject so1 = new SceneObject();

            Component c0_0 = new Component();
            Component c0_1 = new Component();
            Component c1_0 = new Component();

            so0.AddComponent(c0_0);
            so0.AddComponent(c0_1);
            so1.AddComponent(c1_0);

            bool isRemoved = so0.RemoveComponent(c1_0);

            Assert.False(isRemoved);

            Assert.Equal(
                new[] { c0_0, c0_1 },
                so0.Components
            );

            Assert.Same(so1, c1_0.SceneObject);
        }

        [Fact]
        public void TestRemoveComponent_NullComponent()
        {
            SceneObject so0 = new SceneObject();

            Component c0_0 = new Component();
            Component c0_1 = new Component();

            so0.AddComponent(c0_0);
            so0.AddComponent(c0_1);

            bool isRemoved = so0.RemoveComponent(null);

            Assert.False(isRemoved);

            Assert.Equal(
                new[] { c0_0, c0_1 },
                so0.Components
            );
        }

        [Fact]
        public void TestGetComponent()
        {
            SceneObject so0 = new SceneObject();

            Component c0_0 = new Component();
            ComponentA c0_a0 = new ComponentA();
            ComponentA c0_a1 = new ComponentA();
            ComponentB c0_b0 = new ComponentB();
            ComponentB c0_b1 = new ComponentB();
            ComponentA c0_a2 = new ComponentA();

            so0.AddComponent(c0_0);
            so0.AddComponent(c0_a0);
            so0.AddComponent(c0_a1);
            so0.AddComponent(c0_b0);
            so0.AddComponent(c0_b1);
            so0.AddComponent(c0_a2);

            Assert.Equal(
                new[] { c0_0, c0_a0, c0_a1, c0_b0, c0_b1, c0_a2 },
                so0.Components
            );

            Assert.Same(c0_0, so0.GetComponent<Component>());
            Assert.Same(c0_a0, so0.GetComponent<ComponentA>());
            Assert.Same(c0_b0, so0.GetComponent<ComponentB>());
            Assert.Null(so0.GetComponent<ComponentC>());
        }

        [Fact]
        public void TestGetComponents()
        {
            SceneObject so0 = new SceneObject();

            Component c0_0 = new Component();
            ComponentA c0_a0 = new ComponentA();
            ComponentA c0_a1 = new ComponentA();
            ComponentB c0_b0 = new ComponentB();
            ComponentB c0_b1 = new ComponentB();
            ComponentA c0_a2 = new ComponentA();

            so0.AddComponent(c0_0);
            so0.AddComponent(c0_a0);
            so0.AddComponent(c0_a1);
            so0.AddComponent(c0_b0);
            so0.AddComponent(c0_b1);
            so0.AddComponent(c0_a2);

            Assert.Equal(
                new[] { c0_0, c0_a0, c0_a1, c0_b0, c0_b1, c0_a2 },
                so0.Components
            );

            Assert.Equal(
                new[] { c0_0, c0_a0, c0_a1, c0_b0, c0_b1, c0_a2 },
                so0.GetComponents<Component>()
            );
            Assert.Equal(
                new[] { c0_a0, c0_a1, c0_a2 },
                so0.GetComponents<ComponentA>()
            );
            Assert.Equal(
                new[] { c0_b0, c0_b1 },
                so0.GetComponents<ComponentB>()
            );
            Assert.Empty(so0.GetComponents<ComponentC>());
        }
    }
}
