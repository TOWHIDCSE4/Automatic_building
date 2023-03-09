using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A scene object representing a walkway entrance.
    /// </summary>
    [Serializable]
    public class WalkwayEntrance : SceneObject
    {
        #region Constructors

        public WalkwayEntrance() : base()
        {
            InitializeComponents();
        }

        protected WalkwayEntrance(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _walkwayEntranceComponent =
                reader.GetValue<WalkwayEntranceComponent>(
                    nameof(WalkwayEntranceComponent)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(WalkwayEntranceComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(WalkwayEntranceComponent),
                _walkwayEntranceComponent
            );
        }

        #endregion

        #region Properties

        public WalkwayEntranceComponent WalkwayEntranceComponent
        {
            get => _walkwayEntranceComponent;
            set
            {
                ReplaceComponent(_walkwayEntranceComponent, value);
                _walkwayEntranceComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private WalkwayEntranceComponent _walkwayEntranceComponent =
            new WalkwayEntranceComponent();

        #endregion
    }
}
