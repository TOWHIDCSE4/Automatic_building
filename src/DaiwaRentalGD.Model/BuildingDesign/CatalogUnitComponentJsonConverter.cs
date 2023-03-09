using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Converter that creates/loads/saves
    /// <see cref="CatalogUnitComponent"/> from/from/to JSON.
    /// </summary>
    [Serializable]
    public class CatalogUnitComponentJsonConverter :
        UnitComponentJsonConverter
    {
        #region Constructors

        public CatalogUnitComponentJsonConverter() : base()
        { }

        protected CatalogUnitComponentJsonConverter(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        protected internal override UnitComponent CreateFromJson(
            JObject jObject
        )
        {
            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            var cuc = new CatalogUnitComponent();

            LoadJson(cuc, jObject);

            return cuc;
        }

        protected internal override void LoadJson(
            UnitComponent uc, JObject jObject
        )
        {
            base.LoadJson(uc, jObject);

            if (!(uc is CatalogUnitComponent cuc))
            {
                return;
            }

            var entryNameJObject = (JObject)
               jObject[nameof(CatalogUnitComponent.EntryName)];

            var entryName =
                EntryNameJsonConverter.CreateFromJson(entryNameJObject);

            cuc.EntryName = entryName;
        }

        protected internal override JObject SaveJToken(UnitComponent uc)
        {
            var jObject = base.SaveJToken(uc);

            if (!(uc is CatalogUnitComponent cuc))
            {
                return jObject;
            }

            jObject[nameof(CatalogUnitComponent.EntryName)] =
                EntryNameJsonConverter.SaveJToken(cuc.EntryName);

            return jObject;
        }

        #endregion

        #region Properties

        private UnitCatalogEntryNameJsonConverter EntryNameJsonConverter
        { get; } = new UnitCatalogEntryNameJsonConverter();

        #endregion
    }
}
