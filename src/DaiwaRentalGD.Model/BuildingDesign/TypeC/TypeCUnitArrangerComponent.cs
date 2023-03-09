using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.Finance;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeC
{
    /// <summary>
    /// A component that arranges Type C units in a building.
    /// </summary>
    [Serializable]
    public class TypeCUnitArrangerComponent : UnitArrangerComponent
    {
        #region Constructors

        public TypeCUnitArrangerComponent() : base()
        {
            base.MinNumOfFloors = 2;
            base.MaxNumOfFloors = 2;
            base.MinNumOfUnitsPerFloor = 2;
        }

        protected TypeCUnitArrangerComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _floorHeight = reader.GetValue<double>(nameof(FloorHeight));
            _entranceType =
                reader.GetValue<TypeCUnitEntranceType>(nameof(EntranceType));

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

        public IReadOnlyList<Tuple<TypeCUnitComponent, TypeCUnitComponent>>
            GetEntryPairs(
                HouseholdType householdType,
                TypeCUnitEntranceType entranceType,
                double previousSizeYInP
            )
        {
            var entryPairs = TypeCEntryPairs
                .Where(
                    entryPair =>
                    //entryPair.Item1.HouseholdType == householdType &&
                    entryPair.Item1.EntranceType == entranceType &&
                    IsSizeYInPValid(
                        entryPair.Item1.EntryName.SizeYInP,
                        previousSizeYInP
                    )
                ).ToList();

            return entryPairs;
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

        public Tuple<TypeCUnitComponent, TypeCUnitComponent>
            CreateFromEntryPair(
                HouseholdType householdType,
                TypeCUnitEntranceType entranceType,
                double normalizedIndex,
                double previousSizeYInP
            )
        {
            var entryPairs = GetEntryPairs(
                householdType, entranceType,
                previousSizeYInP
            );

            int entryIndex = MathUtils.MapDoubleToInt(
                normalizedIndex, 0.0, 1.0,
                0, entryPairs.Count - 1
            );

            var entryPair = entryPairs[entryIndex];

            var ucPair = new Tuple<TypeCUnitComponent, TypeCUnitComponent>(
                new TypeCUnitComponent(entryPair.Item1),
                new TypeCUnitComponent(entryPair.Item2)
            );
            return ucPair;
        }

        public void UpdateEntries()
        {
            _typeCEntryPairs.Clear();

            if (UnitCatalog == null) { return; }

            
            var typeCEntries =
                UnitCatalog.UnitCatalogComponent.Entries
                .Select(entry => entry as TypeCUnitComponent)
                .Where(
                    entry =>
                    entry != null &&
                    entry.EntryName.MainType == TypeCUnitComponent.MainType
                ).ToList();


            // Type C entries always come in pairs.

            for (
                int entryIndex = 0; entryIndex < typeCEntries.Count;
                entryIndex += 2
            )
            {
                var entry0 = typeCEntries[entryIndex];
                var entry1 = typeCEntries[entryIndex + 1];

                TypeCUnitComponent firstFloorEntry;
                TypeCUnitComponent secondFloorEntry;

                if (entry0.PositionType == TypeCUnitPositionType.FirstFloor)
                {
                    firstFloorEntry = entry0;
                    secondFloorEntry = entry1;
                }
                else
                {
                    firstFloorEntry = entry1;
                    secondFloorEntry = entry0;
                }

                var entryPair =
                    new Tuple<TypeCUnitComponent, TypeCUnitComponent>(
                        firstFloorEntry, secondFloorEntry
                    );

                _typeCEntryPairs.Add(entryPair);
            }
        }

        #endregion

        #region Unit arrangement

        public Tuple<TypeCUnitComponent, TypeCUnitComponent> CreateUnitPair(
            int stack, int numOfUnitsPerFloor
        )
        {
            bool isLastOddUnit =
                numOfUnitsPerFloor % 2 == 1 &&
                stack == numOfUnitsPerFloor - 1;

            bool isLeftUnit = stack % 2 == 0;
            bool isRightUnit = stack % 2 == 1;

            double normalizedEntryIndex;

            if (isLeftUnit || isLastOddUnit)
            {
                normalizedEntryIndex = NormalizedEntryIndices[stack];
            }
            else
            {
                normalizedEntryIndex = NormalizedEntryIndices[stack - 1];
            }

            double previousSizeYInP;

            if (isLastOddUnit)
            {
                int previousStack = stack - 1;
                var previousUc =
                    Building.BuildingComponent.GetUnit(0, previousStack)
                    .UnitComponent as TypeCUnitComponent;

                previousSizeYInP = previousUc.EntryName.SizeYInP;
            }
            else
            {
                int previousStack = stack / 2 * 2 - 2;

                if (previousStack >= 0)
                {
                    var previousUc =
                        Building.BuildingComponent.GetUnit(0, previousStack)
                        .UnitComponent as TypeCUnitComponent;

                    previousSizeYInP = previousUc.EntryName.SizeYInP;
                }
                else
                {
                    previousSizeYInP = InvalidSizeYInP;
                }
            }

            var ucPair = CreateFromEntryPair(
                HouseholdType, EntranceType, normalizedEntryIndex,
                previousSizeYInP
            );

            if (isLastOddUnit || isRightUnit)
            {
                ucPair.Item1.Mirror();
                ucPair.Item2.Mirror();
            }

            return ucPair;
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
                var ucPair = CreateUnitPair(stack, bc.NumOfUnitsPerFloor);

                Unit firstFloorUnit = new Unit
                {
                    UnitComponent = ucPair.Item1
                };
                Unit secondFloorUnit = new Unit
                {
                    UnitComponent = ucPair.Item2
                };

                building.Scene?.AddSceneObject(firstFloorUnit);
                building.Scene?.AddSceneObject(secondFloorUnit);

                bc.SetUnit(0, stack, firstFloorUnit);
                bc.SetUnit(1, stack, secondFloorUnit);
            }

            bc.UpdateUnitTransforms(0, 0);
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
                        as TypeCUnitComponent;

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

        public TypeCUnitEntranceType EntranceType
        {
            get => _entranceType;
            set
            {
                _entranceType = value;
                NotifyPropertyChanged();
            }
        }

        public IReadOnlyList<Tuple<TypeCUnitComponent, TypeCUnitComponent>>
            TypeCEntryPairs => _typeCEntryPairs;

        public HashSet<double> ValidSizeYInPDiffs { get; } =
            new HashSet<double> { -2.0, -1.0, 0.0, 1.0, 2.0 };
        public TypeCUnitRoofType RoofType
        {
            get => _roofType;
            set
            {
                _roofType = value;
                NotifyPropertyChanged();
            }
        }
        private TypeCUnitRoofType _roofType;

        #endregion

        #region Member variables

        private double _floorHeight = UnitComponent.DefaultRoomHeight;

        private TypeCUnitEntranceType _entranceType;

        private readonly List<Tuple<TypeCUnitComponent, TypeCUnitComponent>>
            _typeCEntryPairs =
            new List<Tuple<TypeCUnitComponent, TypeCUnitComponent>>();

        #endregion

        #region Constants

        public const double InvalidSizeYInP = -1;

        #endregion
    }
}
