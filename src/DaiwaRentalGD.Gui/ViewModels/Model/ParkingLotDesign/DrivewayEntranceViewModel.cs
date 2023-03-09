using System;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.DrivewayEntrance"/>.
    /// </summary>
    public class DrivewayEntranceViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public DrivewayEntranceViewModel(
            DrivewayDesignerViewModel drivewayDesignerViewModel,
            DrivewayEntrance drivewayEntrance,
            int drivewayEntranceIndex
        ) : base(drivewayDesignerViewModel.GDModelScene)
        {
            DrivewayDesignerViewModel = drivewayDesignerViewModel;

            DrivewayEntrance = drivewayEntrance ??
                throw new ArgumentNullException(nameof(drivewayEntrance));

            DrivewayEntranceIndex = drivewayEntranceIndex;
        }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(RoadEdgeIndexIndex));
            NotifyPropertyChanged(nameof(RoadEdgeSafeParam));
            NotifyPropertyChanged(nameof(MaxRoadsideIndex));
        }

        #endregion

        #region Properties

        public DrivewayDesignerViewModel DrivewayDesignerViewModel { get; }

        public DrivewayEntrance DrivewayEntrance { get; }

        public int DrivewayEntranceIndex { get; }

        private DrivewayEntranceComponent DrivewayEntranceComponent =>
            DrivewayEntrance.DrivewayEntranceComponent;

        public int RoadEdgeIndexIndex
        {
            get => DrivewayEntranceComponent.RoadEdgeIndexIndex;
            set
            {
                DrivewayEntranceComponent.RoadEdgeIndexIndex = value;

                UpdateGDModelScene();
            }
        }

        public int MaxRoadsideIndex =>
            DrivewayDesignerViewModel.MaxRoadsideIndex;

        public double RoadEdgeSafeParam
        {
            get => DrivewayEntranceComponent.RoadEdgeSafeParam;
            set
            {
                DrivewayEntranceComponent.RoadEdgeSafeParam = value;

                UpdateGDModelScene();
            }
        }

        #endregion
    }
}
