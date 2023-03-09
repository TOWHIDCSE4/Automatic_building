using System.Collections.ObjectModel;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign
    /// .WalkwayDesignerComponent"/>.
    /// </summary>
    public class WalkwayDesignerViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public WalkwayDesignerViewModel(GDModelScene gdms) : base(gdms)
        {
            WalkwayEntranceViewModels =
                new ReadOnlyObservableCollection<WalkwayEntranceViewModel>(
                    _walkwayEntranceViewModels
                );

            UpdateWalkwayEntranceViewModels();
        }

        #endregion

        #region Methods

        private void ClearWalkwayEntranceViewNModels()
        {
            foreach (var viewModel in _walkwayEntranceViewModels)
            {
                viewModel.IsActivated = false;
            }

            _walkwayEntranceViewModels.Clear();
        }

        private void UpdateWalkwayEntranceViewModels()
        {
            ClearWalkwayEntranceViewNModels();

            for (int walkwayEntranceIndex = 0;
                walkwayEntranceIndex <
                WalkwayDesignerComponent.WalkwayEntrances.Count;
                ++walkwayEntranceIndex)
            {
                var walkwayEntrance = WalkwayDesignerComponent
                    .WalkwayEntrances[walkwayEntranceIndex];

                var viewModel = new WalkwayEntranceViewModel(
                    this, walkwayEntrance, walkwayEntranceIndex
                );

                _walkwayEntranceViewModels.Add(viewModel);
            }
        }

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(WalkwayDesignerComponent));
            NotifyPropertyChanged(nameof(NumOfWalkwayEntrances));
            NotifyPropertyChanged(nameof(MaxRoadsideIndex));
        }

        #endregion

        #region Properties

        public override bool IsActivated
        {
            get => base.IsActivated;
            set
            {
                if (value == IsActivated)
                {
                    return;
                }

                base.IsActivated = value;

                if (value)
                {
                    UpdateWalkwayEntranceViewModels();
                }
                else
                {
                    ClearWalkwayEntranceViewNModels();
                }
            }
        }

        public WalkwayDesignerComponent WalkwayDesignerComponent =>
            GDModelScene.ParkingLotDesigner.WalkwayDesignerComponent;

        public int NumOfWalkwayEntrances
        {
            get => WalkwayDesignerComponent.NumOfWalkwayEntrances;
            set
            {
                WalkwayDesignerComponent.NumOfWalkwayEntrances = value;

                UpdateWalkwayEntranceViewModels();

                UpdateGDModelScene();
            }
        }

        public ReadOnlyObservableCollection<WalkwayEntranceViewModel>
            WalkwayEntranceViewModels
        { get; }

        public int MaxRoadsideIndex =>
            WalkwayDesignerComponent
            .ParkingLot.ParkingLotComponent.MaxRoadsideIndex;

        public bool OverlapWithDriveways
        {
            get => WalkwayDesignerComponent.OverlapWithDriveways;
            set
            {
                WalkwayDesignerComponent.OverlapWithDriveways = value;

                UpdateGDModelScene();
            }
        }

        #endregion

        #region Member variables

        private readonly ObservableCollection<WalkwayEntranceViewModel>
            _walkwayEntranceViewModels =
            new ObservableCollection<WalkwayEntranceViewModel>();

        #endregion
    }
}
