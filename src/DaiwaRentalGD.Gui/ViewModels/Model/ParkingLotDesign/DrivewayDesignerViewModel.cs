using System.Collections.ObjectModel;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign
    /// .DrivewayDesignerComponent"/>.
    /// </summary>
    public class DrivewayDesignerViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public DrivewayDesignerViewModel(GDModelScene gdms) : base(gdms)
        {
            DrivewayEntranceViewModels =
                new ReadOnlyObservableCollection<DrivewayEntranceViewModel>(
                    _drivewayEntranceViewModels
                );

            UpdateDrivewayEntranceViewModels();
        }

        #endregion

        #region Methods

        private void ClearDrivewayEntranceViewNModels()
        {
            foreach (var viewModel in _drivewayEntranceViewModels)
            {
                viewModel.IsActivated = false;
            }

            _drivewayEntranceViewModels.Clear();
        }

        private void UpdateDrivewayEntranceViewModels()
        {
            ClearDrivewayEntranceViewNModels();

            for (int drivewayEntranceIndex = 0;
                drivewayEntranceIndex <
                DrivewayDesignerComponent.DrivewayEntrances.Count;
                ++drivewayEntranceIndex)
            {
                var drivewayEntrance = DrivewayDesignerComponent
                    .DrivewayEntrances[drivewayEntranceIndex];

                var viewModel = new DrivewayEntranceViewModel(
                    this, drivewayEntrance, drivewayEntranceIndex
                );

                _drivewayEntranceViewModels.Add(viewModel);
            }
        }

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(DrivewayDesignerComponent));
            NotifyPropertyChanged(nameof(NumOfDrivewayEntrances));
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
                    UpdateDrivewayEntranceViewModels();
                }
                else
                {
                    ClearDrivewayEntranceViewNModels();
                }
            }
        }

        public DrivewayDesignerComponent DrivewayDesignerComponent =>
            GDModelScene.ParkingLotDesigner.DrivewayDesignerComponent;

        public int NumOfDrivewayEntrances
        {
            get => DrivewayDesignerComponent.NumOfDrivewayEntrances;
            set
            {
                DrivewayDesignerComponent.NumOfDrivewayEntrances = value;

                UpdateDrivewayEntranceViewModels();

                UpdateGDModelScene();
            }
        }

        public ReadOnlyObservableCollection<DrivewayEntranceViewModel>
            DrivewayEntranceViewModels
        { get; }

        public int MaxRoadsideIndex =>
            DrivewayDesignerComponent
            .ParkingLot.ParkingLotComponent.MaxRoadsideIndex;

        public bool AllowDrivewayTurning
        {
            get => DrivewayDesignerComponent.AllowDrivewayTurning;
            set
            {
                DrivewayDesignerComponent.AllowDrivewayTurning = value;

                UpdateGDModelScene();
            }
        }

        #endregion

        #region Member variables

        private readonly ObservableCollection<DrivewayEntranceViewModel>
            _drivewayEntranceViewModels =
            new ObservableCollection<DrivewayEntranceViewModel>();

        #endregion
    }
}
