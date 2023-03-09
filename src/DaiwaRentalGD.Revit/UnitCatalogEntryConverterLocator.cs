using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;

namespace DaiwaRentalGD.Revit
{
    /// <summary>
    /// Finds proper <see cref="UnitCatalogEntryConverter"/>s
    /// for given <see cref="Autodesk.Revit.DB.Document"/>s.
    /// </summary>
    public class UnitCatalogEntryConverterLocator
    {
        #region Constructors

        public UnitCatalogEntryConverterLocator() :
            this(new Dictionary<int, UnitCatalogEntryConverter>())
        { }

        public UnitCatalogEntryConverterLocator(
            IReadOnlyDictionary<int, UnitCatalogEntryConverter> converterDict
        )
        {
            if (converterDict is null)
            {
                throw new ArgumentNullException(nameof(converterDict));
            }

            ConverterDict =
                new ReadOnlyDictionary<int, UnitCatalogEntryConverter>(
                    _converterDict
                );

            foreach (var pair in converterDict)
            {
                _converterDict.Add(pair.Key, pair.Value);
            }
        }

        #endregion

        #region Methods

        public UnitCatalogEntryConverter GetConverter(
            string revitDocumentPath
        )
        {
            if (revitDocumentPath is null)
            {
                throw new ArgumentNullException(nameof(revitDocumentPath));
            }

            var baseEntryName = UnitCatalogEntryConverter.Default
                .GetBaseEntryName(revitDocumentPath);

            if (!_converterDict.TryGetValue(
                baseEntryName.MainType, out var converter
            ))
            {
                throw new InvalidOperationException(
                    $"No converter found for Main Type " +
                    $"{baseEntryName.MainType}"
                );
            }

            return converter;
        }

        #endregion

        #region Proeprties

        public static UnitCatalogEntryConverterLocator Default { get; } =
            new UnitCatalogEntryConverterLocator(
                new Dictionary<int, UnitCatalogEntryConverter>
                {
                    {
                        TypeAUnitComponent.MainType,
                        TypeAUnitCatalogEntryConverter.Default
                    },
                    {
                        TypeBUnitComponent.MainType,
                        TypeBUnitCatalogEntryConverter.Default
                    },
                    {
                        TypeCUnitComponent.MainType,
                        TypeCUnitCatalogEntryConverter.Default
                    },
                }
            );

        public IReadOnlyDictionary<int, UnitCatalogEntryConverter>
            ConverterDict
        { get; }

        #endregion

        #region Fields

        private readonly Dictionary<int, UnitCatalogEntryConverter>
            _converterDict = new Dictionary<int, UnitCatalogEntryConverter>();

        #endregion
    }
}
