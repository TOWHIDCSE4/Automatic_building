using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A scene object representing a driveway tile.
    /// </summary>
    [Serializable]
    public class DrivewayTile : SceneObject
    {
        #region Constructors

        public DrivewayTile() : base()
        {
            InitializeComponents();
        }

        protected DrivewayTile(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _transformComponent = reader.GetValue<TransformComponent>(
                nameof(TransformComponent)
            );
            _collisionBody2DComponent =
                reader.GetValue<CollisionBody2DComponent>(
                    nameof(CollisionBody2DComponent)
                );
            _leftCpaac = reader.GetValue<CarParkingAreaAnchorComponent>(
                nameof(LeftCarParkingAreaAnchorComponent)
            );
            _rightCpaac = reader.GetValue<CarParkingAreaAnchorComponent>(
                nameof(RightCarParkingAreaAnchorComponent)
            );
            _drivewayTileComponent = reader.GetValue<DrivewayTileComponent>(
                nameof(DrivewayTileComponent)
            );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(CollisionBody2DComponent);
            AddComponent(LeftCarParkingAreaAnchorComponent);
            AddComponent(RightCarParkingAreaAnchorComponent);
            AddComponent(DrivewayTileComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(TransformComponent), _transformComponent);
            writer.AddValue(
                nameof(CollisionBody2DComponent), _collisionBody2DComponent
            );
            writer.AddValue(
                nameof(LeftCarParkingAreaAnchorComponent),
                _leftCpaac
            );
            writer.AddValue(
                nameof(RightCarParkingAreaAnchorComponent),
                _rightCpaac
            );
            writer.AddValue(
                nameof(DrivewayTileComponent), _drivewayTileComponent
            );
        }

        #endregion

        #region Properties

        public TransformComponent TransformComponent
        {
            get => _transformComponent;
            set
            {
                ReplaceComponent(_transformComponent, value);
                _transformComponent = value;

                NotifyPropertyChanged();
            }
        }

        public CollisionBody2DComponent CollisionBody2DComponent
        {
            get => _collisionBody2DComponent;
            set
            {
                ReplaceComponent(_collisionBody2DComponent, value);
                _collisionBody2DComponent = value;

                NotifyPropertyChanged();
            }
        }

        public CarParkingAreaAnchorComponent
            LeftCarParkingAreaAnchorComponent
        {
            get => _leftCpaac;
            set
            {
                ReplaceComponent(_leftCpaac, value);
                _leftCpaac = value;

                NotifyPropertyChanged();
            }
        }

        public CarParkingAreaAnchorComponent
            RightCarParkingAreaAnchorComponent
        {
            get => _rightCpaac;
            set
            {
                ReplaceComponent(_rightCpaac, value);
                _rightCpaac = value;

                NotifyPropertyChanged();
            }
        }

        public DrivewayTileComponent DrivewayTileComponent
        {
            get => _drivewayTileComponent;
            set
            {
                ReplaceComponent(_drivewayTileComponent, value);
                _drivewayTileComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private CollisionBody2DComponent _collisionBody2DComponent =
            new CollisionBody2DComponent();

        private CarParkingAreaAnchorComponent _leftCpaac =
            new CarParkingAreaAnchorComponent();

        private CarParkingAreaAnchorComponent _rightCpaac =
            new CarParkingAreaAnchorComponent();

        private DrivewayTileComponent _drivewayTileComponent =
            new DrivewayTileComponent();

        #endregion
    }
}
