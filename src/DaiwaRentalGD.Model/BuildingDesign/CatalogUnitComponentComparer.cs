using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Compares two CatalogUnitComponent instances to determine their
    /// order in a unit catalog.
    /// </summary>
    [Serializable]
    public class CatalogUnitComponentComparer :
        IComparer<CatalogUnitComponent>, ISerializable
    {
        #region Constructors

        public CatalogUnitComponentComparer()
        { }

        protected CatalogUnitComponentComparer(
            SerializationInfo info, StreamingContext context
        )
        { }

        #endregion

        #region Methods

        public int Compare(CatalogUnitComponent uc0, CatalogUnitComponent uc1)
        {
            if (uc0 == null && uc1 == null) { return 0; }

            if (uc0 == null && uc1 != null) { return -1; }

            if (uc0 != null && uc1 == null) { return 1; }

            int mainTypeComparison =
                uc0.EntryName.MainType
                .CompareTo(uc1.EntryName.MainType);

            if (mainTypeComparison != 0)
            { return mainTypeComparison; }

            int sizeYInPComparison =
                uc0.EntryName.SizeYInP
                .CompareTo(uc1.EntryName.SizeYInP);

            if (sizeYInPComparison != 0)
            { return sizeYInPComparison; }

            int sizeXInPComparison =
                uc0.EntryName.SizeXInP
                .CompareTo(uc1.EntryName.SizeXInP);

            if (sizeXInPComparison != 0)
            { return sizeXInPComparison; }

            int numOfBedroomsComparison =
                uc0.NumOfBedrooms
                .CompareTo(uc1.NumOfBedrooms);

            if (numOfBedroomsComparison != 0)
            { return numOfBedroomsComparison; }

            int variantTypeComparison =
                uc0.EntryName.VariantType
                .CompareTo(uc1.EntryName.VariantType);

            if (variantTypeComparison != 0)
            { return variantTypeComparison; }

            int indexComparison =
                uc0.EntryName.Index
                .CompareTo(uc1.EntryName.Index);

            if (indexComparison != 0)
            { return indexComparison; }

            return 0;
        }

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        { }

        #endregion
    }
}
