using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// The component for designing driveways on a parking lot.
    /// </summary>
    [Serializable]
    public class DrivewayDesignerComponent : Component
    {
        #region Constructors

        public DrivewayDesignerComponent() : base()
        {
            Initialize();
        }

        protected DrivewayDesignerComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _numOfDrivewayEntrances =
                reader.GetValue<int>(nameof(NumOfDrivewayEntrances));

            var drivewayEntrances =
                reader.GetValues<DrivewayEntrance>(nameof(DrivewayEntrances));

            foreach (var drivewayEntrance in drivewayEntrances)
            {
                _drivewayEntrances.Add(drivewayEntrance);
            }

            _drivewayShapeGrammar = reader.GetValue<DrivewayShapeGrammar>(
                nameof(DrivewayShapeGrammar)
            );

            _drivewaySGState =
                reader.GetValue<DrivewaySGState>(nameof(DrivewaySGState));
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            AlignDrivewayEntrances();
        }

        protected override void Update()
        {
            if (ParkingLot == null)
            {
                return;
            }

            UpdateDrivewayEntranceRoadEdgeIndexIndices();
            AddDrivewayEntrances();

            _drivewaySGState.ParkingLotRequirementsComponent =
                ParkingLotRequirementsComponent;
            _drivewaySGState.Reset();

            _drivewayShapeGrammar.Run(_drivewaySGState);

            ParkingLotRequirementsComponent?.OnCarParkingStatsUpdated();
        }

        private void AlignDrivewayEntrances()
        {
            while (_drivewayEntrances.Count > NumOfDrivewayEntrances)
            {
                _drivewayEntrances.RemoveAt(_drivewayEntrances.Count - 1);
            }

            while (_drivewayEntrances.Count < NumOfDrivewayEntrances)
            {
                _drivewayEntrances.Add(CreateDrivewayEntrance());
            }
        }

        private void AddDrivewayEntrances()
        {
            var plc = ParkingLot.ParkingLotComponent;

            foreach (var de in DrivewayEntrances)
            {
                plc.AddDrivewayEntrance(de);
            }
        }

        private DrivewayEntrance CreateDrivewayEntrance()
        {
            var de = new DrivewayEntrance();

            de.DrivewayEntranceComponent.Width =
                DrivewayBuilder.DrivewayTileComponentTemplate.Width;

            return de;
        }

        private void UpdateDrivewayEntranceRoadEdgeIndexIndices()
        {
            int maxRoadsideIndex =
                ParkingLot.ParkingLotComponent.MaxRoadsideIndex;

            if (maxRoadsideIndex < 0)
            {
                return;
            }

            foreach (var de in DrivewayEntrances)
            {
                de.DrivewayEntranceComponent.RoadEdgeIndexIndex = Math.Min(
                    de.DrivewayEntranceComponent.RoadEdgeIndexIndex,
                    maxRoadsideIndex
                );
            }
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Concat(DrivewayEntrances)
            .Append(DrivewayShapeGrammar)
            .Append(DrivewaySGState);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(NumOfDrivewayEntrances), _numOfDrivewayEntrances
            );
            writer.AddValues(nameof(DrivewayEntrances), _drivewayEntrances);
            writer.AddValue(
                nameof(DrivewayShapeGrammar), _drivewayShapeGrammar
            );
            writer.AddValue(nameof(DrivewaySGState), _drivewaySGState);
        }

        public bool AllowDrivewayTurning
        {
            get => _AllowDrivewayTurning;
            set
            {
                _AllowDrivewayTurning = value;
                _drivewayShapeGrammar.setShapeGrammerRule_AllowDrivewayTuring(value);
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Properties

        public ParkingLot ParkingLot
        {
            get => _drivewaySGState.ParkingLot;
            set
            {
                _drivewaySGState.ParkingLot = value;

                NotifyPropertyChanged();
            }
        }

        public ParkingLotRequirementsComponent ParkingLotRequirementsComponent
            => SceneObject?.GetComponent<ParkingLotRequirementsComponent>();

        public int NumOfDrivewayEntrances
        {
            get => _numOfDrivewayEntrances;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(NumOfDrivewayEntrances)} cannot be negative"
                    );
                }

                _numOfDrivewayEntrances = value;

                AlignDrivewayEntrances();

                NotifyPropertyChanged();
            }
        }

        public DrivewayBuilder DrivewayBuilder =>
            _drivewaySGState.DrivewayBuilder;

        public IReadOnlyList<DrivewayEntrance> DrivewayEntrances =>
            _drivewayEntrances;

        public DrivewaySGState DrivewaySGState => _drivewaySGState;

        public DrivewayShapeGrammar DrivewayShapeGrammar =>
            _drivewayShapeGrammar;

        #endregion

        #region Member variables

        private bool _AllowDrivewayTurning = DefaultAllowDrivewayTurning;

        private int _numOfDrivewayEntrances = DefaultNumOfDrivewayEntrances;

        private readonly ObservableCollection<DrivewayEntrance>
            _drivewayEntrances =
            new ObservableCollection<DrivewayEntrance>();

        private readonly DrivewaySGState _drivewaySGState =
            new DrivewaySGState();

        private readonly DrivewayShapeGrammar _drivewayShapeGrammar =
            new DrivewayShapeGrammar();

        #endregion

        #region Constants

        public const int DefaultNumOfDrivewayEntrances = 0;
        public const bool DefaultAllowDrivewayTurning = true;

        #endregion
    }
}
