using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeC
{
    /// <summary>
    /// View model for Type C
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.BuildingDesigner"/>.
    /// </summary>
    public class TypeCBuildingDesignerViewModel :
        BuildingDesignerViewModelBase
    {
        #region Constructors

        public TypeCBuildingDesignerViewModel(GDModelScene gdms) : base(gdms)
        {
            BuildingPlacementViewModel = new BuildingPlacementViewModel(gdms);
            UnitArrangerViewModel = new TypeCUnitArrangerViewModel(gdms);
        }

        #endregion

        #region Methods

        protected internal override void UpdateBuildingDesigner()
        {
            GDModelScene.BuildingDesigner = new TypeCBuildingDesigner();
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

        public override string UnitTypeName { get; } = TypeCUnitTypeName;

        public override bool IsSupported =>
            GDModelScene.BuildingDesigner is TypeCBuildingDesigner;

        public BuildingPlacementViewModel BuildingPlacementViewModel
        { get; }

        public UnitArrangerViewModel UnitArrangerViewModel
        { get; }

        #endregion

        #region Constants

        public const string TypeCUnitTypeName = "Type C";

        #endregion
    }
}
