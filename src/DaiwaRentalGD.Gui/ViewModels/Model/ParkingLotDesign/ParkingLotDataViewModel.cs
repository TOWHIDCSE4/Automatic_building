using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign
{
    /// <summary>
    /// View model for data related to parking lot.
    /// </summary>
    public class ParkingLotDataViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public ParkingLotDataViewModel(GDModelScene gdms) : base(gdms)
        { }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(CarParkingSpaceFulfillment));
            NotifyPropertyChanged(nameof(NumOfCarParkingSpaces));
            NotifyPropertyChanged(nameof(MinNumOfCarParkingSpaces));
            NotifyPropertyChanged(nameof(MaxNumOfCarParkingSpaces));

            NotifyPropertyChanged(nameof(DrivewayAreaTotal));
            NotifyPropertyChanged(nameof(DrivewayAreaPerCarParkingSpace));

            NotifyPropertyChanged(nameof(BicycleParkingSpaceFulfillment));
            NotifyPropertyChanged(nameof(NumOfBicycleParkingSpaces));
            NotifyPropertyChanged(nameof(MinNumOfBicycleParkingSpaces));
            NotifyPropertyChanged(nameof(MaxNumOfBicycleParkingSpaces));
        }

        #endregion

        #region Properties

        public double? CarParkingSpaceFulfillment =>
            GDModelScene?.ParkingLotDesigner.ParkingLotRequirementsComponent
            .CarParkingSpaceFulfillment;

        public int? NumOfCarParkingSpaces =>
            GDModelScene?.ParkingLot.ParkingLotComponent
            .NumOfCarParkingSpaces;

        public double? MinNumOfCarParkingSpaces =>
            GDModelScene?.ParkingLotDesigner.ParkingLotRequirementsComponent
            .CarParkingSpaceMinTotal;

        public double? MaxNumOfCarParkingSpaces =>
            GDModelScene?.ParkingLotDesigner.ParkingLotRequirementsComponent
            .CarParkingSpaceMaxTotal;

        public double? DrivewayAreaTotal =>
            GDModelScene?.ParkingLotDesigner.ParkingLotRequirementsComponent
            .DrivewayAreaTotal;

        public double? DrivewayAreaPerCarParkingSpace =>
            GDModelScene?.ParkingLotDesigner.ParkingLotRequirementsComponent
            .DrivewayAreaPerCarParkingSpace;

        public double? BicycleParkingSpaceFulfillment =>
            GDModelScene?.ParkingLotDesigner.ParkingLotRequirementsComponent
            .BicycleParkingSpaceFulfillment;

        public int? NumOfBicycleParkingSpaces =>
            GDModelScene?.ParkingLot.ParkingLotComponent
            .NumOfBicycleParkingSpaces;

        public double? MinNumOfBicycleParkingSpaces =>
            GDModelScene?.ParkingLotDesigner.ParkingLotRequirementsComponent
            .BicycleParkingSpaceMinTotal;

        public double? MaxNumOfBicycleParkingSpaces =>
            GDModelScene?.ParkingLotDesigner.ParkingLotRequirementsComponent
            .BicycleParkingSpaceMaxTotal;

        #endregion
    }
}
