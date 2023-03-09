using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that describes a building.
    /// </summary>
    [Serializable]
    public class BuildingComponent : Component
    {
        #region Constructors

        public BuildingComponent() : base()
        { }

        protected BuildingComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _floorHeights.AddRange(
                reader.GetValues<double>(nameof(FloorHeights))
            );

            NumOfFloors = reader.GetValue<int>(nameof(NumOfFloors));

            NumOfUnitsPerFloor =
                reader.GetValue<int>(nameof(NumOfUnitsPerFloor));

            var units = reader.GetValues<List<Unit>>(nameof(Units));
            _units.AddRange(units);

            var staircases =
                reader.GetValues<List<Staircase>>(nameof(Staircases));
            _staircases.AddRange(staircases);

            var corridors =
                reader.GetValues<List<Corridor>>(nameof(Corridors));
            _corridors.AddRange(corridors);

            var balconies =
                reader.GetValues<List<Balcony>>(nameof(Balconies));
            _balconies.AddRange(balconies);

            var roofs = reader.GetValues<List<Roof>>(nameof(Roofs));
            _roofs.AddRange(roofs);

            var entrances =
                reader.GetValues<BuildingEntrance>(nameof(Entrances));
            _entrances.AddRange(entrances);
        }

        #endregion

        #region Methods

        // Size

        public void InsertFloor(int floor, double floorHeight)
        {
            if (floor > NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            if (floorHeight <= 0.0)
            {
                throw new ArgumentException(
                    $"{nameof(floorHeight)} must be positive",
                    nameof(floorHeight)
                );
            }

            _floorHeights.Insert(floor, floorHeight);

            // Update unit collection

            List<Unit> floorUnits =
                Enumerable.Repeat<Unit>(null, NumOfUnitsPerFloor)
                .ToList();

            _units.Insert(floor, floorUnits);

            // Update staircase collection

            List<Staircase> floorStaircases =
                Enumerable.Repeat<Staircase>(null, NumOfUnitsPerFloor)
                .ToList();

            _staircases.Insert(floor, floorStaircases);

            // Update corridor collection

            List<Corridor> floorCorridors =
                Enumerable.Repeat<Corridor>(null, NumOfUnitsPerFloor)
                .ToList();

            _corridors.Insert(floor, floorCorridors);

            // Update balcony collection

            List<Balcony> floorBalconies =
                Enumerable.Repeat<Balcony>(null, NumOfUnitsPerFloor)
                .ToList();

            _balconies.Insert(floor, floorBalconies);

            // Update roof collection

            List<Roof> floorRoofs =
                Enumerable.Repeat<Roof>(null, NumOfUnitsPerFloor)
                .ToList();

            _roofs.Insert(floor, floorRoofs);

            ++NumOfFloors;
        }

        public void AddFloor(double floorHeight)
        {
            InsertFloor(NumOfFloors, floorHeight);
        }

        public void RemoveFloor(int floor)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            for (int stack = 0; stack < NumOfUnitsPerFloor; ++stack)
            {
                RemoveUnit(floor, stack);
                RemoveStaircase(floor, stack);
                RemoveCorridor(floor, stack);
                RemoveBalcony(floor, stack);
                RemoveRoof(floor, stack);
            }

            _units.RemoveAt(floor);

            _floorHeights.RemoveAt(floor);

            _staircases.RemoveAt(floor);

            _corridors.RemoveAt(floor);

            _balconies.RemoveAt(floor);

            _roofs.RemoveAt(floor);

            --NumOfFloors;
        }

        public void InsertStack(int stack)
        {
            if (stack > NumOfUnitsPerFloor || stack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stack));
            }

            // Update unit collection

            foreach (List<Unit> floorUnits in _units)
            {
                floorUnits.Insert(stack, null);
            }

            // Update staircase collection

            foreach (List<Staircase> floorStaircases in _staircases)
            {
                floorStaircases.Insert(stack, null);
            }

            // Update corridor collection

            foreach (List<Corridor> floorCorridors in _corridors)
            {
                floorCorridors.Insert(stack, null);
            }

            // Update balcony collection

            foreach (List<Balcony> floorBalconies in _balconies)
            {
                floorBalconies.Insert(stack, null);
            }

            // Update roof collection

            foreach (List<Roof> floorRoofs in _roofs)
            {
                floorRoofs.Insert(stack, null);
            }

            ++NumOfUnitsPerFloor;
        }

        public void AddStack()
        {
            InsertStack(NumOfUnitsPerFloor);
        }

        public void RemoveStack(int stack)
        {
            if (stack >= NumOfUnitsPerFloor || stack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stack));
            }

            for (int floor = 0; floor < NumOfFloors; ++floor)
            {
                RemoveUnit(floor, stack);
                RemoveStaircase(floor, stack);
                RemoveCorridor(floor, stack);
                RemoveBalcony(floor, stack);
                RemoveRoof(floor, stack);
            }

            // Update unit collection

            foreach (List<Unit> floorUnits in _units)
            {
                floorUnits.RemoveAt(stack);
            }

            // Update staircase collection

            foreach (List<Staircase> floorStaircases in _staircases)
            {
                floorStaircases.RemoveAt(stack);
            }

            // Update corridor collection

            foreach (List<Corridor> floorCorridors in _corridors)
            {
                floorCorridors.RemoveAt(stack);
            }

            // Update balcony collection

            foreach (List<Balcony> floorBalconies in _balconies)
            {
                floorBalconies.RemoveAt(stack);
            }

            // Update roof collection

            foreach (List<Roof> floorRoofs in _roofs)
            {
                floorRoofs.RemoveAt(stack);
            }

            --NumOfUnitsPerFloor;
        }

        public void ClearBuilding()
        {
            for (int floor = NumOfFloors - 1; floor >= 0; --floor)
            {
                RemoveFloor(floor);
            }

            for (int stack = NumOfUnitsPerFloor - 1; stack >= 0; --stack)
            {
                RemoveStack(stack);
            }
        }

        // Units

        public Unit GetUnit(int floor, int stack)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            if (stack >= NumOfUnitsPerFloor || stack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stack));
            }

            return _units[floor][stack];
        }

        public IReadOnlyList<Unit> GetFloorUnits(int floor)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            return _units[floor];
        }

        public IReadOnlyList<Unit> GetStackUnits(int stack)
        {
            if (stack >= NumOfUnitsPerFloor || stack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stack));
            }

            List<Unit> stackUnits =
                _units.Select(floorUnits => floorUnits[stack]).ToList();

            return stackUnits;
        }

        public void SetUnit(int floor, int stack, Unit unit)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            if (stack >= NumOfUnitsPerFloor || stack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stack));
            }

            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (GetUnit(floor, stack) != null)
            {
                throw new InvalidOperationException(
                    $"Unit exists at Floor ${floor}, Stack ${stack}"
                );
            }

            SceneObject.AddChild(unit);

            _units[floor][stack] = unit;

            unit.UnitComponent.RoomHeight = FloorHeights[floor];
        }

        public void RemoveUnit(int floor, int stack)
        {
            Unit unit = GetUnit(floor, stack);

            if (unit == null) { return; }

            _units[floor][stack] = null;

            SceneObject.RemoveChild(unit);

            unit.Scene?.RemoveSceneObject(unit);
        }

        public void UpdateUnitTransforms(int startFloor, int startStack)
        {
            for (int floor = startFloor; floor < NumOfFloors; ++floor)
            {
                for (
                    int stack = startStack;
                    stack < NumOfUnitsPerFloor; ++stack
                )
                {
                    Unit unit = GetUnit(floor, stack);

                    if (unit == null) { continue; }

                    TrsTransform3D unitTf = GetUnitTransform(floor, stack);

                    unit.TransformComponent.Transform = unitTf;
                }
            }
        }

        public TrsTransform3D GetUnitTransform(int floor, int stack)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            if (stack >= NumOfUnitsPerFloor || stack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stack));
            }

            double localX = 0.0;

            for (int stack0 = 0; stack0 < stack; ++stack0)
            {
                Unit unit = GetUnit(floor, stack0);

                if (unit == null) { continue; }

                localX += unit.UnitComponent.GetBBox().SizeX;
            }

            double localY = 0.0;

            double localZ = 0.0;

            for (int floor0 = 0; floor0 < floor; ++floor0)
            {
                localZ += FloorHeights[floor0];
            }

            TrsTransform3D unitTf = new TrsTransform3D
            {
                Tx = localX,
                Ty = localY,
                Tz = localZ
            };

            return unitTf;
        }

        // Staircases

        public Staircase GetStaircase(int floor, int stack)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            if (stack >= NumOfUnitsPerFloor || stack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stack));
            }

            return _staircases[floor][stack];
        }

        public IReadOnlyList<Staircase> GetFloorStaircases(int floor)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            return _staircases[floor];
        }

        public void SetStaircase(int floor, int stack, Staircase staircase)
        {
            if (staircase == null)
            {
                throw new ArgumentNullException(nameof(staircase));
            }

            var unit = GetUnit(floor, stack);

            if (unit == null)
            {
                throw new InvalidOperationException(
                    $"No unit exists at Floor ${floor}, Stack ${stack}"
                );
            }

            if (GetStaircase(floor, stack) != null)
            {
                throw new InvalidOperationException(
                    $"Staircase exists at Floor ${floor}, Stack ${stack}"
                );
            }

            SceneObject.AddChild(staircase);

            _staircases[floor][stack] = staircase;
        }

        public void RemoveStaircase(int floor, int stack)
        {
            var staircase = GetStaircase(floor, stack);

            if (staircase == null)
            {
                return;
            }

            _staircases[floor][stack] = null;

            SceneObject.RemoveChild(staircase);

            staircase.Scene?.RemoveSceneObject(staircase);
        }

        // Corridors

        public Corridor GetCorridor(int floor, int stack)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            if (stack >= NumOfUnitsPerFloor || stack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stack));
            }

            return _corridors[floor][stack];
        }

        public IReadOnlyList<Corridor> GetFloorCorridors(int floor)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            return _corridors[floor];
        }

        public void SetCorridor(int floor, int stack, Corridor corridor)
        {
            if (corridor == null)
            {
                throw new ArgumentNullException(nameof(corridor));
            }

            var unit = GetUnit(floor, stack);

            if (unit == null)
            {
                throw new InvalidOperationException(
                    $"No unit exists at Floor ${floor}, Stack ${stack}"
                );
            }

            if (GetCorridor(floor, stack) != null)
            {
                throw new InvalidOperationException(
                    $"Corridor exists at Floor ${floor}, Stack ${stack}"
                );
            }

            SceneObject.AddChild(corridor);

            _corridors[floor][stack] = corridor;
        }

        public void RemoveCorridor(int floor, int stack)
        {
            var corridor = GetCorridor(floor, stack);

            if (corridor == null)
            {
                return;
            }

            _corridors[floor][stack] = null;

            SceneObject.RemoveChild(corridor);

            corridor.Scene?.RemoveSceneObject(corridor);
        }

        // Balconies

        public Balcony GetBalcony(int floor, int stack)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            if (stack >= NumOfUnitsPerFloor || stack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stack));
            }

            return _balconies[floor][stack];
        }

        public IReadOnlyList<Balcony> GetFloorBalconies(int floor)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            return _balconies[floor];
        }

        public void SetBalcony(int floor, int stack, Balcony balcony)
        {
            if (balcony == null)
            {
                throw new ArgumentNullException(nameof(balcony));
            }

            var unit = GetUnit(floor, stack);

            if (unit == null)
            {
                throw new InvalidOperationException(
                    $"No unit exists at Floor ${floor}, Stack ${stack}"
                );
            }

            if (GetBalcony(floor, stack) != null)
            {
                throw new InvalidOperationException(
                    $"Balcony exists at Floor ${floor}, Stack ${stack}"
                );
            }

            SceneObject.AddChild(balcony);

            _balconies[floor][stack] = balcony;
        }

        public void RemoveBalcony(int floor, int stack)
        {
            var balcony = GetBalcony(floor, stack);

            if (balcony == null)
            {
                return;
            }

            _balconies[floor][stack] = null;

            SceneObject.RemoveChild(balcony);

            balcony.Scene?.RemoveSceneObject(balcony);
        }

        // Roofs

        public Roof GetRoof(int floor, int stack)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            if (stack >= NumOfUnitsPerFloor || stack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stack));
            }

            return _roofs[floor][stack];
        }

        public IReadOnlyList<Roof> GetFloorRoofs(int floor)
        {
            if (floor >= NumOfFloors || floor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }

            return _roofs[floor];
        }

        public void SetRoof(int floor, int stack, Roof roof)
        {
            if (roof == null)
            {
                throw new ArgumentNullException(nameof(roof));
            }

            var unit = GetUnit(floor, stack);

            if (unit == null)
            {
               
                throw new InvalidOperationException(
                    $"No unit exists at Floor ${floor}, Stack ${stack}"
                );
            }

            if (GetRoof(floor, stack) != null)
            {
                throw new InvalidOperationException(
                    $"Roof exists at Floor ${floor}, Stack ${stack}"
                );
            }

            SceneObject.AddChild(roof);

            _roofs[floor][stack] = roof;
        }

        public void RemoveRoof(int floor, int stack)
        {
            var roof = GetRoof(floor, stack);

            if (roof == null)
            {
                return;
            }

            _roofs[floor][stack] = null;

            SceneObject.RemoveChild(roof);

            roof.Scene?.RemoveSceneObject(roof);
        }

        public void ClearRoofs()
        {
            for (int floor = 0; floor < NumOfFloors; ++floor)
            {
                for (int stack = 0; stack < NumOfUnitsPerFloor; ++stack)
                {
                    RemoveRoof(floor, stack);
                }
            }
        }

        // Entrances

        public void InsertEntrance(
            int entranceIndex, BuildingEntrance entrance
        )
        {
            if (entrance == null)
            {
                throw new ArgumentNullException(nameof(entrance));
            }

            _entrances.Insert(entranceIndex, entrance);
        }

        public void AddEntrance(BuildingEntrance entrance)
        {
            InsertEntrance(_entrances.Count, entrance);
        }

        public void RemoveEntrance(int entranceIndex)
        {
            _entrances.RemoveAt(entranceIndex);
        }

        public void ClearEntrances()
        {
            _entrances.Clear();
        }

        // Collision

        public void UpdateCollisionBody2D()
        {
            var cbc = SceneObject?.GetComponent<CollisionBody2DComponent>();

            if (cbc == null) { return; }

            cbc.ClearCollisionPolygons();

            if (NumOfFloors == 0) { return; }

            var collisionPolygons = new List<Polygon>();

            // Add unit collision polygons

            foreach (var unit in GetFloorUnits(0))
            {
                if (unit == null)
                {
                    continue;
                }

                var uc = unit.UnitComponent;
                var unitTf = unit.TransformComponent.Transform;

                var unitCollisionPolygons =
                    uc.RoomPlans.Select(
                        plan =>
                        {
                            var planCollisionPolygon = new Polygon(plan);
                            planCollisionPolygon.OffsetEdges(-cbc.Epsilon);
                            planCollisionPolygon.Transform(unitTf);

                            return planCollisionPolygon;
                        }
                    ).ToList();

                collisionPolygons.AddRange(unitCollisionPolygons);
            }

            // Add staircase collision polygons

            for (int stack = 0; stack < NumOfUnitsPerFloor; ++stack)
            {
                var staircase = GetStaircase(0, stack);

                if (staircase == null)
                {
                    continue;
                }

                var sc = staircase.StaircaseComponent;
                var staircaseTf = staircase.TransformComponent.Transform;

                var staircaseCp = new Polygon(sc.GetPlan());
                staircaseCp.OffsetEdges(-cbc.Epsilon);
                staircaseCp.Transform(staircaseTf);

                collisionPolygons.Add(staircaseCp);
            }

            // Add corridor collision polygons

            for (int stack = 0; stack < NumOfUnitsPerFloor; ++stack)
            {
                var corridor = GetCorridor(0, stack);

                if (corridor == null)
                {
                    continue;
                }

                var cc = corridor.CorridorComponent;
                var corridorTf = corridor.TransformComponent.Transform;

                var corridorCp = new Polygon(cc.Plan);
                corridorCp.OffsetEdges(-cbc.Epsilon);
                corridorCp.Transform(corridorTf);

                collisionPolygons.Add(corridorCp);
            }

            // Add balcony collision polygons

            for (int stack = 0; stack < NumOfUnitsPerFloor; ++stack)
            {
                var balcony = GetBalcony(0, stack);

                if (balcony == null)
                {
                    continue;
                }

                var bc = balcony.BalconyComponent;
                var balconyTf = balcony.TransformComponent.Transform;

                var balconyCp = new Polygon(bc.GetPlan());
                balconyCp.OffsetEdges(-cbc.Epsilon);
                balconyCp.Transform(balconyTf);

                collisionPolygons.Add(balconyCp);
            }

            // Add roof collision polygons

            for (int stack = 0; stack < NumOfUnitsPerFloor; ++stack)
            {
                var roof = GetRoof(NumOfFloors - 1, stack);

                if (roof == null)
                {
                    continue;
                }

                var rc = roof.RoofComponent;
                var roofTf = roof.TransformComponent.Transform;

                var roofCp = new Polygon(rc.GetPlan());
                roofCp.OffsetEdges(-cbc.Epsilon);
                roofCp.Transform(roofTf);

                collisionPolygons.Add(roofCp);
            }

            foreach (var collisionPolygon in collisionPolygons)
            {
                cbc.AddCollisionPolygon(collisionPolygon);
            }
        }

        // Serialization

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Concat(Units)
            .Concat(Staircases)
            .Concat(Corridors)
            .Concat(Balconies)
            .Concat(Roofs);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValues(nameof(FloorHeights), _floorHeights);

            writer.AddValue(nameof(NumOfFloors), NumOfFloors);

            writer.AddValue(nameof(NumOfUnitsPerFloor), NumOfUnitsPerFloor);

            writer.AddValues(nameof(Units), _units);

            writer.AddValues(nameof(Staircases), _staircases);

            writer.AddValues(nameof(Corridors), _corridors);

            writer.AddValues(nameof(Balconies), _balconies);

            writer.AddValues(nameof(Roofs), _roofs);

            writer.AddValues(nameof(Entrances), _entrances);
        }

        #endregion

        #region Properties

        public IReadOnlyList<double> FloorHeights
        {
            get
            {
                return _floorHeights;
            }
        }

        public double TotalFloorHeight
        {
            get
            {
                return FloorHeights.Sum();
            }
        }

        public int NumOfFloors { get; private set; } = 0;

        public int NumOfUnitsPerFloor { get; private set; } = 0;

        public int NumOfUnits
        {
            get { return NumOfUnitsPerFloor * NumOfFloors; }
        }

        public IReadOnlyList<Unit> Units
        {
            get
            {
                return _units.SelectMany(floorUnits => floorUnits).ToList();
            }
        }

        public IReadOnlyList<Staircase> Staircases =>
            _staircases.SelectMany(staircases => staircases).ToList();

        public IReadOnlyList<Corridor> Corridors =>
            _corridors.SelectMany(corridors => corridors).ToList();

        public IReadOnlyList<Balcony> Balconies =>
            _balconies.SelectMany(balconies => balconies).ToList();

        public IReadOnlyList<Roof> Roofs =>
            _roofs.SelectMany(roofs => roofs).ToList();

        public IReadOnlyList<BuildingEntrance> Entrances => _entrances;

        #endregion

        #region Member variables

        private readonly List<double> _floorHeights = new List<double>();

        private readonly List<List<Unit>> _units = new List<List<Unit>>();

        private readonly List<List<Staircase>> _staircases =
            new List<List<Staircase>>();

        private readonly List<List<Corridor>> _corridors =
            new List<List<Corridor>>();

        private readonly List<List<Balcony>> _balconies =
            new List<List<Balcony>>();

        private readonly List<List<Roof>> _roofs =
            new List<List<Roof>>();

        private readonly List<BuildingEntrance> _entrances =
            new List<BuildingEntrance>();

        #endregion
    }
}
