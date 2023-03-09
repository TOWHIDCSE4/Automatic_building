using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.Finance;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that arranges the units in a building.
    /// </summary>
    [Serializable]
    public class UnitArrangerComponent : Component
    {
        #region Constructor

        public UnitArrangerComponent() : base()
        {
            AlignNormalizedEntryIndices();
        }

        protected UnitArrangerComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _building = reader.GetValue<Building>(nameof(Building));

            _unitCatalog = reader.GetValue<UnitCatalog>(nameof(UnitCatalog));
            _householdType =
                reader.GetValue<HouseholdType>(nameof(HouseholdType));

            _minNumOfFloors = reader.GetValue<int>(nameof(MinNumOfFloors));
            _maxNumOfFloors = reader.GetValue<int>(nameof(MaxNumOfFloors));
            _numOfFloors = reader.GetValue<int>(nameof(NumOfFloors));
            _normalizedNumOfFloors =
                reader.GetValue<double>(nameof(NormalizedNumOfFloors));

            _minNumOfUnitsPerFloor =
                reader.GetValue<int>(nameof(MinNumOfUnitsPerFloor));
            _maxNumOfUnitsPerFloor =
                reader.GetValue<int>(nameof(MaxNumOfUnitsPerFloor));
            _numOfUnitsPerFloor =
                reader.GetValue<int>(nameof(NumOfUnitsPerFloor));
            _unitTypeForNumber =
                reader.GetValue<int>(nameof(UnitTypeForNumber));
            _entraceTypeForNumber =
            reader.GetValue<int>(nameof(EntraceTypeForNumber));
            _normalizedNumOfUnitsPerFloor =
                reader.GetValue<double>(nameof(NormalizedNumOfUnitsPerFloor));

            var normalizedEntryIndices =
                reader.GetValues<double>(nameof(NormalizedEntryIndices));

            foreach (var index in normalizedEntryIndices)
            {
                _normalizedEntryIndices.Add(index);
            }
        }

        #endregion

        #region Methods

        public virtual void ArrangeUnits(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            building.BuildingComponent.ClearBuilding();
        }

        public virtual void SetNormalizedEntryIndex(
            int entryIndexIndex, double normalizedEntryIndex
        )
        {
            if (normalizedEntryIndex < 0.0 || normalizedEntryIndex > 1.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(normalizedEntryIndex),
                    $"{nameof(normalizedEntryIndex)} must be between " +
                    "0.0 (inclusive) and 1.0 (inclusive)"
                );
            }

            _normalizedEntryIndices[entryIndexIndex] = normalizedEntryIndex;
        }

        protected override void Update()
        {
            if (Building == null) { return; }

            ArrangeUnits(Building);
        }

        private void AlignNormalizedEntryIndices()
        {
            while (_normalizedEntryIndices.Count > NumOfUnitsPerFloor)
            {
                _normalizedEntryIndices
                    .RemoveAt(_normalizedEntryIndices.Count - 1);
            }

            while (_normalizedEntryIndices.Count < NumOfUnitsPerFloor)
            {
                _normalizedEntryIndices.Add(DefaultNormalizedEntryIndex);
            }
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(Building).Append(UnitCatalog);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Building), _building);
            writer.AddValue(nameof(UnitCatalog), _unitCatalog);
            writer.AddValue(nameof(HouseholdType), _householdType);

            writer.AddValue(nameof(MinNumOfFloors), _minNumOfFloors);
            writer.AddValue(nameof(MaxNumOfFloors), _maxNumOfFloors);
            writer.AddValue(nameof(NumOfFloors), _numOfFloors);
            writer.AddValue(
                nameof(NormalizedNumOfFloors), _normalizedNumOfFloors
            );

            writer.AddValue(
                nameof(MinNumOfUnitsPerFloor), _minNumOfUnitsPerFloor
            );
            writer.AddValue(
                nameof(MaxNumOfUnitsPerFloor), _maxNumOfUnitsPerFloor
            );
            writer.AddValue(
                nameof(NumOfUnitsPerFloor), _numOfUnitsPerFloor
            );
            writer.AddValue(
             nameof(UnitTypeForNumber), _unitTypeForNumber
            );
            writer.AddValue(
          nameof(EntraceTypeForNumber), _entraceTypeForNumber
         );
            writer.AddValue(
                nameof(NormalizedNumOfUnitsPerFloor),
                _normalizedNumOfUnitsPerFloor
            );

            writer.AddValues(
                nameof(NormalizedEntryIndices),
                _normalizedEntryIndices.ToList()
            );
        }

        #endregion

        #region Properties

        public Building Building
        {
            get => _building;
            set
            {
                _building = value;
                NotifyPropertyChanged();
            }
        }

        public UnitCatalog UnitCatalog
        {
            get => _unitCatalog;
            set
            {
                _unitCatalog = value;
                NotifyPropertyChanged();
            }
        }

        public HouseholdType HouseholdType
        {
            get => _householdType;
            set
            {
                _householdType = value;
                NotifyPropertyChanged();
            }
        }



        public virtual int MinNumOfFloors
        {
            get => _minNumOfFloors;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MinNumOfFloors)} must be positive"
                    );
                }
                if (value > MaxNumOfFloors)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MinNumOfFloors)} cannot be greater than " +
                        $"{nameof(MaxNumOfFloors)}"
                    );
                }

                _minNumOfFloors = value;

                NumOfFloors = Math.Max(NumOfFloors, value);

                NotifyPropertyChanged();
            }
        }

        public virtual int MaxNumOfFloors
        {
            get => _maxNumOfFloors;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxNumOfFloors)} must be positive"
                    );
                }
                if (value < MinNumOfFloors)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxNumOfFloors)} cannot be smaller than " +
                        $"{nameof(MinNumOfFloors)}"
                    );
                }

                _maxNumOfFloors = value;

                NumOfFloors = Math.Min(NumOfFloors, value);

                NotifyPropertyChanged();
            }
        }

        public int NumOfFloors
        {
            get => _numOfFloors;
            set
            {
                if (value < MinNumOfFloors || value > MaxNumOfFloors)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(NumOfFloors)} must be between " +
                        $"{nameof(MinNumOfFloors)} (inclusive) and " +
                        $"{nameof(MaxNumOfFloors)} (inclusive)"
                    );
                }

                _numOfFloors = value;

                _normalizedNumOfFloors = MathUtils.MapIntToDouble(
                    NumOfFloors, MinNumOfFloors, MaxNumOfFloors,
                    0.0, 1.0
                );

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NormalizedNumOfFloors));
            }
        }

        public double NormalizedNumOfFloors
        {
            get => _normalizedNumOfFloors;
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(NormalizedNumOfFloors)} must be between " +
                        $"0.0 (inclusive) and 1.0 (inclusive)"
                    );
                }

                _normalizedNumOfFloors = value;

                _numOfFloors = MathUtils.MapDoubleToInt(
                    NormalizedNumOfFloors, 0.0, 1.0,
                    MinNumOfFloors, MaxNumOfFloors
                );

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NumOfFloors));
            }
        }





        public virtual int MinNumOfUnitsPerFloor
        {
            get => _minNumOfUnitsPerFloor;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MinNumOfUnitsPerFloor)} must be positive"
                    );
                }
                if (value > MaxNumOfUnitsPerFloor)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MinNumOfUnitsPerFloor)} cannot be " +
                        $"greather than {nameof(MaxNumOfUnitsPerFloor)}"
                    );
                }

                _minNumOfUnitsPerFloor = value;

                NumOfUnitsPerFloor = Math.Max(NumOfUnitsPerFloor, value);

                NotifyPropertyChanged();
            }
        }

        public virtual int MaxNumOfUnitsPerFloor
        {
            get => _maxNumOfUnitsPerFloor;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxNumOfUnitsPerFloor)} must be positive"
                    );
                }
                if (value < MinNumOfUnitsPerFloor)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxNumOfUnitsPerFloor)} cannot be " +
                        $"smaller than {nameof(MinNumOfUnitsPerFloor)}"
                    );
                }

                _maxNumOfUnitsPerFloor = value;

                NumOfUnitsPerFloor = Math.Min(NumOfUnitsPerFloor, value);

                NotifyPropertyChanged();
            }
        }

        public int NumOfUnitsPerFloor
        {
            get => _numOfUnitsPerFloor;
            set
            {
                if (
                    value < MinNumOfUnitsPerFloor ||
                    value > MaxNumOfUnitsPerFloor
                )
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(NumOfUnitsPerFloor)} must be between " +
                        $"{nameof(MinNumOfUnitsPerFloor)} (inclusive) and " +
                        $"{nameof(MaxNumOfUnitsPerFloor)} (inclusive)"
                    );
                }

                _numOfUnitsPerFloor = value;

                _normalizedNumOfUnitsPerFloor = MathUtils.MapIntToDouble(
                    NumOfUnitsPerFloor,
                    MinNumOfUnitsPerFloor, MaxNumOfUnitsPerFloor,
                    0.0, 1.0
                );

                AlignNormalizedEntryIndices();

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NormalizedNumOfUnitsPerFloor));
            }
        }
        public double NormalizedNumOfUnitsPerFloor
        {
            get => _normalizedNumOfUnitsPerFloor;
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(NormalizedNumOfUnitsPerFloor)} " +
                        "must be between 0.0 (inclusive) and 1.0 (inclusive)"
                    );
                }

                _normalizedNumOfUnitsPerFloor = value;

                _numOfUnitsPerFloor = MathUtils.MapDoubleToInt(
                    NormalizedNumOfUnitsPerFloor, 0.0, 1.0,
                    MinNumOfUnitsPerFloor, MaxNumOfUnitsPerFloor
                );

                AlignNormalizedEntryIndices();

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NumOfUnitsPerFloor));
            }
        }




        public double NormalizedUnitTypeForNumber
        {
            get => _normalizedUnitTypeForNumber;
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(NormalizedUnitTypeForNumber)} " +
                        "must be between 0.0 (inclusive) and 1.0 (inclusive)"
                    );
                }

                _normalizedUnitTypeForNumber = value;

                _unitTypeForNumber = MathUtils.MapDoubleToInt(
                    NormalizedUnitTypeForNumber, 0.0, 1.0,
                    MinUnitTypeForNumber, MaxUnitTypeForNumber
                );

                AlignNormalizedEntryIndices();

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(UnitTypeForNumber));
            }
        }
        public virtual int MinUnitTypeForNumber
        {
            get => _minUnitTypeForNumber;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MinUnitTypeForNumber)} must be positive"
                    );
                }
                if (value > MaxUnitTypeForNumber)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MinUnitTypeForNumber)} cannot be " +
                        $"greather than {nameof(MaxUnitTypeForNumber)}"
                    );
                }

                _minUnitTypeForNumber = value;

                UnitTypeForNumber = Math.Max(UnitTypeForNumber, value);

                NotifyPropertyChanged();
            }
        }

        public virtual int MaxUnitTypeForNumber
        {
            get => _maxUnitTypeForNumber;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxUnitTypeForNumber)} must be positive"
                    );
                }
                if (value < MinUnitTypeForNumber)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxUnitTypeForNumber)} cannot be " +
                        $"smaller than {nameof(MinUnitTypeForNumber)}"
                    );
                }

                _maxUnitTypeForNumber = value;

                UnitTypeForNumber = Math.Min(UnitTypeForNumber, value);

                NotifyPropertyChanged();
            }
        }
        public int UnitTypeForNumber
        {
            get => _unitTypeForNumber;
            set
            {
                if (
                    value < MinUnitTypeForNumber ||
                    value > MaxUnitTypeForNumber
                )
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(UnitTypeForNumber)} must be between " +
                        $"{nameof(MinUnitTypeForNumber)} (inclusive) and " +
                        $"{nameof(MaxUnitTypeForNumber)} (inclusive)"
                    );
                }

                _unitTypeForNumber = value;

                _normalizedUnitTypeForNumber = MathUtils.MapIntToDouble(
                    UnitTypeForNumber,
                    MinUnitTypeForNumber, MaxUnitTypeForNumber,
                    0.0, 1.0
                );

                AlignNormalizedEntryIndices();

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NormalizedUnitTypeForNumber));
            }
        }

        public int EntraceTypeForNumber
        {
            get => _entraceTypeForNumber;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(EntraceTypeForNumber)} must be between " +
                        $" MinEntraceTypeForNumber (inclusive) and " +
                        $" MaxEntraceTypeForNumber (inclusive)"
                    );
                }

                _entraceTypeForNumber = value;
                NotifyPropertyChanged();
            }
        }



        public IReadOnlyList<double> NormalizedEntryIndices =>
            _normalizedEntryIndices;
        #endregion

        #region Member variables

        private Building _building;
        private UnitCatalog _unitCatalog;

        private HouseholdType _householdType = DefaultHouseholdType;

        private int _minNumOfFloors = DefaultMinNumOfFloors;
        private int _maxNumOfFloors = DefaultMaxNumOfFloors;
        private int _numOfFloors = DefaultMinNumOfFloors;
        private double _normalizedNumOfFloors = 0.0;


        private int _minNumOfUnitsPerFloor = DefaultMinNumOfUnitsPerFloor;
        private int _maxNumOfUnitsPerFloor = DefaultMaxNumOfUnitsPerFloor;
        private int _numOfUnitsPerFloor = DefaultMinNumOfUnitsPerFloor;
        private double _normalizedNumOfUnitsPerFloor = 0.0;


        private int _minUnitTypeForNumber = DefaultMinUnitTypeForNumber;
        private int _maxUnitTypeForNumber = DefaultMaxUnitTypeForNumber;
        private int _unitTypeForNumber = DefaultMinUnitTypeForNumber;
        private double _normalizedUnitTypeForNumber = 0.0;

        private int _entraceTypeForNumber = DefaultMinUnitTypeForNumber;
        private readonly ObservableCollection<double>
            _normalizedEntryIndices =
            new ObservableCollection<double>();

        #endregion

        #region Constants

        public const HouseholdType DefaultHouseholdType =
            HouseholdType.SinglePerson;

        public const int DefaultMinNumOfFloors = 2;
        public const int DefaultMaxNumOfFloors = 3;

        public const int DefaultMinNumOfUnitsPerFloor = 2;
        public const int DefaultMaxNumOfUnitsPerFloor = 8;

        public const int DefaultMinUnitTypeForNumber = 0;
        public const int DefaultMaxUnitTypeForNumber = 2;

        public const double DefaultNormalizedEntryIndex = 0.0;

        public const int DefaultMinEntraceTypeForNumber = 0;
        public const int DefaultMaxEntraceTypeForNumber = 1;


        #endregion
    }
}
