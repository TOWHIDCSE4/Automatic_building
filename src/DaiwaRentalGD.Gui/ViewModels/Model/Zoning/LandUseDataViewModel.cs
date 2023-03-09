using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model.Zoning
{
    /// <summary>
    /// View model for data related to land use.
    /// </summary>
    public class LandUseDataViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public LandUseDataViewModel(GDModelScene gdms) : base(gdms)
        { }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(IsValidBuildingPlacement));
            NotifyPropertyChanged(nameof(FloorAreaRatio));
            NotifyPropertyChanged(nameof(TotalFloorArea));
            NotifyPropertyChanged(nameof(SiteArea));
            NotifyPropertyChanged(nameof(BuildingCoverageRatio));
            NotifyPropertyChanged(nameof(BuildingArea));
        }

        #endregion

        #region Properties

        public double? FloorAreaRatio =>
            GDModelScene?.LandUseEvaluator.FloorAreaRatioComponent
            .FloorAreaRatio;

        public double? TotalFloorArea =>
            GDModelScene?.LandUseEvaluator.FloorAreaRatioComponent
            .TotalFloorArea;

        public double? SiteArea =>
            GDModelScene?.LandUseEvaluator.FloorAreaRatioComponent
            .SiteArea;

        public double? BuildingCoverageRatio =>
            GDModelScene?.LandUseEvaluator.BuildingCoverageRatioComponent
            .BuildingCoverageRatio;

        public double? BuildingArea =>
            GDModelScene?.LandUseEvaluator.BuildingCoverageRatioComponent
            .BuildingArea;
        public bool? IsValidBuildingPlacement => GDModelScene?.BuildingPlacementValidation.BuildingPlacementComponent.IsValidBuildingPlacement;

        #endregion
    }
}
