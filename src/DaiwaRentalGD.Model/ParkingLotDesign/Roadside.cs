using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A roadside of the site boundary that can be used for car parking.
    /// </summary>
    [Serializable]
    public class Roadside : SceneObject
    {
        #region Constructors

        public Roadside() : base()
        {
            InitializeComponents();
        }

        protected Roadside(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _transformComponent = reader.GetValue<TransformComponent>(
                nameof(TransformComponent)
            );
            _cpaac = reader.GetValue<CarParkingAreaAnchorComponent>(
                nameof(CarParkingAreaAnchorComponent)
            );
            _roadsideComponent = reader.GetValue<RoadsideComponent>(
                nameof(RoadsideComponent)
            );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(CarParkingAreaAnchorComponent);
            AddComponent(RoadsideComponent);
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
                nameof(CarParkingAreaAnchorComponent), _cpaac
            );
            writer.AddValue(nameof(RoadsideComponent), _roadsideComponent);
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

        public CarParkingAreaAnchorComponent CarParkingAreaAnchorComponent
        {
            get => _cpaac;
            set
            {
                ReplaceComponent(_cpaac, value);
                _cpaac = value;

                NotifyPropertyChanged();
            }
        }

        public RoadsideComponent RoadsideComponent
        {
            get => _roadsideComponent;
            set
            {
                ReplaceComponent(_roadsideComponent, value);
                _roadsideComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private CarParkingAreaAnchorComponent _cpaac =
            new CarParkingAreaAnchorComponent();

        private RoadsideComponent _roadsideComponent =
            new RoadsideComponent();

        #endregion
    }
}
