using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign
    /// .ParkingLotDesigner"/>.
    /// </summary>
    public class ParkingLotDesignerViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public ParkingLotDesignerViewModel(GDModelScene gdms) : base(gdms)
        {
            ParkingLotRequirementsViewModel =
                new ParkingLotRequirementsViewModel(gdms);

            WalkwayDesignerViewModel =
                new WalkwayDesignerViewModel(gdms);

            RoadsideCarParkingAreaDesignerViewModel =
                new RoadsideCarParkingAreaDesignerViewModel(gdms);

            DrivewayDesignerViewModel =
                new DrivewayDesignerViewModel(gdms);

            BikewayDesignerViewModel =
                new BikewayDesignerViewModel(gdms);
        }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(ParkingLotDesigner));
        }

        #endregion

        #region Properties

        public override bool IsActivated
        {
            get => base.IsActivated;
            set
            {
                base.IsActivated = value;

                ParkingLotRequirementsViewModel.IsActivated = value;
                WalkwayDesignerViewModel.IsActivated = value;
                RoadsideCarParkingAreaDesignerViewModel.IsActivated = value;
                DrivewayDesignerViewModel.IsActivated = value;
                BikewayDesignerViewModel.IsActivated = value;
            }
        }

        public ParkingLotDesigner ParkingLotDesigner =>
            GDModelScene.ParkingLotDesigner;

        public ParkingLotRequirementsViewModel ParkingLotRequirementsViewModel
        { get; }

        public WalkwayDesignerViewModel WalkwayDesignerViewModel
        { get; }

        public RoadsideCarParkingAreaDesignerViewModel
            RoadsideCarParkingAreaDesignerViewModel
        { get; }

        public DrivewayDesignerViewModel DrivewayDesignerViewModel
        { get; }

        public BikewayDesignerViewModel BikewayDesignerViewModel
        { get; }

        #endregion
    }
}
