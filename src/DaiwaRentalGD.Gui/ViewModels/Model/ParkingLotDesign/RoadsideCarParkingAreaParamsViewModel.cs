using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign
    /// .RoadsideCarParkingAreaParams"/>.
    /// </summary>
    public class RoadsideCarParkingAreaParamsViewModel :
        GDModelSceneViewModelBase
    {
        #region Constructors

        public RoadsideCarParkingAreaParamsViewModel(
            GDModelScene gdms, RoadsideCarParkingAreaParams rcpap,
            int roadsideIndex
        ) : base(gdms)
        {
            RoadsideCarParkingAreaParams = rcpap;
            RoadsideIndex = roadsideIndex;
        }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(IsEnabled));
            NotifyPropertyChanged(nameof(Offset));
            NotifyPropertyChanged(nameof(MaxOffset));
        }

        #endregion

        #region Properties

        public RoadsideCarParkingAreaParams RoadsideCarParkingAreaParams
        { get; }

        public int RoadsideIndex { get; }

        public bool IsEnabled
        {
            get => RoadsideCarParkingAreaParams.IsEnabled;
            set
            {
                RoadsideCarParkingAreaParams.IsEnabled = value;

                UpdateGDModelScene();
            }
        }

        public double Offset
        {
            get => RoadsideCarParkingAreaParams.Offset;
            set
            {
                RoadsideCarParkingAreaParams.Offset = value;

                UpdateGDModelScene();
            }
        }

        public double MaxOffset
        {
            get => RoadsideCarParkingAreaParams.MaxOffset;
        }

        #endregion
    }
}
