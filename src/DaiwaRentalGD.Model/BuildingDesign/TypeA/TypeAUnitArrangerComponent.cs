using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.Finance;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeA
{
    /// <summary>
    /// A component that arranges Type A units in a building.
    /// </summary>
    [Serializable]
    public class TypeAUnitArrangerComponent : UnitArrangerComponent
    {
        #region Constructors

        public TypeAUnitArrangerComponent() : base()
        {
            base.MinNumOfFloors = 2;
            base.MaxNumOfFloors = 3;
            base.MinNumOfUnitsPerFloor = 2;
        }

        protected TypeAUnitArrangerComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _floorHeight = reader.GetValue<double>(nameof(FloorHeight));
            _entranceType =
                reader.GetValue<TypeAUnitEntranceType>(nameof(EntranceType));

            UpdateEntries();
        }

        #endregion

        #region Methods

        #region Unit catalog queries

        public override void SetNormalizedEntryIndex(
            int entryIndexIndex, double normalizedEntryIndex
        )
        {
            base.SetNormalizedEntryIndex(
                entryIndexIndex, normalizedEntryIndex
            );

            if (entryIndexIndex % 2 == 0)
            {
                if (entryIndexIndex + 1 < NormalizedEntryIndices.Count)
                {
                    base.SetNormalizedEntryIndex(
                        entryIndexIndex + 1, normalizedEntryIndex
                    );
                }
            }
            else
            {
                base.SetNormalizedEntryIndex(
                    entryIndexIndex - 1, normalizedEntryIndex
                );
            }
        }

        public IReadOnlyList<TypeAUnitComponent> GetEntries(
            HouseholdType householdType,
            TypeAUnitEntranceType entranceType,
            TypeAUnitPositionType positionType,
            double previousSizeYInP
        )
        {
            var entries = TypeAEntries
                .Where(
                    entry =>
                    //entry.HouseholdType == householdType &&
                    entry.EntranceType == entranceType &&
                    entry.PositionType == positionType &&
                    IsSizeYInPValid(
                        entry.EntryName.SizeYInP,
                        previousSizeYInP
                    )
                ).ToList();           
            return entries;
        }

        private bool IsSizeYInPValid(double sizeYInP, double previousSizeYInP)
        {
            if (previousSizeYInP == InvalidSizeYInP)
            {
                return true;
            }

            double sizeYInPDiff = sizeYInP - previousSizeYInP;

            bool isSizeYInPValid = ValidSizeYInPDiffs.Contains(sizeYInPDiff);

            return isSizeYInPValid;
        }

        public TypeAUnitComponent CreateFromEntry(
            HouseholdType householdType,
            TypeAUnitEntranceType entranceType,
            TypeAUnitPositionType positionType,
            double normalizedIndex,
            double previousSizeYInP
        )
        {
            var entries = GetEntries(
                householdType, entranceType, positionType,
                previousSizeYInP
            );            

            int entryIndex = MathUtils.MapDoubleToInt(
                normalizedIndex, 0.0, 1.0,
                0, entries.Count - 1
            );

            TypeAUnitComponent entry = entries[entryIndex];

            TypeAUnitComponent uc = new TypeAUnitComponent(entry);
            return uc;
        }

        public TypeAUnitComponent CreateBasicUnitFromEntry(
            double normalizedIndex, bool isMirrored,
            double previousSizeYInP
        )
        {
            TypeAUnitComponent uc = CreateFromEntry(
                HouseholdType,
                EntranceType, TypeAUnitPositionType.Basic,
                normalizedIndex,
                previousSizeYInP
            );

            if (isMirrored)
            {
                uc.Mirror();
            }

            return uc;
        }

        public TypeAUnitComponent CreateEndUnitFromEntry(
            double normalizedIndex, double previousSizeYInP
        )
        {
            TypeAUnitComponent uc = CreateFromEntry(
                HouseholdType,
                EntranceType, TypeAUnitPositionType.End,
                normalizedIndex,
                previousSizeYInP
            );

            uc.Mirror();

            return uc;
        }

        public void UpdateEntries()
        {
            _typeAEntries.Clear();

            if (UnitCatalog == null) { return; }
            var typeAEntries =
                UnitCatalog.UnitCatalogComponent.Entries
                .Select(entry => entry as TypeAUnitComponent)
                .Where(
                    entry =>
                    entry != null &&
                    entry.EntryName.MainType == TypeAUnitComponent.MainType
                );

            _typeAEntries.AddRange(typeAEntries);
        }

        #endregion

        #region Unit arrangement

        public TypeAUnitComponent CreateUnit(
            int stack, int numOfUnitsPerFloor
        )
        {
            bool isEndUnit =
                numOfUnitsPerFloor % 2 == 1 &&
                stack == numOfUnitsPerFloor - 1;

            bool isLeftUnit =
                stack % 2 == 0 &&
                stack < numOfUnitsPerFloor - 1;
            bool isRightUnit = stack % 2 == 1;

            double normalizedEntryIndex;

            if (isLeftUnit || isEndUnit)
            {
                normalizedEntryIndex = NormalizedEntryIndices[stack];
            }
            else
            {
                normalizedEntryIndex = NormalizedEntryIndices[stack - 1];
            }

            TypeAUnitComponent uc;

            if (isEndUnit)
            {
                var previousUc =
                    Building.BuildingComponent.GetUnit(0, stack - 1)
                    .UnitComponent as TypeAUnitComponent;

                double previousSizeYInP = previousUc.EntryName.SizeYInP;

                uc = CreateEndUnitFromEntry(
                    normalizedEntryIndex, previousSizeYInP
                );
            }
            else
            {
                bool isMirrored = isRightUnit;

                int previousPairUcStack = stack / 2 * 2 - 2;

                double previousSizeYInP;

                if (previousPairUcStack >= 0)
                {
                    var previousPairUc =
                        Building.BuildingComponent.GetUnit(
                            0, previousPairUcStack
                        ).UnitComponent as TypeAUnitComponent;

                    previousSizeYInP = previousPairUc.EntryName.SizeYInP;
                }
                else
                {
                    previousSizeYInP = InvalidSizeYInP;
                }

                uc = CreateBasicUnitFromEntry(
                    normalizedEntryIndex, isMirrored,
                    previousSizeYInP
                );
            }

            return uc;
        }

        public override void ArrangeUnits(Building building)
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

            for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
            {
                for (int floor = 0; floor < bc.NumOfFloors; ++floor)
                {
                    Unit unit = new Unit
                    {
                        UnitComponent = CreateUnit(
                            stack, bc.NumOfUnitsPerFloor
                        )
                    };

                    building.Scene?.AddSceneObject(unit);
                    bc.SetUnit(floor, stack, unit);
                }
            }

            bc.UpdateUnitTransforms(0, 0);
        }

        public Staircase CreateStaircase()
        {
            var staircase = new Staircase();

            return staircase;
        }

        public void ArrangeStaircases(Building building)
        {
            var bc = building.BuildingComponent;

            for (int floor = 0; floor < bc.NumOfFloors; ++floor)
            {
                for (int stack = 0; stack < bc.NumOfUnitsPerFloor; stack += 2)
                {
                    var uc =
                        bc.GetUnit(floor, stack)?.UnitComponent
                        as TypeAUnitComponent;

                    if (uc == null)
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

        public void ArrangeBalconies(Building building)
        {
            var bc = building.BuildingComponent;

            for (int floor = 0; floor < bc.NumOfFloors; ++floor)
            {
                for (int stack = 0; stack < bc.NumOfUnitsPerFloor; stack += 1)
                {
                    var uc =
                        bc.GetUnit(floor, stack)?.UnitComponent
                        as TypeAUnitComponent;

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
            writer.AddValue(nameof(EntranceType), _entranceType);
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

        public TypeAUnitEntranceType EntranceType
        {
            get => _entranceType;
            set
            {
                _entranceType = value;
                NotifyPropertyChanged();
            }
        }
        public TypeAUnitRoofType RoofType
        {
            get => _roofType;
            set
            {
                _roofType = value;
                NotifyPropertyChanged();
            }
        }
        private TypeAUnitRoofType _roofType;
        public IReadOnlyList<TypeAUnitComponent> TypeAEntries =>
            _typeAEntries;

        public HashSet<double> ValidSizeYInPDiffs { get; } =
            new HashSet<double> { -2.0, -1.0, 0.0, 1.0, 2.0 };

        #endregion

        #region Member variables

        private double _floorHeight = UnitComponent.DefaultRoomHeight;

        private TypeAUnitEntranceType _entranceType;

        private readonly List<TypeAUnitComponent> _typeAEntries =
            new List<TypeAUnitComponent>();

        #endregion

        #region Constants

        public const double InvalidSizeYInP = -1;

        #endregion
    }
}
