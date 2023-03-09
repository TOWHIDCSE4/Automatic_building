using System.Collections.Generic;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeA
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.TypeA
    /// .TypeAUnitComponent"/>
    /// presented as a unit catalog entry.
    /// </summary>
    public class TypeAUnitCatalogEntryViewModel : UnitCatalogEntryViewModel
    {
        #region Constructors

        public TypeAUnitCatalogEntryViewModel(TypeAUnitComponent uc) :
            base(uc)
        {
            TypeAUnitComponent = uc;
        }

        #endregion

        #region Properties

        public TypeAUnitComponent TypeAUnitComponent { get; }

        public string PositionType =>
            _positionTypeTextDict[TypeAUnitComponent.PositionType];

        public string EntranceType =>
            _entranceTypeTextDict[TypeAUnitComponent.EntranceType];

        #endregion

        #region Member variables

        private static readonly Dictionary<TypeAUnitPositionType, string>
            _positionTypeTextDict =
            new Dictionary<TypeAUnitPositionType, string>
            {
                { TypeAUnitPositionType.Basic, "Basic" },
                { TypeAUnitPositionType.End, "End" }
            };

        private static readonly Dictionary<TypeAUnitEntranceType, string>
            _entranceTypeTextDict =
            new Dictionary<TypeAUnitEntranceType, string>
            {
                { TypeAUnitEntranceType.North, "North" },
                { TypeAUnitEntranceType.South, "South" }
            };

        #endregion
    }
}
