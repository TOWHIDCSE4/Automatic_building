using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;
using Newtonsoft.Json.Linq;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Converter that creates/loads/saves
    /// <see cref="UnitCatalogComponent"/> from/from/to JSON.
    /// </summary>
    [Serializable]
    public class UnitCatalogComponentJsonConverter : ISerializable
    {
        #region Constructors

        public UnitCatalogComponentJsonConverter()
        {
            UnitComponentJsonConverterDict =
                new ReadOnlyDictionary
                <int, CatalogUnitComponentJsonConverter>(
                    _unitComponentJsonConverterDict
                );
        }

        protected UnitCatalogComponentJsonConverter(
            SerializationInfo info, StreamingContext context
        )
        {
            UnitComponentJsonConverterDict =
                new ReadOnlyDictionary
                <int, CatalogUnitComponentJsonConverter>(
                    _unitComponentJsonConverterDict
                );

            EntryNameJsonConverter =
                info.GetValue<UnitCatalogEntryNameJsonConverter>(
                    nameof(EntryNameJsonConverter)
                );
        }

        #endregion

        #region Methods

        public virtual void LoadJson(UnitCatalogComponent ucc, string json)
        {
            if (ucc == null)
            {
                throw new ArgumentNullException(nameof(ucc));
            }

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var jObject = JObject.Parse(json);

            LoadJson(ucc, jObject);
        }

        internal virtual void LoadJson(
            UnitCatalogComponent ucc, JObject jObject
        )
        {
            if (ucc == null)
            {
                throw new ArgumentNullException(nameof(ucc));
            }

            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            ucc.Clear();

            var entryJArray =
                (JArray)jObject[nameof(UnitCatalogComponent.Entries)];

            foreach (var entryJObject in entryJArray.Cast<JObject>())
            {
                var entryNameJObject = (JObject)
                    entryJObject[nameof(CatalogUnitComponent.EntryName)];

                var entryName =
                    EntryNameJsonConverter.CreateFromJson(entryNameJObject);

                var unitComponentJsonConverter =
                    UnitComponentJsonConverterDict[entryName.MainType];

                CatalogUnitComponent cuc =
                    unitComponentJsonConverter.CreateFromJson(entryJObject)
                    as CatalogUnitComponent;

                ucc.AddEntry(cuc);
            }
        }

        public virtual string SaveJson(UnitCatalogComponent ucc) =>
            SaveJToken(ucc).ToString();

        internal virtual JObject SaveJToken(UnitCatalogComponent ucc)
        {
            var jObject = new JObject();

            var entryJTokens = ucc.Entries.Select(
                entry =>
                UnitComponentJsonConverterDict[entry.EntryName.MainType]
                .SaveJToken(entry)
            );

            var entryJArray = new JArray(entryJTokens);

            jObject[nameof(UnitCatalogComponent.Entries)] = entryJArray;

            return jObject;
        }

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(
                nameof(EntryNameJsonConverter), EntryNameJsonConverter
            );
        }

        #endregion

        #region Properties

        private UnitCatalogEntryNameJsonConverter EntryNameJsonConverter
        { get; } = new UnitCatalogEntryNameJsonConverter();

        public IReadOnlyDictionary<int, CatalogUnitComponentJsonConverter>
            UnitComponentJsonConverterDict
        { get; }

        #endregion

        #region Member variables

        private readonly Dictionary<int, CatalogUnitComponentJsonConverter>
            _unitComponentJsonConverterDict =
            new Dictionary<int, CatalogUnitComponentJsonConverter>
            {
                {
                    TypeAUnitComponent.MainType,
                    new TypeAUnitComponentJsonConverter()
                },
                {
                    TypeBUnitComponent.MainType,
                    new TypeBUnitComponentJsonConverter()
                },
                {
                    TypeCUnitComponent.MainType,
                    new TypeCUnitComponentJsonConverter()
                }
            };

        #endregion
    }
}
