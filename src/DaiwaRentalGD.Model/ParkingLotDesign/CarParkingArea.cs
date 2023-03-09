using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A scene object representing a car parking area consisting of
    /// one or more car parking spaces.
    /// </summary>
    [Serializable]
    public class CarParkingArea : SceneObject
    {
        #region Constructors

        public CarParkingArea() : base()
        {
            InitializeComponents();
        }

        protected CarParkingArea(
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
            _carParkingAreaComponent =
                reader.GetValue<CarParkingAreaComponent>(
                    nameof(CarParkingAreaComponent)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(CollisionBody2DComponent);
            AddComponent(CarParkingAreaComponent);
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
                nameof(CollisionBody2DComponent), _collisionBody2DComponent
            );
            writer.AddValue(
                nameof(CarParkingAreaComponent), _carParkingAreaComponent
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

        public CarParkingAreaComponent CarParkingAreaComponent
        {
            get => _carParkingAreaComponent;
            set
            {
                ReplaceComponent(_carParkingAreaComponent, value);
                _carParkingAreaComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private CollisionBody2DComponent _collisionBody2DComponent =
            new CollisionBody2DComponent();

        private CarParkingAreaComponent _carParkingAreaComponent =
            new CarParkingAreaComponent();

        #endregion
    }
}
