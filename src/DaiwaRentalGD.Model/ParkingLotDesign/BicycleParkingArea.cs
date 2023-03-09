using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A scene object representing a bicycle parking area consisting of
    /// one or more bicycle parking spaces.
    /// </summary>
    [Serializable]
    public class BicycleParkingArea : SceneObject
    {
        #region Constructors

        public BicycleParkingArea() : base()
        {
            InitializeComponents();
        }

        protected BicycleParkingArea(
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
            _bicycleParkingAreaComponent =
                reader.GetValue<BicycleParkingAreaComponent>(
                    nameof(BicycleParkingAreaComponent)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(CollisionBody2DComponent);
            AddComponent(BicycleParkingAreaComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(TransformComponent), _transformComponent);
            writer.AddValue(
                nameof(CollisionBody2DComponent),
                _collisionBody2DComponent
            );
            writer.AddValue(
                nameof(BicycleParkingAreaComponent),
                _bicycleParkingAreaComponent
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

        public BicycleParkingAreaComponent BicycleParkingAreaComponent
        {
            get => _bicycleParkingAreaComponent;
            set
            {
                ReplaceComponent(_bicycleParkingAreaComponent, value);
                _bicycleParkingAreaComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private CollisionBody2DComponent _collisionBody2DComponent =
            new CollisionBody2DComponent();

        private BicycleParkingAreaComponent _bicycleParkingAreaComponent =
            new BicycleParkingAreaComponent();

        #endregion
    }
}
