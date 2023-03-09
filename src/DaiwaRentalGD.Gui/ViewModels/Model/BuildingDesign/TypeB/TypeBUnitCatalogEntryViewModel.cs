using System.Collections.Generic;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeB
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.TypeB
    /// .TypeBUnitComponent"/>
    /// presented as a unit catalog entry.
    /// </summary>
    public class TypeBUnitCatalogEntryViewModel : UnitCatalogEntryViewModel
    {
        #region Constructors

        public TypeBUnitCatalogEntryViewModel(TypeBUnitComponent uc) :
            base(uc)
        {
            TypeBUnitComponent = uc;
        }

        #endregion

        #region Properties

        public TypeBUnitComponent TypeBUnitComponent { get; }

        public string LayoutType =>
            _layoutTypeTextDict[TypeBUnitComponent.LayoutType];

        #endregion

        #region Member variables

        private static Dictionary<TypeBUnitLayoutType, string>
            _layoutTypeTextDict =
            new Dictionary<TypeBUnitLayoutType, string>
            {
                { TypeBUnitLayoutType.Basic, "Basic" },
                { TypeBUnitLayoutType.Stair, "Stair" }
            };

        #endregion
    }
}
