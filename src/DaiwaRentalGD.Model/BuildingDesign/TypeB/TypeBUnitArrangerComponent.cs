using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.Finance;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeB
{
    /// <summary>
    /// A component that arranges Type B units in a building.
    /// </summary>
    [Serializable]
    public class TypeBUnitArrangerComponent : UnitArrangerComponent
    {
        #region Constructors

        public TypeBUnitArrangerComponent() : base()
        {
            base.MinNumOfFloors = 2;
            base.MaxNumOfFloors = 3;
            base.MinNumOfUnitsPerFloor = 2;
        }

        protected TypeBUnitArrangerComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _floorHeight = reader.GetValue<double>(nameof(FloorHeight));
            _stairType = reader.GetValue<TypeBStairType>(nameof(StairType));
            _corridorWidth0 = reader.GetValue<double>(nameof(CorridorWidth0));
            _corridorWidth1 = reader.GetValue<double>(nameof(CorridorWidth1));

            UpdateEntries();
        }

        #endregion

        #region Methods

        #region Unit catalog queries

        public IReadOnlyList<TypeBUnitComponent> GetEntries(
            HouseholdType householdType,
            TypeBUnitLayoutType layoutType,
            double previousSizeYInP
        )
        {
            var entries = TypeBEntries
                .Where(
                    entry =>
                    //entry.HouseholdType == householdType &&
                    entry.LayoutType == layoutType &&
                    IsSizeYInPValid(
                        entry.EntryName.SizeYInP,
                        layoutType, previousSizeYInP
                    )
                ).ToList();

            return entries;
        }

        private bool IsSizeYInPValid(
            double sizeYInP,
            TypeBUnitLayoutType layoutType, double previousSizeYInP
        )
        {
            if (previousSizeYInP == InvalidSizeYInP)
            {
                return true;
            }

            var validSizeYInPDiffs =
                layoutType == TypeBUnitLayoutType.Basic ?
                ValidBasicSizeYInPDiffs : ValidStairSizeYInPDiffs;

            double sizeYInPDiff = sizeYInP - previousSizeYInP;

            bool isSizeYInPValid = validSizeYInPDiffs.Contains(sizeYInPDiff);

            return isSizeYInPValid;
        }

        public TypeBUnitComponent CreateFromEntry(
            HouseholdType householdType,
            TypeBUnitLayoutType layoutType,
            double normalizedIndex,
            double previousSizeYInP
        )
        {
            var entries = GetEntries(
                householdType, layoutType, previousSizeYInP
            );

            int entryIndex = MathUtils.MapDoubleToInt(
                normalizedIndex, 0.0, 1.0,
                0, entries.Count - 1
            );

            TypeBUnitComponent entry = entries[entryIndex];

            TypeBUnitComponent uc = new TypeBUnitComponent(entry);
            return uc;
        }

        public TypeBUnitComponent CreateBasicFromEntry(
            double normalizedIndex, double previousSizeYInP
        )
        {
            return CreateFromEntry(
                HouseholdType, TypeBUnitLayoutType.Basic, normalizedIndex,
                previousSizeYInP
            );
        }

        public TypeBUnitComponent CreateStairFromEntry(
            double normalizedIndex, double previousSizeYInP
        )
        {
            return CreateFromEntry(
                HouseholdType, TypeBUnitLayoutType.Stair, normalizedIndex,
                previousSizeYInP
            );
        }

        public void UpdateEntries()
        {
            _typeBEntries.Clear();

            if (UnitCatalog == null) { return; }

            var typeBEntries =
                UnitCatalog.UnitCatalogComponent.Entries
                .Select(entry => entry as TypeBUnitComponent)
                .Where(
                    entry =>
                    entry != null &&
                    entry.EntryName.MainType == TypeBUnitComponent.MainType
                );

            _typeBEntries.AddRange(typeBEntries);
        }

        #endregion

        #region Unit arrangement

        public override void ArrangeUnits(Building building)
        {
            switch (StairType)
            {
                case TypeBStairType.NorthUShaped:

                    ArrangeUnitsNorthUShaped(building);
                    break;

                default:

                    ResetBuilding(building);
                    break;
            }
        }

        private void ResetBuilding(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            BuildingComponent bc = building.BuildingComponent;

            bc.ClearBuilding();

            for (int floor = 0; floor < NumOfFloors; ++floor)
            {
                bc.AddFloor(FloorHeight);
            }

            for (int stack = 0; stack < NumOfUnitsPerFloor; ++stack)
            {
                bc.AddStack();
            }
        }

        private void ArrangeUnitsNorthUShaped(Building building)
        {
            ResetBuilding(building);

            var bc = building.BuildingComponent;

            double previousSizeYInP = InvalidSizeYInP;

            for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
            {
                double normalizedIndex = NormalizedEntryIndices[stack];

                for (int floor = 0; floor < bc.NumOfFloors; ++floor)
                {
                    TypeBUnitComponent uc;
                    if (stack < bc.NumOfUnitsPerFloor - 1)
                    {
                        uc = CreateBasicFromEntry(
                            normalizedIndex, previousSizeYInP
                        );
                    }
                    else
                    {
                        uc = CreateStairFromEntry(
                            normalizedIndex, previousSizeYInP
                        );
                    }

                    Unit unit = new Unit { UnitComponent = uc };

                    building.Scene?.AddSceneObject(unit);
                    bc.SetUnit(floor, stack, unit);
                }

                var currentUc =
                    bc.GetUnit(0, stack).UnitComponent
                    as TypeBUnitComponent;
                previousSizeYInP = currentUc.EntryName.SizeYInP;
            }

            bc.UpdateUnitTransforms(0, 0);

            double maxSizeYInP = double.NegativeInfinity;

            foreach (var unit in bc.GetFloorUnits(0))
            {
                var uc = unit.UnitComponent as TypeBUnitComponent;
                double sizeYInP = uc.EntryName.SizeYInP;

                if (sizeYInP > maxSizeYInP)
                {
                    maxSizeYInP = sizeYInP;
                }
            }

            foreach (var unit in bc.Units)
            {
                var uc = unit.UnitComponent as TypeBUnitComponent;
                double sizeYInP = uc.EntryName.SizeYInP;

                var tc = unit.TransformComponent;
                double tyInP = maxSizeYInP - sizeYInP;
                tc.Transform.Ty = LengthP.PToM(tyInP);
            }
        }

        public Staircase CreateStaircase()
        {
            var staircase = new Staircase();

            var sc = staircase.StaircaseComponent;

            sc.Entrance.EntrancePoint =
                new Point(0.0, sc.Length / 2.0, 0.0);

            sc.Entrance.EntranceDirection =
                new DenseVector(new[] { 1.0, 0.0, 0.0 });

            return staircase;
        }

        public void ArrangeStaircases(Building building)
        {
            var bc = building.BuildingComponent;

            for (int floor = 0; floor < bc.NumOfFloors; ++floor)
            {
                for (int stack = 0; stack < bc.NumOfUnitsPerFloor; stack += 1)
                {
                    var uc =
                        bc.GetUnit(floor, stack)?.UnitComponent
                        as TypeBUnitComponent;

                    if (uc == null)
                    {
                        continue;
                    }

                    if (uc.LayoutType != TypeBUnitLayoutType.Stair)
                    {
                        continue;
                    }

                    var staircase = CreateStaircase();

                    bc.SetStaircase(floor, stack, staircase);

                    staircase.TransformComponent.Transform =
                        uc.GetStaircaseTransform(staircase);
                }
            }
        }

        public Corridor CreateCorridor(Building building, int floor)
        {
            var staircase = CreateStaircase();
            double staircaseWidth = staircase.StaircaseComponent.Width;

            var bc = building.BuildingComponent;

            double corridorLength = 0.0;

            foreach (var unit in bc.GetFloorUnits(floor))
            {
                var uc = unit.UnitComponent;
                var unitBbox = uc.GetBBox();
                var unitWidth = unitBbox.SizeX;

                corridorLength += unitWidth;
            }

            var corridor = new Corridor();
            var cc = corridor.CorridorComponent;

            cc.Plan = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(corridorLength, 0.0, 0.0),
                    new Point(
                        corridorLength,
                        CorridorWidth1,
                        0.0
                    ),
                    new Point(
                        corridorLength - staircaseWidth,
                        CorridorWidth1,
                        0.0
                    ),
                    new Point(
                        corridorLength - staircaseWidth,
                        CorridorWidth0,
                        0.0
                    ),
                    new Point(0.0, CorridorWidth0, 0.0)
                }
            );

            cc.Entrance.EntrancePoint =
                new Point(
                    corridorLength - staircaseWidth / 2.0,
                    CorridorWidth1,
                    0.0
                );
            cc.Entrance.EntranceDirection =
                new DenseVector(new[] { 0.0, -1.0, 0.0 });

            return corridor;
        }

        public void ArrangeCorridors(Building building)
        {
            var bc = building.BuildingComponent;

            for (int floor = 0; floor < bc.NumOfFloors; ++floor)
            {
                var uc =
                    bc.GetUnit(floor, 0).UnitComponent
                    as TypeBUnitComponent;

                var corridor = CreateCorridor(building, floor);

                bc.SetCorridor(floor, 0, corridor);

                corridor.TransformComponent.Transform =
                    uc.GetCorridorTransform(corridor);
            }
        }

        public void ArrangeBalconies(Building building)
        {
            var bc = building.BuildingComponent;

            for (int floor = 0; floor < bc.NumOfFloors; ++floor)
            {
                for (int stack = 0; stack < bc.NumOfUnitsPerFloor; stack += 1)
                {
                    var uc =
                        bc.GetUnit(floor, stack)?.UnitComponent
                        as TypeBUnitComponent;

                    if (uc == null)
                    {
                        continue;
                    }

                    var balcony = new Balcony();

                    bc.SetBalcony(floor, stack, balcony);

                    balcony.BalconyComponent.Length =
                        uc.BalconyLength;

                    balcony.TransformComponent.Transform =
                        uc.GetBalconyTransform(balcony);
                }
            }
        }

        protected override void Update()
        {
            if (Building == null)
            {
                return;
            }

            UpdateEntries();

            ArrangeUnits(Building);

            ArrangeStaircases(Building);

            ArrangeCorridors(Building);

            ArrangeBalconies(Building);
        }

        #endregion

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(FloorHeight), _floorHeight);
            writer.AddValue(nameof(StairType), _stairType);
            writer.AddValue(nameof(CorridorWidth0), _corridorWidth0);
            writer.AddValue(nameof(CorridorWidth1), _corridorWidth1);
        }

        #endregion

        #region Properties

        public override int MinNumOfFloors
        {
            get => base.MinNumOfFloors;
            set { }
        }

        public override int MaxNumOfFloors
        {
            get => base.MaxNumOfFloors;
            set { }
        }

        public override int MinNumOfUnitsPerFloor
        {
            get => base.MinNumOfUnitsPerFloor;
            set { }
        }

        public double FloorHeight
        {
            get => _floorHeight;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(FloorHeight)} must be positive"
                    );
                }

                _floorHeight = value;
                NotifyPropertyChanged();
            }
        }

        public TypeBStairType StairType
        {
            get => _stairType;
            set
            {
                _stairType = value;
                NotifyPropertyChanged();
            }
        }

        public IReadOnlyList<TypeBUnitComponent> TypeBEntries =>
            _typeBEntries;

        public HashSet<double> ValidBasicSizeYInPDiffs { get; } =
            new HashSet<double> { -2.0, -1.0, 0.0, 1.0, 2.0 };

        public HashSet<double> ValidStairSizeYInPDiffs { get; } =
            new HashSet<double> { 0.0, 2.0 };

        public double CorridorWidth0
        {
            get => _corridorWidth0;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(CorridorWidth0)} must be positive"
                    );
                }

                _corridorWidth0 = value;
                NotifyPropertyChanged();
            }
        }

        public double CorridorWidth1
        {
            get => _corridorWidth1;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(CorridorWidth1)} must be positive"
                    );
                }

                _corridorWidth1 = value;
                NotifyPropertyChanged();
            }
        }
        public TypeBUnitRoofType RoofType
        {
            get => _roofType;
            set
            {
                _roofType = value;
                NotifyPropertyChanged();
            }
        }
        private TypeBUnitRoofType _roofType;
        #endregion

        #region Member variables

        private double _floorHeight = UnitComponent.DefaultRoomHeight;

        private TypeBStairType _stairType;

        private readonly List<TypeBUnitComponent> _typeBEntries =
            new List<TypeBUnitComponent>();

        private double _corridorWidth0 = DefaultCorridorWidth0P.M;
        private double _corridorWidth1 = DefaultCorridorWidth1P.M;

        #endregion

        #region Constants

        public const double InvalidSizeYInP = -1;

        public static readonly LengthP DefaultCorridorWidth0P =
            new LengthP(1.625);

        public static readonly LengthP DefaultCorridorWidth1P =
            new LengthP(2.0);

        #endregion
    }
}
