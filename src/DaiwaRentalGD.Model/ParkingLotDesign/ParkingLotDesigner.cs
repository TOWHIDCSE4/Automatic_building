using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A scene object containing components for designing various parts
    /// of a parking lot.
    /// </summary>
    [Serializable]
    public class ParkingLotDesigner : SceneObject
    {
        #region Constructors

        public ParkingLotDesigner() : base()
        {
            InitializeComponents();
        }

        protected ParkingLotDesigner(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _parkingLotRequirementsComponent =
                reader.GetValue<ParkingLotRequirementsComponent>(
                    nameof(ParkingLotRequirementsComponent)
                );
            _parkingRequirementsJsonComponent =
                reader.GetValue<ParkingRequirementsJsonComponent>(
                    nameof(ParkingRequirementsJsonComponent)
                );
            _walkwayDesignerComponent =
                reader.GetValue<WalkwayDesignerComponent>(
                    nameof(WalkwayDesignerComponent)
                );
            _roadsideCarParkingAreaDesignerComponent =
                reader.GetValue<RoadsideCarParkingAreaDesignerComponent>(
                    nameof(RoadsideCarParkingAreaDesignerComponent)
                );
            _drivewayDesignerComponent =
                reader.GetValue<DrivewayDesignerComponent>(
                    nameof(DrivewayDesignerComponent)
                );
            _bikewayDesignerComponent =
                reader.GetValue<BikewayDesignerComponent>(
                    nameof(BikewayDesignerComponent)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(ParkingLotRequirementsComponent);
            AddComponent(ParkingRequirementsJsonComponent);

            AddComponent(WalkwayDesignerComponent);
            AddComponent(RoadsideCarParkingAreaDesignerComponent);
            AddComponent(DrivewayDesignerComponent);
            AddComponent(BikewayDesignerComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(ParkingLotRequirementsComponent),
                _parkingLotRequirementsComponent
            );
            writer.AddValue(
                nameof(ParkingRequirementsJsonComponent),
                _parkingRequirementsJsonComponent
            );
            writer.AddValue(
                nameof(WalkwayDesignerComponent),
                _walkwayDesignerComponent
            );
            writer.AddValue(
                nameof(RoadsideCarParkingAreaDesignerComponent),
                _roadsideCarParkingAreaDesignerComponent
            );
            writer.AddValue(
                nameof(DrivewayDesignerComponent),
                _drivewayDesignerComponent
            );
            writer.AddValue(
                nameof(BikewayDesignerComponent),
                _bikewayDesignerComponent
            );
        }

        public void setDesignOrder_Overlap(bool overlap)
        {

            if (overlap)
            {
                RemoveComponent(DrivewayDesignerComponent);
                int ix = GetComponentIndex(WalkwayDesignerComponent);
                InsertComponent(ix, DrivewayDesignerComponent);
            }
            else
            {
                RemoveComponent(DrivewayDesignerComponent);
                int ix = GetComponentIndex(BikewayDesignerComponent);
                InsertComponent(ix, DrivewayDesignerComponent);
            }
        }

        #endregion

        #region Properties

        public ParkingLotRequirementsComponent ParkingLotRequirementsComponent
        {
            get => _parkingLotRequirementsComponent;
            set
            {
                var oldParkingLotRequirementsComponent =
                    _parkingLotRequirementsComponent;

                ReplaceComponent(_parkingLotRequirementsComponent, value);
                _parkingLotRequirementsComponent = value;

                _parkingLotRequirementsComponent.ParkingLot =
                    oldParkingLotRequirementsComponent.ParkingLot;

                NotifyPropertyChanged();
            }
        }

        public ParkingRequirementsJsonComponent
            ParkingRequirementsJsonComponent
        {
            get => _parkingRequirementsJsonComponent;
            set
            {
                ReplaceComponent(_parkingRequirementsJsonComponent, value);
                _parkingRequirementsJsonComponent = value;

                NotifyPropertyChanged();
            }
        }

        public WalkwayDesignerComponent WalkwayDesignerComponent
        {
            get => _walkwayDesignerComponent;
            set
            {
                var oldWalkwayDesignerComponent = _walkwayDesignerComponent;

                ReplaceComponent(_walkwayDesignerComponent, value);
                _walkwayDesignerComponent = value;

                _walkwayDesignerComponent.ParkingLot =
                    oldWalkwayDesignerComponent.ParkingLot;

                NotifyPropertyChanged();
            }
        }

        public RoadsideCarParkingAreaDesignerComponent
            RoadsideCarParkingAreaDesignerComponent
        {
            get => _roadsideCarParkingAreaDesignerComponent;
            set
            {
                var oldRoadsideCarParkingAreaDesignerComponent =
                    _roadsideCarParkingAreaDesignerComponent;

                ReplaceComponent(
                    _roadsideCarParkingAreaDesignerComponent, value
                );
                _roadsideCarParkingAreaDesignerComponent = value;

                _roadsideCarParkingAreaDesignerComponent.ParkingLot =
                    oldRoadsideCarParkingAreaDesignerComponent.ParkingLot;

                NotifyPropertyChanged();
            }
        }

        public DrivewayDesignerComponent DrivewayDesignerComponent
        {
            get => _drivewayDesignerComponent;
            set
            {
                var oldDrivewayDesignerComponent =
                    _drivewayDesignerComponent;

                ReplaceComponent(_drivewayDesignerComponent, value);
                _drivewayDesignerComponent = value;

                _drivewayDesignerComponent.ParkingLot =
                    oldDrivewayDesignerComponent.ParkingLot;

                NotifyPropertyChanged();
            }
        }

        public BikewayDesignerComponent BikewayDesignerComponent
        {
            get => _bikewayDesignerComponent;
            set
            {
                var oldBikewayDesignerComponent = _bikewayDesignerComponent;

                ReplaceComponent(_bikewayDesignerComponent, value);
                _bikewayDesignerComponent = value;

                _bikewayDesignerComponent.ParkingLot =
                    oldBikewayDesignerComponent.ParkingLot;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private ParkingLotRequirementsComponent
            _parkingLotRequirementsComponent =
            new ParkingLotRequirementsComponent();

        private ParkingRequirementsJsonComponent
            _parkingRequirementsJsonComponent =
            new ParkingRequirementsJsonComponent();

        private WalkwayDesignerComponent _walkwayDesignerComponent =
            new WalkwayDesignerComponent();

        private RoadsideCarParkingAreaDesignerComponent
            _roadsideCarParkingAreaDesignerComponent =
            new RoadsideCarParkingAreaDesignerComponent();

        private DrivewayDesignerComponent _drivewayDesignerComponent =
            new DrivewayDesignerComponent();

        private BikewayDesignerComponent _bikewayDesignerComponent =
            new BikewayDesignerComponent();

        #endregion
    }
}
