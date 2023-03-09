using System;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.CatalogUnitComponent"/>
    /// presented as a unit catalog entry.
    /// </summary>
    public class UnitCatalogEntryViewModel : ViewModelBase
    {
        #region Constructors

        public UnitCatalogEntryViewModel(CatalogUnitComponent cuc)
        {
            CatalogUnitComponent = cuc ??
                throw new ArgumentNullException(nameof(cuc));
        }

        #endregion

        #region Properties

        public CatalogUnitComponent CatalogUnitComponent { get; }

        public string FullName => CatalogUnitComponent.EntryName.FullName;

        public int MainType => CatalogUnitComponent.EntryName.MainType;

        public string SizeInP => string.Format(
            "{0:N1} x {1:N1}",
            CatalogUnitComponent.EntryName.SizeXInP,
            CatalogUnitComponent.EntryName.SizeYInP
        );

        public int VariantType => CatalogUnitComponent.EntryName.VariantType;

        public string PlanName => CatalogUnitComponent.PlanName;

        #endregion
    }
}
