using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component that calculates the requirements for parking lot.
    /// </summary>
    [Serializable]
    public class ParkingLotRequirementsComponent : Component
    {
        #region Constructors

        public ParkingLotRequirementsComponent() : base()
        {
            UnitRequirementsTable =
                new ReadOnlyDictionary<int, UnitParkingRequirements>(
                    _unitRequirementsTable
                );
        }

        protected ParkingLotRequirementsComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            UnitRequirementsTable =
                new ReadOnlyDictionary<int, UnitParkingRequirements>(
                    _unitRequirementsTable
                );

            var reader = new WorkspaceItemReader(this, info, context);

            _parkingLot = reader.GetValue<ParkingLot>(nameof(ParkingLot));

            _overrideCarParkingSpaceMinTotal = reader.GetValue<double>(
                nameof(OverrideCarParkingSpaceMinTotal)
            );
            _autoCarParkingSpaceMinTotal = reader.GetValue<double>(
                nameof(AutoCarParkingSpaceMinTotal)
            );
            _useOverrideCarParkingSpaceMinTotal = reader.GetValue<bool>(
                nameof(UseOverrideCarParkingSpaceMinTotal)
            );
            _overrideCarParkingSpaceMaxTotal = reader.GetValue<double>(
                nameof(OverrideCarParkingSpaceMaxTotal)
            );
            _autoCarParkingSpaceMaxTotal = reader.GetValue<double>(
                nameof(AutoCarParkingSpaceMaxTotal)
            );
            _useOverrideCarParkingSpaceMaxTotal = reader.GetValue<bool>(
                nameof(UseOverrideCarParkingSpaceMaxTotal)
            );

            _overrideBicycleParkingSpaceMinTotal = reader.GetValue<double>(
                nameof(OverrideBicycleParkingSpaceMinTotal)
            );
            _autoBicycleParkingSpaceMinTotal = reader.GetValue<double>(
                nameof(AutoBicycleParkingSpaceMinTotal)
            );
            _useOverrideBicycleParkingSpaceMinTotal = reader.GetValue<bool>(
                nameof(UseOverrideBicycleParkingSpaceMinTotal)
            );
            _overrideBicycleParkingSpaceMaxTotal = reader.GetValue<double>(
                nameof(OverrideBicycleParkingSpaceMaxTotal)
            );
            _autoBicycleParkingSpaceMaxTotal = reader.GetValue<double>(
                nameof(AutoBicycleParkingSpaceMaxTotal)
            );
            _useOverrideBicycleParkingSpaceMaxTotal = reader.GetValue<bool>(
                nameof(UseOverrideBicycleParkingSpaceMaxTotal)
            );

            var unitRequirementPairs =
                reader.GetValues<KeyValuePair<int, UnitParkingRequirements>>(
                    nameof(UnitRequirementsTable)
                );

            foreach (var pair in unitRequirementPairs)
            {
                _unitRequirementsTable.Add(pair.Key, pair.Value);
            }
        }

        #endregion

        #region Methods

        #region Parking requirements

        public void AddUnitRequirements(UnitParkingRequirements upr)
        {
            if (upr == null)
            {
                throw new ArgumentNullException(nameof(upr));
            }

            if (UnitRequirementsTable.ContainsKey(upr.NumOfBedrooms))
            {
                throw new ArgumentException(
                    $"{nameof(UnitRequirementsTable)} already contains " +
                    $"requirements for " +
                    $"{upr.NumOfBedrooms}-bedroom units",
                    nameof(upr)
                );
            }

            _unitRequirementsTable[upr.NumOfBedrooms] = upr;
        }

        public bool RemoveUnitRequirements(int numOfBedrooms)
        {
            return _unitRequirementsTable.Remove(numOfBedrooms);
        }

        public void ClearUnitRequirementsTable()
        {
            _unitRequirementsTable.Clear();
        }

        public double GetAutoCarParkingSpaceMinTotal(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            var units = building.BuildingComponent.Units;

            if (!units.Any())
            {
                return 0.0;
            }

            var numOfBedroomsList =
                (from unit in units select unit.UnitComponent.NumOfBedrooms)
                .ToList();

            var carParkingSpaceMins =
                from numOfBedrooms in numOfBedroomsList
                select (
                    UnitRequirementsTable.ContainsKey(numOfBedrooms) ?
                    UnitRequirementsTable[numOfBedrooms].CarParkingSpaceMin :
                    UnitParkingRequirements.DefaultCarParkingSpaceMin
                );

            double carParkingSpaceMinTotal = carParkingSpaceMins.Sum();

            return carParkingSpaceMinTotal;
        }

        public double GetAutoCarParkingSpaceMaxTotal(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            var units = building.BuildingComponent.Units;

            if (!units.Any())
            {
                return 0.0;
            }

            var numOfBedroomsList =
                (from unit in units select unit.UnitComponent.NumOfBedrooms)
                .ToList();

            var carParkingSpaceMaxs =
                from numOfBedrooms in numOfBedroomsList
                select (
                    UnitRequirementsTable.ContainsKey(numOfBedrooms) ?
                    UnitRequirementsTable[numOfBedrooms].CarParkingSpaceMax :
                    UnitParkingRequirements.DefaultCarParkingSpaceMax
                );

            double carParkingSpaceMaxTotal = carParkingSpaceMaxs.Sum();

            return carParkingSpaceMaxTotal;
        }

        public double GetAutoBicycleParkingSpaceMinTotal(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            var units = building.BuildingComponent.Units;

            if (!units.Any())
            {
                return 0.0;
            }

            var numOfBedroomsList =
                (from unit in units select unit.UnitComponent.NumOfBedrooms)
                .ToList();

            var bicycleParkingSpaceMins =
                from numOfBedrooms in numOfBedroomsList
                select (
                    UnitRequirementsTable.ContainsKey(numOfBedrooms) ?
                    UnitRequirementsTable[numOfBedrooms]
                    .BicycleParkingSpaceMin :
                    UnitParkingRequirements.DefaultBicycleParkingSpaceMin
                );

            double bicycleParkingSpaceMinTotal =
                bicycleParkingSpaceMins.Sum();

            return bicycleParkingSpaceMinTotal;
        }

        public double GetAutoBicycleParkingSpaceMaxTotal(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            var units = building.BuildingComponent.Units;

            if (!units.Any())
            {
                return 0.0;
            }

            var numOfBedroomsList =
                (from unit in units select unit.UnitComponent.NumOfBedrooms)
                .ToList();

            var bicycleParkingSpaceMaxs =
                from numOfBedrooms in numOfBedroomsList
                select (
                    UnitRequirementsTable.ContainsKey(numOfBedrooms) ?
                    UnitRequirementsTable[numOfBedrooms]
                    .BicycleParkingSpaceMax :
                    UnitParkingRequirements.DefaultBicycleParkingSpaceMax
                );

            double bicycleParkingSpaceMaxTotal =
                bicycleParkingSpaceMaxs.Sum();

            return bicycleParkingSpaceMaxTotal;
        }

        #endregion

        protected override void Update()
        {
            AutoCarParkingSpaceMinTotal = 0.0;
            AutoCarParkingSpaceMaxTotal = 0.0;
            AutoBicycleParkingSpaceMinTotal = 0.0;
            AutoBicycleParkingSpaceMaxTotal = 0.0;

            Building building = ParkingLot?.ParkingLotComponent.Building;

            if (building == null)
            {
                return;
            }

            AutoCarParkingSpaceMinTotal =
                GetAutoCarParkingSpaceMinTotal(building);
            AutoCarParkingSpaceMaxTotal =
                GetAutoCarParkingSpaceMaxTotal(building);
            AutoBicycleParkingSpaceMinTotal =
                GetAutoBicycleParkingSpaceMinTotal(building);
            AutoBicycleParkingSpaceMaxTotal =
                GetAutoBicycleParkingSpaceMaxTotal(building);
        }

        public void OnCarParkingStatsUpdated()
        {
            NotifyPropertyChanged(nameof(NumOfCarParkingSpaces));
            NotifyPropertyChanged(nameof(CarParkingSpaceFulfillment));
            NotifyPropertyChanged(nameof(DrivewayAreaTotal));
            NotifyPropertyChanged(nameof(DrivewayAreaPerCarParkingSpace));
        }

        public void OnBicycleParkingStatsUpdated()
        {
            NotifyPropertyChanged(nameof(NumOfBicycleParkingSpaces));
            NotifyPropertyChanged(nameof(BicycleParkingSpaceFulfillment));
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(ParkingLot);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(ParkingLot), _parkingLot);

            writer.AddValue(
                nameof(OverrideCarParkingSpaceMinTotal),
                _overrideCarParkingSpaceMinTotal
            );
            writer.AddValue(
                nameof(AutoCarParkingSpaceMinTotal),
                _autoCarParkingSpaceMinTotal
            );
            writer.AddValue(
                nameof(UseOverrideCarParkingSpaceMinTotal),
                _useOverrideCarParkingSpaceMinTotal
            );
            writer.AddValue(
                nameof(OverrideCarParkingSpaceMaxTotal),
                _overrideCarParkingSpaceMaxTotal
            );
            writer.AddValue(
                nameof(AutoCarParkingSpaceMaxTotal),
                _autoCarParkingSpaceMaxTotal
            );
            writer.AddValue(
                nameof(UseOverrideCarParkingSpaceMaxTotal),
                _useOverrideCarParkingSpaceMaxTotal
            );

            writer.AddValue(
                nameof(OverrideBicycleParkingSpaceMinTotal),
                _overrideBicycleParkingSpaceMinTotal
            );
            writer.AddValue(
                nameof(AutoBicycleParkingSpaceMinTotal),
                _autoBicycleParkingSpaceMinTotal
            );
            writer.AddValue(
                nameof(UseOverrideBicycleParkingSpaceMinTotal),
                _useOverrideBicycleParkingSpaceMinTotal
            );
            writer.AddValue(
                nameof(OverrideBicycleParkingSpaceMaxTotal),
                _overrideBicycleParkingSpaceMaxTotal
            );
            writer.AddValue(
                nameof(AutoBicycleParkingSpaceMaxTotal),
                _autoBicycleParkingSpaceMaxTotal
            );
            writer.AddValue(
                nameof(UseOverrideBicycleParkingSpaceMaxTotal),
                _useOverrideBicycleParkingSpaceMaxTotal
            );

            writer.AddValues(
                nameof(UnitRequirementsTable),
                _unitRequirementsTable.ToList()
            );
        }

        #endregion

        #region Properties

        public ParkingLot ParkingLot
        {
            get => _parkingLot;
            set
            {
                _parkingLot = value;
                NotifyPropertyChanged();
            }
        }

        #region Walkways

        public IReadOnlyList<WalkwayPath> WalkwayPaths =>
            ParkingLot?.ParkingLotComponent.WalkwayPaths ??
            new List<WalkwayPath>();

        public IReadOnlyList<WalkwayPath> SiteEntranceWalkwayPaths =>
            WalkwayPaths
            .Where(
                wp => wp.SourceVertex.Type ==
                WalkwayGraphVertexType.Source
            ).ToList();

        public IReadOnlyList<WalkwayPath> BuildingEntranceWalkwayPaths =>
            WalkwayPaths
            .Where(
                wp => wp.SourceVertex.Type ==
                WalkwayGraphVertexType.Destination
            ).ToList();

        public bool IsBuildingAccessibleViaWalkway
        {
            get
            {
                if (ParkingLot == null)
                {
                    return false;
                }

                var plc = ParkingLot.ParkingLotComponent;
                var bc = plc.Building.BuildingComponent;

                return
                    SiteEntranceWalkwayPaths.Count >= 1 &&
                    BuildingEntranceWalkwayPaths.Count ==
                    bc.Entrances.Count - 1;
            }
        }

        #endregion

        public ReadOnlyDictionary<int, UnitParkingRequirements>
            UnitRequirementsTable
        { get; }

        #region Car parking

        public double OverrideCarParkingSpaceMinTotal
        {
            get => _overrideCarParkingSpaceMinTotal;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(OverrideCarParkingSpaceMinTotal)} " +
                        "cannot be negative"
                    );
                }

                _overrideCarParkingSpaceMinTotal = value;

                NotifyPropertyChanged();

                if (UseOverrideCarParkingSpaceMinTotal)
                {
                    NotifyPropertyChanged(nameof(CarParkingSpaceMinTotal));
                }
            }
        }

        public double AutoCarParkingSpaceMinTotal
        {
            get => _autoCarParkingSpaceMinTotal;
            private set
            {
                _autoCarParkingSpaceMinTotal = value;

                NotifyPropertyChanged();

                if (!UseOverrideCarParkingSpaceMinTotal)
                {
                    NotifyPropertyChanged(nameof(CarParkingSpaceMinTotal));
                }
            }
        }

        public bool UseOverrideCarParkingSpaceMinTotal
        {
            get => _useOverrideCarParkingSpaceMinTotal;
            set
            {
                _useOverrideCarParkingSpaceMinTotal = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CarParkingSpaceMinTotal));
            }
        }

        public double CarParkingSpaceMinTotal =>
            UseOverrideCarParkingSpaceMinTotal ?
            OverrideCarParkingSpaceMinTotal :
            AutoCarParkingSpaceMinTotal;

        public double OverrideCarParkingSpaceMaxTotal
        {
            get => _overrideCarParkingSpaceMaxTotal;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(OverrideCarParkingSpaceMaxTotal)} " +
                        "cannot be negative"
                    );
                }

                _overrideCarParkingSpaceMaxTotal = value;

                NotifyPropertyChanged();

                if (UseOverrideCarParkingSpaceMaxTotal)
                {
                    NotifyPropertyChanged(nameof(CarParkingSpaceMaxTotal));
                }
            }
        }

        public double AutoCarParkingSpaceMaxTotal
        {
            get => _autoCarParkingSpaceMaxTotal;
            private set
            {
                _autoCarParkingSpaceMaxTotal = value;
                NotifyPropertyChanged();

                if (!UseOverrideCarParkingSpaceMaxTotal)
                {
                    NotifyPropertyChanged(nameof(CarParkingSpaceMaxTotal));
                }
            }
        }

        public bool UseOverrideCarParkingSpaceMaxTotal
        {
            get => _useOverrideCarParkingSpaceMaxTotal;
            set
            {
                _useOverrideCarParkingSpaceMaxTotal = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CarParkingSpaceMaxTotal));
            }
        }

        public double CarParkingSpaceMaxTotal =>
            UseOverrideCarParkingSpaceMaxTotal ?
            OverrideCarParkingSpaceMaxTotal :
            AutoCarParkingSpaceMaxTotal;

        public int NumOfCarParkingSpaces =>
            ParkingLot?.ParkingLotComponent.NumOfCarParkingSpaces ?? 0;

        public double CarParkingSpaceFulfillment =>
            CarParkingSpaceMinTotal == 0.0 ? 1.0 :
            NumOfCarParkingSpaces / CarParkingSpaceMinTotal;

        public double DrivewayAreaTotal
        {
            get
            {
                var dts = ParkingLot?.ParkingLotComponent.DrivewayTiles;

                if (dts == null || dts.Count == 0)
                {
                    return 0.0;
                }

                double dtAreaTotal =
                    dts
                    .Select(dt => dt.DrivewayTileComponent.GetPlan().Area)
                    .Sum();

                return dtAreaTotal;
            }
        }

        public double DrivewayAreaPerCarParkingSpace
        {
            get
            {
                int numOfCpss = NumOfCarParkingSpaces;

                if (numOfCpss == 0)
                {
                    return 0.0;
                }

                double dtAreaTotal = DrivewayAreaTotal;
                double dtAreaPerCps = dtAreaTotal / numOfCpss;

                return dtAreaPerCps;
            }
        }

        #endregion

        #region Bicycle parking

        public double OverrideBicycleParkingSpaceMinTotal
        {
            get => _overrideBicycleParkingSpaceMinTotal;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(OverrideBicycleParkingSpaceMinTotal)} " +
                        "cannot be negative"
                    );
                }

                _overrideBicycleParkingSpaceMinTotal = value;

                NotifyPropertyChanged();

                if (UseOverrideBicycleParkingSpaceMinTotal)
                {
                    NotifyPropertyChanged(nameof(BicycleParkingSpaceMinTotal));
                }
            }
        }

        public double AutoBicycleParkingSpaceMinTotal
        {
            get => _autoBicycleParkingSpaceMinTotal;
            private set
            {
                _autoBicycleParkingSpaceMinTotal = value;

                NotifyPropertyChanged();

                if (!UseOverrideBicycleParkingSpaceMinTotal)
                {
                    NotifyPropertyChanged(nameof(BicycleParkingSpaceMinTotal));
                }
            }
        }

        public bool UseOverrideBicycleParkingSpaceMinTotal
        {
            get => _useOverrideBicycleParkingSpaceMinTotal;
            set
            {
                _useOverrideBicycleParkingSpaceMinTotal = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(BicycleParkingSpaceMinTotal));
            }
        }

        public double BicycleParkingSpaceMinTotal =>
            UseOverrideBicycleParkingSpaceMinTotal ?
            OverrideBicycleParkingSpaceMinTotal :
            AutoBicycleParkingSpaceMinTotal;

        public double OverrideBicycleParkingSpaceMaxTotal
        {
            get => _overrideBicycleParkingSpaceMaxTotal;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(OverrideBicycleParkingSpaceMaxTotal)} " +
                        "cannot be negative"
                    );
                }

                _overrideBicycleParkingSpaceMaxTotal = value;

                NotifyPropertyChanged();

                if (UseOverrideBicycleParkingSpaceMaxTotal)
                {
                    NotifyPropertyChanged(nameof(BicycleParkingSpaceMaxTotal));
                }
            }
        }

        public double AutoBicycleParkingSpaceMaxTotal
        {
            get => _autoBicycleParkingSpaceMaxTotal;
            private set
            {
                _autoBicycleParkingSpaceMaxTotal = value;
                NotifyPropertyChanged();

                if (!UseOverrideBicycleParkingSpaceMaxTotal)
                {
                    NotifyPropertyChanged(nameof(BicycleParkingSpaceMaxTotal));
                }
            }
        }

        public bool UseOverrideBicycleParkingSpaceMaxTotal
        {
            get => _useOverrideBicycleParkingSpaceMaxTotal;
            set
            {
                _useOverrideBicycleParkingSpaceMaxTotal = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(BicycleParkingSpaceMaxTotal));
            }
        }

        public double BicycleParkingSpaceMaxTotal =>
            UseOverrideBicycleParkingSpaceMaxTotal ?
            OverrideBicycleParkingSpaceMaxTotal :
            AutoBicycleParkingSpaceMaxTotal;

        public int NumOfBicycleParkingSpaces =>
            ParkingLot?.ParkingLotComponent.NumOfBicycleParkingSpaces ?? 0;

        public double BicycleParkingSpaceFulfillment =>
            BicycleParkingSpaceMinTotal == 0.0 ? 1.0 :
            NumOfBicycleParkingSpaces / BicycleParkingSpaceMinTotal;

        #endregion

        #endregion

        #region Member variables

        private ParkingLot _parkingLot;

        #region Car parking

        private double _overrideCarParkingSpaceMinTotal =
            DefaultOverrideCarParkingSpaceMinTotal;

        private double _autoCarParkingSpaceMinTotal = 0.0;

        private bool _useOverrideCarParkingSpaceMinTotal =
            DefaultUseOverrideCarParkingSpaceMinTotal;

        private double _overrideCarParkingSpaceMaxTotal =
            DefaultOverrideCarParkingSpaceMaxTotal;

        private double _autoCarParkingSpaceMaxTotal = 0.0;

        private bool _useOverrideCarParkingSpaceMaxTotal =
            DefaultUseOverrideCarParkingSpaceMaxTotal;

        #endregion

        #region Bicycle parking

        private double _overrideBicycleParkingSpaceMinTotal =
            DefaultOverrideBicycleParkingSpaceMinTotal;

        private double _autoBicycleParkingSpaceMinTotal = 0.0;

        private bool _useOverrideBicycleParkingSpaceMinTotal =
            DefaultUseOverrideBicycleParkingSpaceMinTotal;

        private double _overrideBicycleParkingSpaceMaxTotal =
            DefaultOverrideBicycleParkingSpaceMaxTotal;

        private double _autoBicycleParkingSpaceMaxTotal = 0.0;

        private bool _useOverrideBicycleParkingSpaceMaxTotal =
            DefaultUseOverrideBicycleParkingSpaceMaxTotal;

        private readonly Dictionary<int, UnitParkingRequirements>
            _unitRequirementsTable =
            new Dictionary<int, UnitParkingRequirements>();

        #endregion

        #endregion

        #region Constants

        #region Car parking

        public const double DefaultOverrideCarParkingSpaceMinTotal = 8.0;
        public const double DefaultOverrideCarParkingSpaceMaxTotal = 12.0;

        public const bool DefaultUseOverrideCarParkingSpaceMinTotal = false;
        public const bool DefaultUseOverrideCarParkingSpaceMaxTotal = false;

        #endregion

        #region Bicycle parking

        public const double DefaultOverrideBicycleParkingSpaceMinTotal = 8.0;
        public const double DefaultOverrideBicycleParkingSpaceMaxTotal = 12.0;

        public const bool DefaultUseOverrideBicycleParkingSpaceMinTotal =
            false;
        public const bool DefaultUseOverrideBicycleParkingSpaceMaxTotal =
            false;

        #endregion

        #endregion
    }
}
