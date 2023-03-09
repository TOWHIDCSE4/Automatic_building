using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A scene object that represents a parking lot.
    /// </summary>
    [Serializable]
    public class ParkingLot : SceneObject
    {
        #region Constructors

        public ParkingLot() : base()
        {
            InitializeComponents();
        }

        protected ParkingLot(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _transformComponent = reader.GetValue<TransformComponent>(
                nameof(TransformComponent)
            );
            _parkingLotComponent = reader.GetValue<ParkingLotComponent>(
                nameof(ParkingLotComponent)
            );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(ParkingLotComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(TransformComponent), _transformComponent);
            writer.AddValue(
                nameof(ParkingLotComponent), _parkingLotComponent
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

        public ParkingLotComponent ParkingLotComponent
        {
            get => _parkingLotComponent;
            set
            {
                ReplaceComponent(_parkingLotComponent, value);
                _parkingLotComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private ParkingLotComponent _parkingLotComponent =
            new ParkingLotComponent();

        #endregion
    }
}
