using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model.Zoning
{
    /// <summary>
    /// View model for data related to slant planes.
    /// </summary>
    public class SlantPlanesDataViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public SlantPlanesDataViewModel(GDModelScene gdms) : base(gdms)
        { }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(IsNorthSlantPlanesValid));
            NotifyPropertyChanged(nameof(IsAdjacentSiteSlantPlanesValid));
            NotifyPropertyChanged(nameof(IsRoadSlantPlanesValid));
            NotifyPropertyChanged(nameof(IsAbsoluteHeightPlanesValid));
            NotifyPropertyChanged(nameof(IsGroundLevelSetbackValid));
        }

        #endregion

        #region Properties

        public bool? IsNorthSlantPlanesValid =>
            GDModelScene?.SlantPlanes.NorthSlantPlanesComponent
            .IsValid;

        public bool? IsAdjacentSiteSlantPlanesValid =>
            GDModelScene?.SlantPlanes.AdjacentSiteSlantPlanesComponent
            .IsValid;

        public bool? IsRoadSlantPlanesValid =>
            GDModelScene?.SlantPlanes.RoadSlantPlanesComponent
            .IsValid;
        public bool? IsAbsoluteHeightPlanesValid =>
           GDModelScene?.SlantPlanes.AbsoluteHeightSlantPlanesComponent
           .IsValid;
        

        public bool? IsGroundLevelSetbackValid =>
            GDModelScene?.Setback.SetbackResolverComponent
            .IsValid;

        #endregion
    }
}
