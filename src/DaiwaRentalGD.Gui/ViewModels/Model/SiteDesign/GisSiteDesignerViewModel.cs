using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model.SiteDesign
{
    /// <summary>
    /// View model for a GIS-based
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.SiteDesigner"/>.
    /// </summary>
    public class GisSiteDesignerViewModel : SiteDesignerViewModelBase
    {
        #region Constructors

        public GisSiteDesignerViewModel(GDModelScene gdms) : base(gdms)
        { }

        #endregion

        #region Properties

        public override string SiteDesignerName { get; } =
            GisSiteDesignerName;

        #endregion

        #region Constants

        public const string GisSiteDesignerName = "GIS";

        #endregion
    }
}
