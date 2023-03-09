using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeB
{
    /// <summary>
    /// View model for Type B
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.BuildingDesigner"/>.
    /// </summary>
    public class TypeBBuildingDesignerViewModel :
        BuildingDesignerViewModelBase
    {
        #region Constructors

        public TypeBBuildingDesignerViewModel(GDModelScene gdms) : base(gdms)
        {
            BuildingPlacementViewModel = new BuildingPlacementViewModel(gdms);
            UnitArrangerViewModel = new TypeBUnitArrangerViewModel(gdms);
        }

        #endregion

        #region Methods

        protected internal override void UpdateBuildingDesigner()
        {
            GDModelScene.BuildingDesigner = new TypeBBuildingDesigner();
        }

        #endregion

        #region Properties

        public override bool IsActivated
        {
            get => base.IsActivated;
            set
            {
                base.IsActivated = value;

                BuildingPlacementViewModel.IsActivated = value;
                UnitArrangerViewModel.IsActivated = value;
            }
        }

        public override string UnitTypeName { get; } = TypeBUnitTypeName;

        public override bool IsSupported =>
            GDModelScene.BuildingDesigner is TypeBBuildingDesigner;

        public BuildingPlacementViewModel BuildingPlacementViewModel
        { get; }

        public UnitArrangerViewModel UnitArrangerViewModel
        { get; }

        #endregion

        #region Constants

        public const string TypeBUnitTypeName = "Type B";

        #endregion
    }
}
