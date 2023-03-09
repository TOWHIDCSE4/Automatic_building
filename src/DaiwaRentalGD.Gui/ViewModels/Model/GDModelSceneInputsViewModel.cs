using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign;
using DaiwaRentalGD.Gui.ViewModels.Model.Finance;
using DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign;
using DaiwaRentalGD.Gui.ViewModels.Model.SiteDesign;
using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model
{
    /// <summary>
    /// View model for the inputs of
    /// <see cref="DaiwaRentalGD.Model.GDModelScene"/>.
    /// </summary>
    public class GDModelSceneInputsViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public GDModelSceneInputsViewModel(GDModelScene gdms) : base(gdms)
        {
            UnitCatalogInputsViewModel = new UnitCatalogInputsViewModel(gdms);

            SiteInputsViewModel = new SiteInputsViewModel(gdms);

            BuildingInputsViewModel = new BuildingInputsViewModel(gdms);

            ParkingLotDesignerViewModel =
                new ParkingLotDesignerViewModel(gdms);

            FinanceInputsViewModel = new FinanceInputsViewModel(gdms);
        }

        #endregion

        #region Properties

        public override bool IsActivated
        {
            get => base.IsActivated;
            set
            {
                base.IsActivated = value;

                SiteInputsViewModel.IsActivated = value;
                BuildingInputsViewModel.IsActivated = value;
                ParkingLotDesignerViewModel.IsActivated = value;
            }
        }

        public UnitCatalogInputsViewModel UnitCatalogInputsViewModel { get; }

        public SiteInputsViewModel SiteInputsViewModel { get; }

        public BuildingInputsViewModel BuildingInputsViewModel { get; }

        public ParkingLotDesignerViewModel ParkingLotDesignerViewModel
        { get; }

        public FinanceInputsViewModel FinanceInputsViewModel { get; }

        #endregion
    }
}
