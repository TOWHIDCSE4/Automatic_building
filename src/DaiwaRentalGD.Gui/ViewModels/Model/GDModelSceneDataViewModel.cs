using DaiwaRentalGD.Gui.ViewModels.Model.Finance;
using DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign;
using DaiwaRentalGD.Gui.ViewModels.Model.Zoning;
using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model
{
    /// <summary>
    /// View model for data of
    /// a <see cref="DaiwaRentalGD.Model.GDModelScene"/>.
    /// </summary>
    public class GDModelSceneDataViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public GDModelSceneDataViewModel(GDModelScene gdms) : base(gdms)
        {
            SlantPlanesDataViewModel = new SlantPlanesDataViewModel(gdms);
            LandUseDataViewModel = new LandUseDataViewModel(gdms);
            ParkingLotDataViewModel = new ParkingLotDataViewModel(gdms);
            FinanceDataViewModel = new FinanceDataViewModel(gdms);
        }

        #endregion

        #region Properties

        public SlantPlanesDataViewModel SlantPlanesDataViewModel { get; }

        public LandUseDataViewModel LandUseDataViewModel { get; }

        public ParkingLotDataViewModel ParkingLotDataViewModel { get; }

        public FinanceDataViewModel FinanceDataViewModel { get; }

        #endregion
    }
}
