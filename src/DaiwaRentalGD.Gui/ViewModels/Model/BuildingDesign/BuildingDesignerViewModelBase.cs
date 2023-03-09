using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.BuildingDesigner"/>.
    /// </summary>
    public abstract class BuildingDesignerViewModelBase :
        GDModelSceneViewModelBase
    {
        #region Constructors

        protected BuildingDesignerViewModelBase(GDModelScene gdms) :
            base(gdms)
        { }

        #endregion

        #region Methods

        protected internal abstract void UpdateBuildingDesigner();

        #endregion

        #region Properties

        public abstract bool IsSupported { get; }

        public abstract string UnitTypeName { get; }

        public BuildingDesigner BuildingDesigner =>
            GDModelScene.BuildingDesigner;

        #endregion
    }
}
