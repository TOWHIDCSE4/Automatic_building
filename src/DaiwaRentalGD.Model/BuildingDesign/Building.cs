using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A scene object representing a building.
    /// </summary>
    [Serializable]
    public class Building : SceneObject
    {
        #region Constructors

        public Building() : base()
        {
            InitializeComponents();
        }

        protected Building(
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

            RigidBody2DComponent = reader.GetValue<RigidBody2DComponent>(
                nameof(RigidBody2DComponent)
            );

            _buildingComponent = reader.GetValue<BuildingComponent>(
                nameof(BuildingComponent)
            );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(CollisionBody2DComponent);
            AddComponent(RigidBody2DComponent);
            AddComponent(BuildingComponent);
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
                nameof(RigidBody2DComponent), RigidBody2DComponent
            );

            writer.AddValue(nameof(BuildingComponent), _buildingComponent);
        }

        #endregion

        #region Proeprties

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

        public RigidBody2DComponent RigidBody2DComponent { get; } =
            new RigidBody2DComponent();

        public BuildingComponent BuildingComponent
        {
            get => _buildingComponent;
            set
            {
                ReplaceComponent(_buildingComponent, value);
                _buildingComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private CollisionBody2DComponent _collisionBody2DComponent =
            new CollisionBody2DComponent();

        private BuildingComponent _buildingComponent =
            new BuildingComponent();

        #endregion
    }
}
