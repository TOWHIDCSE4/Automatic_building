using System;
using System.Collections.ObjectModel;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign
    /// .RoadsideCarParkingAreaDesignerComponent"/>.
    /// </summary>
    public class RoadsideCarParkingAreaDesignerViewModel :
        GDModelSceneViewModelBase
    {
        #region Constructors

        public RoadsideCarParkingAreaDesignerViewModel(GDModelScene gdms) :
            base(gdms)
        {
            RoadsideCarParkingAreaParamsViewModels =
                new ReadOnlyObservableCollection<
                    RoadsideCarParkingAreaParamsViewModel
                >(_roadsideCarParkingAreaParamsViewModels);

            UpdateRoadsideCarParkingAreaParamsViewModels();
        }

        #endregion

        #region Methods

        protected override void GDModelSceneUpdatedEventHandler(
            object sender, EventArgs e
        )
        {
            base.GDModelSceneUpdatedEventHandler(sender, e);

            UpdateRoadsideCarParkingAreaParamsViewModels();
        }

        private void ClearRoadsideCarParkingAreaParamsViewModels()
        {
            foreach (var viewModel in _roadsideCarParkingAreaParamsViewModels)
            {
                viewModel.IsActivated = false;
            }

            _roadsideCarParkingAreaParamsViewModels.Clear();
        }

        private void UpdateRoadsideCarParkingAreaParamsViewModels()
        {
             var rcpapList =
                RoadsideCarParkingAreaDesignerComponent
                .RoadsideCarParkingAreaParamsList;

            while (_roadsideCarParkingAreaParamsViewModels.Count >
                rcpapList.Count)
            {
                _roadsideCarParkingAreaParamsViewModels.RemoveAt(
                    _roadsideCarParkingAreaParamsViewModels.Count - 1
                );
            }

            while (_roadsideCarParkingAreaParamsViewModels.Count <
                rcpapList.Count)
            {
                int roadsideIndex =
                    _roadsideCarParkingAreaParamsViewModels.Count;

                var rcpap = rcpapList[roadsideIndex];

                var viewModel =
                    new RoadsideCarParkingAreaParamsViewModel(
                        GDModelScene, rcpap, roadsideIndex
                    );

                _roadsideCarParkingAreaParamsViewModels.Add(viewModel);
            }
        }

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(
                nameof(RoadsideCarParkingAreaDesignerComponent)
            );
            NotifyPropertyChanged(nameof(RoadsideStartIndex));
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
                    UpdateRoadsideCarParkingAreaParamsViewModels();
                }
                else
                {
                    ClearRoadsideCarParkingAreaParamsViewModels();
                }
            }
        }

        public RoadsideCarParkingAreaDesignerComponent
            RoadsideCarParkingAreaDesignerComponent =>
            GDModelScene.ParkingLotDesigner
            .RoadsideCarParkingAreaDesignerComponent;

        public int RoadsideStartIndex
        {
            get => RoadsideCarParkingAreaDesignerComponent
                .RoadsideStartIndex;
            set
            {
                RoadsideCarParkingAreaDesignerComponent
                    .RoadsideStartIndex = value;

                UpdateGDModelScene();
            }
        }

        public int MaxRoadsideIndex =>
            RoadsideCarParkingAreaDesignerComponent
            .ParkingLot.ParkingLotComponent.MaxRoadsideIndex;

        public ReadOnlyObservableCollection<
            RoadsideCarParkingAreaParamsViewModel
        > RoadsideCarParkingAreaParamsViewModels
        { get; }

        #endregion

        #region Member variables

        private readonly
            ObservableCollection<RoadsideCarParkingAreaParamsViewModel>
            _roadsideCarParkingAreaParamsViewModels =
            new ObservableCollection<RoadsideCarParkingAreaParamsViewModel>();

        #endregion
    }
}
