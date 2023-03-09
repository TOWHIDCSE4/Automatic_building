using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeA
{
    /// <summary>
    /// View model for Type A
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.BuildingDesigner"/>.
    /// </summary>
    public class TypeABuildingDesignerViewModel :
        BuildingDesignerViewModelBase
    {
        #region Constructors

        public TypeABuildingDesignerViewModel(GDModelScene gdms) : base(gdms)
        {
            BuildingPlacementViewModel = new BuildingPlacementViewModel(gdms);
            UnitArrangerViewModel = new TypeAUnitArrangerViewModel(gdms);
        }

        #endregion

        #region Methods

        protected internal override void UpdateBuildingDesigner()
        {
            GDModelScene.BuildingDesigner = new TypeABuildingDesigner();
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

        public override string UnitTypeName { get; } = TypeAUnitTypeName;

        public override bool IsSupported =>
            GDModelScene.BuildingDesigner is TypeABuildingDesigner;

        public BuildingPlacementViewModel BuildingPlacementViewModel
        { get; }

        public UnitArrangerViewModel UnitArrangerViewModel
        { get; }

        #endregion

        #region Constants

        public const string TypeAUnitTypeName = "Type A";

        #endregion
    }
}
