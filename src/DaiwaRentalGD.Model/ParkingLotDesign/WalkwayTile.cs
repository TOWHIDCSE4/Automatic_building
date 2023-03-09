using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A scene object representing a walkway tile.
    /// </summary>
    [Serializable]
    public class WalkwayTile : SceneObject
    {
        #region Constructors

        public WalkwayTile() : base()
        {
            InitializeComponents();
        }

        protected WalkwayTile(
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
            _walkwayTileComponent = reader.GetValue<WalkwayTileComponent>(
                nameof(WalkwayTileComponent)
            );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(CollisionBody2DComponent);
            AddComponent(WalkwayTileComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(TransformComponent), _transformComponent
            );
            writer.AddValue(
                nameof(CollisionBody2DComponent),
                _collisionBody2DComponent
            );
            writer.AddValue(
                nameof(WalkwayTileComponent),
                _walkwayTileComponent
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

        public WalkwayTileComponent WalkwayTileComponent
        {
            get => _walkwayTileComponent;
            set
            {
                ReplaceComponent(_walkwayTileComponent, value);
                _walkwayTileComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private CollisionBody2DComponent _collisionBody2DComponent =
            new CollisionBody2DComponent();

        private WalkwayTileComponent _walkwayTileComponent =
            new WalkwayTileComponent();

        #endregion
    }
}
