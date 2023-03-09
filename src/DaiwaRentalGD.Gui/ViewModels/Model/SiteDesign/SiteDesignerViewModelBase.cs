using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model.SiteDesign
{
    /// <summary>
    /// Base class for view models for
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.SiteDesigner"/>.
    /// </summary>
    public abstract class SiteDesignerViewModelBase :
        GDModelSceneViewModelBase
    {
        #region Constructors

        protected SiteDesignerViewModelBase(GDModelScene gdms) : base(gdms)
        { }

        #endregion

        #region Properties

        public abstract string SiteDesignerName { get; }

        #endregion
    }
}
