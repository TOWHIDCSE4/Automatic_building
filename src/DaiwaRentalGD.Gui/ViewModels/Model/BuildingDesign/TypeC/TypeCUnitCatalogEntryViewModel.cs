using System.Collections.Generic;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeC
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.TypeC
    /// .TypeCUnitComponent"/>
    /// presented as a unit catalog entry.
    /// </summary>
    public class TypeCUnitCatalogEntryViewModel : UnitCatalogEntryViewModel
    {
        #region Constructors

        public TypeCUnitCatalogEntryViewModel(TypeCUnitComponent uc) :
            base(uc)
        {
            TypeCUnitComponent = uc;
        }

        #endregion

        #region Properties

        public TypeCUnitComponent TypeCUnitComponent { get; }

        public string PositionType =>
            _positionTypeTextDict[TypeCUnitComponent.PositionType];

        public string EntranceType =>
            _entranceTypeTextDict[TypeCUnitComponent.EntranceType];

        #endregion

        #region Member variables

        private static readonly Dictionary<TypeCUnitPositionType, string>
            _positionTypeTextDict =
            new Dictionary<TypeCUnitPositionType, string>
            {
                { TypeCUnitPositionType.FirstFloor, "1st Floor" },
                { TypeCUnitPositionType.SecondFloor, "2nd Floor" }
            };

        private static readonly Dictionary<TypeCUnitEntranceType, string>
            _entranceTypeTextDict =
            new Dictionary<TypeCUnitEntranceType, string>
            {
                { TypeCUnitEntranceType.North, "North" },
                { TypeCUnitEntranceType.South, "South" }
            };

        #endregion
    }
}
