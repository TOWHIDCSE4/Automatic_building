using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A scene object representing a bikeway tile.
    /// </summary>
    [Serializable]
    public class BikewayTile : SceneObject
    {
        #region Constructors

        public BikewayTile() : base()
        {
            InitializeComponents();
        }

        protected BikewayTile(
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

            LeftBicycleParkingAreaAnchorComponent =
                reader.GetValue<BicycleParkingAreaAnchorComponent>(
                    nameof(LeftBicycleParkingAreaAnchorComponent)
                );

            RightBicycleParkingAreaAnchorComponent =
                reader.GetValue<BicycleParkingAreaAnchorComponent>(
                    nameof(RightBicycleParkingAreaAnchorComponent)
                );

            _bikewayTileComponent = reader.GetValue<BikewayTileComponent>(
                nameof(BikewayTileComponent)
            );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(CollisionBody2DComponent);
            AddComponent(LeftBicycleParkingAreaAnchorComponent);
            AddComponent(RightBicycleParkingAreaAnchorComponent);
            AddComponent(BikewayTileComponent);
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
                nameof(LeftBicycleParkingAreaAnchorComponent),
                LeftBicycleParkingAreaAnchorComponent
            );

            writer.AddValue(
                nameof(RightBicycleParkingAreaAnchorComponent),
                RightBicycleParkingAreaAnchorComponent
            );

            writer.AddValue(
                nameof(BikewayTileComponent), _bikewayTileComponent
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

        public BicycleParkingAreaAnchorComponent
            LeftBicycleParkingAreaAnchorComponent
        { get; } = new BicycleParkingAreaAnchorComponent();

        public BicycleParkingAreaAnchorComponent
            RightBicycleParkingAreaAnchorComponent
        { get; } = new BicycleParkingAreaAnchorComponent();

        public BikewayTileComponent BikewayTileComponent
        {
            get => _bikewayTileComponent;
            set
            {
                ReplaceComponent(_bikewayTileComponent, value);
                _bikewayTileComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private CollisionBody2DComponent _collisionBody2DComponent =
            new CollisionBody2DComponent();

        private BikewayTileComponent _bikewayTileComponent =
            new BikewayTileComponent();

        #endregion
    }
}
