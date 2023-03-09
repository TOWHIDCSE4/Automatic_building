using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeB
{
    /// <summary>
    /// Converter that creates/loads/saves
    /// <see cref="TypeBUnitComponent"/> from/from/to JSON.
    /// </summary>
    [Serializable]
    public class TypeBUnitComponentJsonConverter :
        CatalogUnitComponentJsonConverter
    {
        #region Constructors

        public TypeBUnitComponentJsonConverter() : base()
        { }

        protected TypeBUnitComponentJsonConverter(
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

            var ucTypeB = new TypeBUnitComponent();

            LoadJson(ucTypeB, jObject);

            return ucTypeB;
        }

        protected internal override void LoadJson(
            UnitComponent uc, JObject jObject
        )
        {
            base.LoadJson(uc, jObject);

            if (!(uc is TypeBUnitComponent ucTypeB))
            {
                return;
            }

            // LayoutType

            string layoutTypeText =
                (string)jObject[nameof(TypeBUnitComponent.LayoutType)];

            if (!Enum.TryParse<TypeBUnitLayoutType>(
                layoutTypeText, out var layoutType
            ))
            {
                throw new ArgumentException(
                    "Invalid layout type",
                    nameof(jObject)
                );
            }

            ucTypeB.LayoutType = layoutType;

            // StaircaseAnchorPoint

            var staircaseAnchorPointJToken =
                jObject[nameof(TypeBUnitComponent.StaircaseAnchorPoint)];

            var staircaseAnchorPoint =
                CreatePointFromJson(staircaseAnchorPointJToken);

            ucTypeB.StaircaseAnchorPoint.Vector = staircaseAnchorPoint.Vector;

            // CorridorAnchorPoint

            var corridorAnchorPointJToken =
                jObject[nameof(TypeBUnitComponent.CorridorAnchorPoint)];

            var corridorAnchorPoint =
                CreatePointFromJson(corridorAnchorPointJToken);

            ucTypeB.CorridorAnchorPoint.Vector = corridorAnchorPoint.Vector;

            // BalconyAnchorPoint

            var balconyAnchorPointJToken =
                jObject[nameof(TypeBUnitComponent.BalconyAnchorPoint)];

            var balconyAnchorPoint =
                CreatePointFromJson(balconyAnchorPointJToken);

            ucTypeB.BalconyAnchorPoint.Vector = balconyAnchorPoint.Vector;

            // BalconyLength

            ucTypeB.BalconyLength =
                (double)jObject[nameof(TypeBUnitComponent.BalconyLength)];
        }

        protected internal override JObject SaveJToken(UnitComponent uc)
        {
            var jObject = base.SaveJToken(uc);

            if (!(uc is TypeBUnitComponent ucTypeB))
            {
                return jObject;
            }

            // LayoutType

            jObject[nameof(TypeBUnitComponent.LayoutType)] =
                ucTypeB.LayoutType.ToString();

            // StaircaseAnchorPoint

            jObject[nameof(TypeBUnitComponent.StaircaseAnchorPoint)] =
                SaveJToken(ucTypeB.StaircaseAnchorPoint);

            // CorridorAnchorPoint

            jObject[nameof(TypeBUnitComponent.CorridorAnchorPoint)] =
                SaveJToken(ucTypeB.CorridorAnchorPoint);

            // BalconyAnchorPoint

            jObject[nameof(TypeBUnitComponent.BalconyAnchorPoint)] =
                SaveJToken(ucTypeB.BalconyAnchorPoint);

            // BalconyLength

            jObject[nameof(TypeBUnitComponent.BalconyLength)] =
                ucTypeB.BalconyLength;

            return jObject;
        }

        #endregion
    }
}
