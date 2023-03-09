using System;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.WalkwayEntrance"/>.
    /// </summary>
    public class WalkwayEntranceViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public WalkwayEntranceViewModel(
            WalkwayDesignerViewModel walkwayDesignerViewModel,
            WalkwayEntrance walkwayEntrance,
            int walkwayEntranceIndex
        ) : base(walkwayDesignerViewModel.GDModelScene)
        {
            WalkwayDesignerViewModel = walkwayDesignerViewModel;

            WalkwayEntrance = walkwayEntrance ??
                throw new ArgumentNullException(nameof(walkwayEntrance));

            WalkwayEntranceIndex = walkwayEntranceIndex;
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

        public WalkwayDesignerViewModel WalkwayDesignerViewModel { get; }

        public WalkwayEntrance WalkwayEntrance { get; }

        public int WalkwayEntranceIndex { get; }

        private WalkwayEntranceComponent WalkwayEntranceComponent =>
            WalkwayEntrance.WalkwayEntranceComponent;

        public int RoadEdgeIndexIndex
        {
            get => WalkwayEntranceComponent.RoadEdgeIndexIndex;
            set
            {
                WalkwayEntranceComponent.RoadEdgeIndexIndex = value;

                UpdateGDModelScene();
            }
        }

        public int MaxRoadsideIndex =>
            WalkwayDesignerViewModel.MaxRoadsideIndex;

        public double RoadEdgeSafeParam
        {
            get => WalkwayEntranceComponent.RoadEdgeSafeParam;
            set
            {
                WalkwayEntranceComponent.RoadEdgeSafeParam = value;

                UpdateGDModelScene();
            }
        }

        #endregion
    }
}
