using System;
using System.Linq;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Scene;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestBuildingComponent
    {
        private Unit MakeUnitWithSizeXY(double sizeX, double sizeY)
        {
            Unit unit = new Unit();

            Polygon roomPlan = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(sizeX, 0.0, 0.0),
                    new Point(sizeX, sizeY, 0.0),
                    new Point(0.0, sizeY, 0.0)
                }
            );

            unit.UnitComponent.AddRoomPlan(roomPlan);

            return unit;
        }

        [Fact]
        public void TestConstructor()
        {
            BuildingComponent bc = new BuildingComponent();

            Assert.Empty(bc.FloorHeights);
            Assert.Equal(0.0, bc.TotalFloorHeight);
            Assert.Equal(0, bc.NumOfFloors);
            Assert.Equal(0, bc.NumOfUnitsPerFloor);
            Assert.Equal(0, bc.NumOfUnits);
            Assert.Empty(bc.Units);
        }

        [Fact]
        public void TestInsertFloor_NoStack()
        {
            BuildingComponent bc = new BuildingComponent();

            bc.InsertFloor(0, 3.0);
            bc.InsertFloor(0, 4.0);
            bc.InsertFloor(2, 5.0);

            Assert.Equal(
                new[] { 4.0, 3.0, 5.0 },
                bc.FloorHeights
            );
            Assert.Equal(12.0, bc.TotalFloorHeight);
            Assert.Equal(3, bc.NumOfFloors);
            Assert.Equal(0, bc.NumOfUnitsPerFloor);
            Assert.Equal(0, bc.NumOfUnits);
            Assert.Empty(bc.Units);
        }

        [Fact]
        public void TestInsertFloor_NoUnit()
        {
            BuildingComponent bc = new BuildingComponent();

            bc.AddStack();
            bc.AddStack();
            bc.AddStack();

            bc.InsertFloor(0, 3.0);
            bc.InsertFloor(0, 4.0);
            bc.InsertFloor(2, 5.0);

            Assert.Equal(
                new[] { 4.0, 3.0, 5.0 },
                bc.FloorHeights
            );
            Assert.Equal(12.0, bc.TotalFloorHeight);
            Assert.Equal(3, bc.NumOfFloors);
            Assert.Equal(3, bc.NumOfUnitsPerFloor);
            Assert.Equal(9, bc.NumOfUnits);
            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 9),
                bc.Units
            );

            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 3),
                bc.GetFloorUnits(0)
            );
            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 3),
                bc.GetFloorUnits(1)
            );
            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 3),
                bc.GetFloorUnits(2)
            );
        }

        [Fact]
        public void TestInsertFloor()
        {
            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            building.AddComponent(bc);

            bc.AddStack();
            bc.AddStack();
            bc.AddStack();

            bc.AddFloor(3.0);
            bc.AddFloor(4.0);

            Unit unit0 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit1 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit2 = MakeUnitWithSizeXY(5.0, 10.0);

            bc.SetUnit(0, 0, unit0);
            bc.SetUnit(1, 1, unit1);
            bc.SetUnit(1, 2, unit2);

            bc.InsertFloor(1, 5.0);

            Assert.Equal(
                new[] { 3.0, 5.0, 4.0 },
                bc.FloorHeights
            );
            Assert.Equal(12.0, bc.TotalFloorHeight);
            Assert.Equal(3, bc.NumOfFloors);
            Assert.Equal(3, bc.NumOfUnitsPerFloor);
            Assert.Equal(9, bc.NumOfUnits);
            Assert.Equal(
                new Unit[]
                {
                    unit0, null, null,
                    null, null, null,
                    null, unit1, unit2
                },
                bc.Units
            );

            Assert.Equal(
                new[] { unit0, null, null },
                bc.GetFloorUnits(0)
            );
            Assert.Equal(
                new Unit[] { null, null, null },
                bc.GetFloorUnits(1)
            );
            Assert.Equal(
                new[] { null, unit1, unit2 },
                bc.GetFloorUnits(2)
            );

            Assert.Equal(
                new TrsTransform3D { Tx = 0.0, Tz = 0.0 },
                unit0.TransformComponent.Transform
            );
            Assert.Equal(
                new TrsTransform3D { Tx = 0.0, Tz = 8.0 },
                unit1.TransformComponent.Transform
            );
            Assert.Equal(
                new TrsTransform3D { Tx = 5.0, Tz = 8.0 },
                unit2.TransformComponent.Transform
            );
        }

        [Fact]
        public void TestInsertFloor_InvalidFloorHeight()
        {
            BuildingComponent bc = new BuildingComponent();

            Assert.ThrowsAny<ArgumentException>(
                () => { bc.AddFloor(0.0); }
            );

            Assert.ThrowsAny<ArgumentException>(
                () => { bc.AddFloor(-1.0); }
            );

            Assert.Empty(bc.FloorHeights);
            Assert.Equal(0.0, bc.TotalFloorHeight);
            Assert.Equal(0, bc.NumOfFloors);
            Assert.Equal(0, bc.NumOfUnitsPerFloor);
            Assert.Equal(0, bc.NumOfUnits);
            Assert.Empty(bc.Units);
        }

        [Fact]
        public void TestInsertFloor_NoStackInvalidFloor()
        {
            BuildingComponent bc = new BuildingComponent();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.InsertFloor(10, 3.0); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.InsertFloor(-1, 3.0); }
            );

            Assert.Equal(0, bc.NumOfFloors);
            Assert.Empty(bc.Units);
        }

        [Fact]
        public void TestRemoveFloor()
        {
            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            building.AddComponent(bc);

            bc.AddStack();
            bc.AddStack();
            bc.AddStack();

            bc.AddFloor(5.0);
            bc.AddFloor(3.0);
            bc.AddFloor(4.0);

            Unit unit0 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit1 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit2 = MakeUnitWithSizeXY(5.0, 10.0);

            bc.SetUnit(0, 0, unit0);
            bc.SetUnit(1, 1, unit1);
            bc.SetUnit(2, 2, unit2);

            bc.RemoveFloor(1);

            Assert.Equal(
                new[] { 5.0, 4.0 },
                bc.FloorHeights
            );
            Assert.Equal(9.0, bc.TotalFloorHeight);
            Assert.Equal(2, bc.NumOfFloors);
            Assert.Equal(3, bc.NumOfUnitsPerFloor);
            Assert.Equal(6, bc.NumOfUnits);
            Assert.Equal(
                new Unit[]
                {
                    unit0, null, null,
                    null, null, unit2
                },
                bc.Units
            );
            Assert.Equal(
                new[] { unit0, unit2 },
                building.Children
            );

            Assert.Null(unit1.Parent);

            Assert.Equal(
                new[] { unit0, null, null },
                bc.GetFloorUnits(0)
            );
            Assert.Equal(
                new[] { null, null, unit2 },
                bc.GetFloorUnits(1)
            );

            Assert.Equal(
                new TrsTransform3D { Tx = 0.0, Tz = 0.0 },
                unit0.TransformComponent.Transform
            );
            Assert.Equal(
                new TrsTransform3D { Tx = 0.0, Tz = 5.0 },
                unit2.TransformComponent.Transform
            );
        }

        [Fact]
        public void TestRemoveFloor_NoFloorNoStack()
        {
            BuildingComponent bc = new BuildingComponent();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.RemoveFloor(0); }
            );

            Assert.Equal(0, bc.NumOfFloors);
        }

        [Fact]
        public void TestInsertStack_NoFloor()
        {
            BuildingComponent bc = new BuildingComponent();

            bc.AddStack();
            bc.AddStack();
            bc.AddStack();

            Assert.Empty(bc.FloorHeights);
            Assert.Equal(0.0, bc.TotalFloorHeight);
            Assert.Equal(0, bc.NumOfFloors);
            Assert.Equal(3, bc.NumOfUnitsPerFloor);
            Assert.Equal(0, bc.NumOfUnits);
            Assert.Empty(bc.Units);

            Assert.Empty(bc.GetStackUnits(0));
            Assert.Empty(bc.GetStackUnits(1));
            Assert.Empty(bc.GetStackUnits(2));
        }

        [Fact]
        public void TestInsertStack_NoUnit()
        {
            BuildingComponent bc = new BuildingComponent();

            bc.InsertFloor(0, 3.0);
            bc.InsertFloor(0, 4.0);
            bc.InsertFloor(2, 5.0);

            bc.AddStack();
            bc.AddStack();
            bc.AddStack();

            Assert.Equal(
                new[] { 4.0, 3.0, 5.0 },
                bc.FloorHeights
            );
            Assert.Equal(12.0, bc.TotalFloorHeight);
            Assert.Equal(3, bc.NumOfFloors);
            Assert.Equal(3, bc.NumOfUnitsPerFloor);
            Assert.Equal(9, bc.NumOfUnits);
            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 9),
                bc.Units
            );

            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 3),
                bc.GetFloorUnits(0)
            );
            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 3),
                bc.GetFloorUnits(1)
            );
            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 3),
                bc.GetFloorUnits(2)
            );
            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 3),
                bc.GetStackUnits(0)
            );
            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 3),
                bc.GetStackUnits(1)
            );
            Assert.Equal(
                Enumerable.Repeat<Unit>(null, 3),
                bc.GetStackUnits(2)
            );
        }

        [Fact]
        public void TestInsertStack()
        {
            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            building.AddComponent(bc);

            bc.AddStack();
            bc.AddStack();

            bc.AddFloor(3.0);
            bc.AddFloor(4.0);
            bc.AddFloor(5.0);

            Unit unit0 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit1 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit2 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit3 = MakeUnitWithSizeXY(5.0, 10.0);

            bc.SetUnit(0, 0, unit0);
            bc.SetUnit(1, 1, unit1);
            bc.SetUnit(2, 1, unit2);

            bc.InsertStack(1);

            bc.SetUnit(2, 1, unit3);

            Assert.Equal(
                new[] { 3.0, 4.0, 5.0 },
                bc.FloorHeights
            );
            Assert.Equal(12.0, bc.TotalFloorHeight);
            Assert.Equal(3, bc.NumOfFloors);
            Assert.Equal(3, bc.NumOfUnitsPerFloor);
            Assert.Equal(9, bc.NumOfUnits);
            Assert.Equal(
                new Unit[]
                {
                    unit0, null, null,
                    null, null, unit1,
                    null, unit3, unit2
                },
                bc.Units
            );

            Assert.Equal(
                new[] { unit0, null, null },
                bc.GetFloorUnits(0)
            );
            Assert.Equal(
                new Unit[] { null, null, unit1 },
                bc.GetFloorUnits(1)
            );
            Assert.Equal(
                new[] { null, unit3, unit2 },
                bc.GetFloorUnits(2)
            );
            Assert.Equal(
                new[] { unit0, null, null },
                bc.GetStackUnits(0)
            );
            Assert.Equal(
                new Unit[] { null, null, unit3 },
                bc.GetStackUnits(1)
            );
            Assert.Equal(
                new[] { null, unit1, unit2 },
                bc.GetStackUnits(2)
            );

            Assert.Equal(
                new TrsTransform3D { Tx = 0.0, Tz = 0.0 },
                unit0.TransformComponent.Transform
            );
            Assert.Equal(
                new TrsTransform3D { Tx = 0.0, Tz = 3.0 },
                unit1.TransformComponent.Transform
            );
            Assert.Equal(
                new TrsTransform3D { Tx = 5.0, Tz = 7.0 },
                unit2.TransformComponent.Transform
            );
            Assert.Equal(
                new TrsTransform3D { Tx = 0.0, Tz = 7.0 },
                unit3.TransformComponent.Transform
            );
        }

        [Fact]
        public void TestInsertStack_NoFloorInvalidStack()
        {
            BuildingComponent bc = new BuildingComponent();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.InsertStack(10); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.InsertStack(-1); }
            );

            Assert.Equal(0, bc.NumOfUnitsPerFloor);
            Assert.Empty(bc.Units);
        }

        [Fact]
        public void TestRemoveStack()
        {
            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            building.AddComponent(bc);

            bc.AddStack();
            bc.AddStack();
            bc.AddStack();

            bc.AddFloor(3.0);
            bc.AddFloor(4.0);
            bc.AddFloor(5.0);

            Unit unit0 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit1 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit2 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit3 = MakeUnitWithSizeXY(5.0, 10.0);

            bc.SetUnit(0, 0, unit0);
            bc.SetUnit(1, 2, unit1);
            bc.SetUnit(2, 1, unit2);
            bc.SetUnit(2, 2, unit3);

            bc.RemoveStack(1);

            Assert.Equal(
                new[] { 3.0, 4.0, 5.0 },
                bc.FloorHeights
            );
            Assert.Equal(12.0, bc.TotalFloorHeight);
            Assert.Equal(3, bc.NumOfFloors);
            Assert.Equal(2, bc.NumOfUnitsPerFloor);
            Assert.Equal(6, bc.NumOfUnits);
            Assert.Equal(
                new Unit[]
                {
                    unit0, null,
                    null, unit1,
                    null, unit3
                },
                bc.Units
            );
            Assert.Equal(
                new[] { unit0, unit1, unit3 },
                building.Children
            );

            Assert.Null(unit2.Parent);

            Assert.Equal(
                new[] { unit0, null },
                bc.GetFloorUnits(0)
            );
            Assert.Equal(
                new Unit[] { null, unit1 },
                bc.GetFloorUnits(1)
            );
            Assert.Equal(
                new[] { null, unit3 },
                bc.GetFloorUnits(2)
            );
            Assert.Equal(
                new[] { unit0, null, null },
                bc.GetStackUnits(0)
            );
            Assert.Equal(
                new[] { null, unit1, unit3 },
                bc.GetStackUnits(1)
            );

            Assert.Equal(
                new TrsTransform3D { Tx = 0.0, Tz = 0.0 },
                unit0.TransformComponent.Transform
            );
            Assert.Equal(
                new TrsTransform3D { Tx = 0.0, Tz = 3.0 },
                unit1.TransformComponent.Transform
            );
            Assert.Equal(
                new TrsTransform3D { Tx = 0.0, Tz = 7.0 },
                unit3.TransformComponent.Transform
            );
        }

        [Fact]
        public void TestRemoveStack_NoFloorNoStack()
        {
            BuildingComponent bc = new BuildingComponent();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.RemoveStack(0); }
            );

            Assert.Equal(0, bc.NumOfUnitsPerFloor);
        }

        [Fact]
        public void TestSetUnit()
        {
            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            building.AddComponent(bc);

            bc.AddFloor(4.0);
            bc.AddFloor(3.0);
            bc.AddStack();
            bc.AddStack();
            bc.AddStack();

            Unit unit = MakeUnitWithSizeXY(5.0, 9.0);

            bc.SetUnit(1, 2, unit);

            Assert.Equal(unit, bc.GetUnit(1, 2));
            Assert.Equal(
                new[]
                {
                    null, null, null,
                    null, null, unit
                },
                bc.Units
            );
            Assert.Equal(
                new[] { unit },
                building.Children
            );

            Assert.Equal(building, unit.Parent);
            Assert.Equal(3.0, unit.UnitComponent.RoomHeight);
        }

        [Fact]
        public void TestSetUnit_NoSceneObject()
        {
            BuildingComponent bc = new BuildingComponent();

            bc.AddFloor(4.0);
            bc.AddFloor(5.0);
            bc.AddStack();
            bc.AddStack();

            Unit unit = MakeUnitWithSizeXY(5.0, 10.0);

            Assert.Null(bc.SceneObject);

            Assert.ThrowsAny<NullReferenceException>(
                () => { bc.SetUnit(0, 0, unit); }
            );

            Assert.Equal(
                new Unit[]
                {
                    null, null,
                    null, null
                },
                bc.Units
            );

            Assert.Null(unit.Parent);
            Assert.Equal(
                UnitComponent.DefaultRoomHeight,
                unit.UnitComponent.RoomHeight
            );
        }

        [Fact]
        public void TestSetUnit_NullUnit()
        {
            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            building.AddComponent(bc);

            bc.AddFloor(3.0);
            bc.AddFloor(3.0);
            bc.AddStack();
            bc.AddStack();

            Assert.ThrowsAny<ArgumentNullException>(
                () => { bc.SetUnit(0, 0, null); }
            );

            Assert.Equal(
                new Unit[]
                {
                    null, null,
                    null, null
                },
                bc.Units
            );
            Assert.Empty(building.Children);
        }

        [Fact]
        public void TestSetUnit_InvalidFloorOrStack()
        {
            BuildingComponent bc = new BuildingComponent();
            SceneObject building = new SceneObject();
            building.AddComponent(bc);

            bc.AddFloor(4.0);
            bc.AddFloor(5.0);
            bc.AddStack();
            bc.AddStack();

            Unit unit = MakeUnitWithSizeXY(5.0, 10.0);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.SetUnit(-1, 0, unit); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.SetUnit(0, -1, unit); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.SetUnit(10, 0, unit); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.SetUnit(0, 10, unit); }
            );

            Assert.Equal(Enumerable.Repeat<Unit>(null, 4), bc.Units);
            Assert.Empty(building.Children);

            Assert.Null(unit.Parent);
            Assert.Equal(
                UnitComponent.DefaultRoomHeight,
                unit.UnitComponent.RoomHeight
            );
        }

        [Fact]
        public void TestSetUnit_UnitHasParent()
        {
            BuildingComponent bc = new BuildingComponent();
            SceneObject building = new SceneObject();
            building.AddComponent(bc);

            bc.AddFloor(4.0);
            bc.AddFloor(5.0);
            bc.AddStack();
            bc.AddStack();

            Unit unit = MakeUnitWithSizeXY(5.0, 10.0);
            SceneObject unitParent = new SceneObject();
            unitParent.AddChild(unit);

            Assert.ThrowsAny<ArgumentException>(
                () => { bc.SetUnit(0, 0, unit); }
            );

            Assert.Equal(Enumerable.Repeat<Unit>(null, 4), bc.Units);
            Assert.Empty(building.Children);

            Assert.Equal(unitParent, unit.Parent);
            Assert.Equal(
                UnitComponent.DefaultRoomHeight,
                unit.UnitComponent.RoomHeight
            );
        }

        [Fact]
        public void TestSetUnit_UnitInDiffScene()
        {
            Scene.Scene scene0 = new Scene.Scene();
            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            scene0.AddSceneObject(building);
            building.AddComponent(bc);

            bc.AddFloor(4.0);
            bc.AddFloor(5.0);
            bc.AddStack();
            bc.AddStack();

            Scene.Scene scene1 = new Scene.Scene();
            Unit unit = MakeUnitWithSizeXY(5.0, 10.0);
            scene1.AddSceneObject(unit);

            Assert.ThrowsAny<ArgumentException>(
                () => { bc.SetUnit(0, 0, unit); }
            );

            Assert.Equal(Enumerable.Repeat<Unit>(null, 4), bc.Units);
            Assert.Empty(building.Children);

            Assert.Null(unit.Parent);
            Assert.Equal(scene1, unit.Scene);
            Assert.Equal(
                UnitComponent.DefaultRoomHeight,
                unit.UnitComponent.RoomHeight
            );
        }

        [Fact]
        public void TestSetUnit_UnitAlreadySet()
        {
            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            building.AddComponent(bc);

            bc.AddFloor(4.0);
            bc.AddFloor(5.0);
            bc.AddStack();
            bc.AddStack();

            Unit unit0 = MakeUnitWithSizeXY(5.0, 10.0);
            Unit unit1 = MakeUnitWithSizeXY(5.0, 10.0);

            bc.SetUnit(0, 0, unit0);

            Assert.ThrowsAny<InvalidOperationException>(
                () => { bc.SetUnit(0, 0, unit1); }
            );

            Assert.Equal(
                new[]
                {
                    unit0, null,
                    null, null
                },
                bc.Units
            );
            Assert.Equal(new[] { unit0 }, building.Children);

            Assert.Equal(building, unit0.Parent);

            Assert.Null(unit1.Parent);
            Assert.Equal(
                UnitComponent.DefaultRoomHeight,
                unit1.UnitComponent.RoomHeight
            );
        }

        [Fact]
        public void TestRemoveUnit()
        {
            Scene.Scene scene = new Scene.Scene();

            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            building.AddComponent(bc);

            bc.AddFloor(4.0);
            bc.AddFloor(5.0);
            bc.AddStack();
            bc.AddStack();
            bc.AddStack();

            Unit unit = MakeUnitWithSizeXY(5.0, 8.0);

            scene.AddSceneObject(building);
            scene.AddSceneObject(unit);

            bc.SetUnit(1, 1, unit);

            bc.RemoveUnit(1, 1);

            Assert.Null(bc.GetUnit(1, 1));
            Assert.Equal(Enumerable.Repeat<Unit>(null, 6), bc.Units);
            Assert.Empty(building.Children);

            Assert.Null(unit.Parent);
            Assert.Null(unit.Scene);

            Assert.Equal(new[] { building }, scene.SceneObjects);

            bc.RemoveUnit(0, 0);
            Assert.Null(bc.GetUnit(0, 0));
        }

        [Fact]
        public void TestRemoveUnit_InvalidFloorOrStack()
        {
            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            building.AddComponent(bc);

            bc.AddFloor(4.0);
            bc.AddFloor(3.0);
            bc.AddStack();
            bc.AddStack();
            bc.AddStack();

            Unit unit = MakeUnitWithSizeXY(5.0, 8.0);

            bc.SetUnit(1, 1, unit);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.RemoveUnit(-1, 1); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.RemoveUnit(4, 1); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.RemoveUnit(1, -1); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.RemoveUnit(1, 4); }
            );

            Assert.Equal(
                new[]
                {
                    null, null, null,
                    null, unit, null
                },
                bc.Units
            );
            Assert.Equal(new[] { unit }, building.Children);

            Assert.Equal(building, unit.Parent);
        }

        [Fact]
        public void TestClearBuilding()
        {
            Scene.Scene scene = new Scene.Scene();

            SceneObject building = new SceneObject();
            BuildingComponent bc = new BuildingComponent();
            building.AddComponent(bc);

            bc.AddFloor(4.0);
            bc.AddFloor(5.0);
            bc.AddStack();
            bc.AddStack();

            Unit unit0 = MakeUnitWithSizeXY(5.0, 8.0);
            Unit unit1 = MakeUnitWithSizeXY(5.0, 8.0);
            Unit unit2 = MakeUnitWithSizeXY(5.0, 8.0);
            Unit unit3 = MakeUnitWithSizeXY(5.0, 8.0);

            scene.AddSceneObject(building);
            scene.AddSceneObject(unit0);
            scene.AddSceneObject(unit1);
            scene.AddSceneObject(unit2);
            scene.AddSceneObject(unit3);

            bc.SetUnit(0, 0, unit0);
            bc.SetUnit(0, 1, unit1);
            bc.SetUnit(1, 0, unit2);
            bc.SetUnit(1, 1, unit3);

            bc.ClearBuilding();

            Assert.Equal(0, bc.NumOfFloors);
            Assert.Equal(0, bc.NumOfUnitsPerFloor);
            Assert.Empty(bc.Units);
            Assert.Empty(building.Children);
            Assert.Equal(new[] { building }, scene.SceneObjects);

            Assert.Null(unit0.Scene);
            Assert.Null(unit0.Parent);
            Assert.Null(unit1.Scene);
            Assert.Null(unit1.Parent);
            Assert.Null(unit2.Scene);
            Assert.Null(unit2.Parent);
            Assert.Null(unit3.Scene);
            Assert.Null(unit3.Parent);
        }

        [Fact]
        public void TestInsertEntrance()
        {
            BuildingComponent bc = new BuildingComponent();

            BuildingEntrance be0 = new BuildingEntrance();
            BuildingEntrance be1 = new BuildingEntrance();

            bc.AddEntrance(be1);
            bc.InsertEntrance(0, be0);

            Assert.Equal(new[] { be0, be1 }, bc.Entrances);
        }

        [Fact]
        public void TestInsertEntrance_NullEntrance()
        {
            BuildingComponent bc = new BuildingComponent();

            BuildingEntrance be0 = new BuildingEntrance();
            BuildingEntrance be1 = new BuildingEntrance();

            bc.AddEntrance(be1);
            bc.InsertEntrance(0, be0);

            Assert.Throws<ArgumentNullException>(
                () => { bc.InsertEntrance(1, null); }
            );

            Assert.Equal(new[] { be0, be1 }, bc.Entrances);
        }

        [Fact]
        public void TestInsertEntrance_InvalidIndex()
        {
            BuildingComponent bc = new BuildingComponent();

            BuildingEntrance be0 = new BuildingEntrance();
            BuildingEntrance be1 = new BuildingEntrance();

            bc.AddEntrance(be1);
            bc.InsertEntrance(0, be0);

            BuildingEntrance be2 = new BuildingEntrance();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.InsertEntrance(-1, be2); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.InsertEntrance(10, be2); }
            );

            Assert.Equal(new[] { be0, be1 }, bc.Entrances);
        }

        [Fact]
        public void TestRemoveEntrance()
        {
            BuildingComponent bc = new BuildingComponent();

            BuildingEntrance be0 = new BuildingEntrance();
            BuildingEntrance be1 = new BuildingEntrance();

            bc.AddEntrance(be1);
            bc.InsertEntrance(0, be0);

            bc.RemoveEntrance(0);

            Assert.Equal(new[] { be1 }, bc.Entrances);
        }

        [Fact]
        public void TestRemoveEntrance_InvalidIndex()
        {
            BuildingComponent bc = new BuildingComponent();

            BuildingEntrance be0 = new BuildingEntrance();
            BuildingEntrance be1 = new BuildingEntrance();

            bc.AddEntrance(be1);
            bc.InsertEntrance(0, be0);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.RemoveEntrance(-1); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { bc.RemoveEntrance(2); }
            );

            Assert.Equal(new[] { be0, be1 }, bc.Entrances);
        }

        [Fact]
        public void TestClearEntrances()
        {
            BuildingComponent bc = new BuildingComponent();

            BuildingEntrance be0 = new BuildingEntrance();
            BuildingEntrance be1 = new BuildingEntrance();

            bc.AddEntrance(be1);
            bc.InsertEntrance(0, be0);

            bc.ClearEntrances();

            Assert.Empty(bc.Entrances);
        }
    }
}
