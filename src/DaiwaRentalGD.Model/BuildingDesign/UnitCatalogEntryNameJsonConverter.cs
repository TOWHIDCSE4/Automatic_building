using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Converter that creates/loads/saves
    /// <see cref="UnitCatalogEntryName"/> from/from/to JSON.
    /// </summary>
    [Serializable]
    public class UnitCatalogEntryNameJsonConverter : ISerializable
    {
        #region Constructors

        public UnitCatalogEntryNameJsonConverter()
        { }

        protected UnitCatalogEntryNameJsonConverter(
            SerializationInfo info, StreamingContext context
        )
        { }

        #endregion

        #region Methods

        public virtual UnitCatalogEntryName CreateFromJson(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var jObject = JObject.Parse(json);

            return CreateFromJson(jObject);
        }

        internal virtual UnitCatalogEntryName CreateFromJson(JObject jObject)
        {
            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            var entryName = new UnitCatalogEntryName();

            LoadJson(entryName, jObject);

            return entryName;
        }

        public virtual void LoadJson(
            UnitCatalogEntryName entryName, string json
        )
        {
            if (entryName == null)
            {
                throw new ArgumentNullException(nameof(entryName));
            }

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var jObject = JObject.Parse(json);

            LoadJson(entryName, jObject);
        }

        protected internal virtual void LoadJson(
            UnitCatalogEntryName entryName, JObject jObject
        )
        {
            if (entryName == null)
            {
                throw new ArgumentNullException(nameof(entryName));
            }

            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            entryName.MainType =
                (int)jObject[nameof(UnitCatalogEntryName.MainType)];

            entryName.SizeXInP =
                (int)jObject[nameof(UnitCatalogEntryName.SizeXInP)];

            entryName.SizeYInP =
                (int)jObject[nameof(UnitCatalogEntryName.SizeYInP)];

            entryName.VariantType =
                (int)jObject[nameof(UnitCatalogEntryName.VariantType)];

            entryName.Index =
                (int)jObject[nameof(UnitCatalogEntryName.Index)];
        }

        public virtual string SaveJson(UnitCatalogEntryName entryName) =>
            SaveJToken(entryName).ToString();

        protected internal virtual JObject SaveJToken(
            UnitCatalogEntryName entryName
        )
        {
            var jObject = new JObject
            {
                [nameof(UnitCatalogEntryName.MainType)] =
                entryName.MainType,

                [nameof(UnitCatalogEntryName.SizeXInP)] =
                entryName.SizeXInP,

                [nameof(UnitCatalogEntryName.SizeYInP)] =
                entryName.SizeYInP,

                [nameof(UnitCatalogEntryName.VariantType)] =
                entryName.VariantType,

                [nameof(UnitCatalogEntryName.Index)] =
                entryName.Index
            };

            return jObject;
        }

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        { }

        #endregion
    }
}
