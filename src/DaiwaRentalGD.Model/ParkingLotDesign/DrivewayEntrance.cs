using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A scene object representing a driveway entrance.
    /// </summary>
    [Serializable]
    public class DrivewayEntrance : SceneObject
    {
        #region Constructors

        public DrivewayEntrance() : base()
        {
            InitializeComponents();
        }

        protected DrivewayEntrance(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _drivewayEntranceComponent =
                reader.GetValue<DrivewayEntranceComponent>(
                    nameof(DrivewayEntranceComponent)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(DrivewayEntranceComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(DrivewayEntranceComponent), _drivewayEntranceComponent
            );
        }

        #endregion

        #region Properties

        public DrivewayEntranceComponent DrivewayEntranceComponent
        {
            get => _drivewayEntranceComponent;
            set
            {
                ReplaceComponent(_drivewayEntranceComponent, value);
                _drivewayEntranceComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private DrivewayEntranceComponent _drivewayEntranceComponent =
            new DrivewayEntranceComponent();

        #endregion
    }
}
