using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeA
{
    /// <summary>
    /// Converter that creates/loads/saves
    /// <see cref="TypeAUnitComponent"/> from/from/to JSON.
    /// </summary>
    [Serializable]
    public class TypeAUnitComponentJsonConverter :
        CatalogUnitComponentJsonConverter
    {
        #region Constructors

        public TypeAUnitComponentJsonConverter() : base()
        { }

        protected TypeAUnitComponentJsonConverter(
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

            var ucTypeA = new TypeAUnitComponent();

            LoadJson(ucTypeA, jObject);

            return ucTypeA;
        }

        protected internal override void LoadJson(
            UnitComponent uc, JObject jObject
        )
        {
            base.LoadJson(uc, jObject);

            if (!(uc is TypeAUnitComponent ucTypeA))
            {
                return;
            }

            // PositionType

            string positionTypeText =
                (string)jObject[nameof(TypeAUnitComponent.PositionType)];

            if (!Enum.TryParse<TypeAUnitPositionType>(
                positionTypeText, out var positionType
            ))
            {
                throw new ArgumentException(
                    "Invalid position type",
                    nameof(jObject)
                );
            }

            ucTypeA.PositionType = positionType;

            // EntranceType

            string entranceTypeText =
                (string)jObject[nameof(TypeAUnitComponent.EntranceType)];

            if (!Enum.TryParse<TypeAUnitEntranceType>(
                entranceTypeText, out var entranceType
            ))
            {
                throw new ArgumentException(
                    "Invalid entrance type",
                    nameof(jObject)
                );
            }

            ucTypeA.EntranceType = entranceType;

            // StaircaseAnchorPoint

            var staircaseAnchorPointJToken =
                jObject[nameof(TypeAUnitComponent.StaircaseAnchorPoint)];

            var staircaseAnchorPoint =
                CreatePointFromJson(staircaseAnchorPointJToken);

            ucTypeA.StaircaseAnchorPoint.Vector = staircaseAnchorPoint.Vector;

            // BalconyAnchorPoint

            var balconyAnchorPointJToken =
                jObject[nameof(TypeAUnitComponent.BalconyAnchorPoint)];

            var balconyAnchorPoint =
                CreatePointFromJson(balconyAnchorPointJToken);

            ucTypeA.BalconyAnchorPoint.Vector = balconyAnchorPoint.Vector;

            // BalconyLength

            ucTypeA.BalconyLength =
                (double)jObject[nameof(TypeAUnitComponent.BalconyLength)];
        }

        protected internal override JObject SaveJToken(UnitComponent uc)
        {
            var jObject = base.SaveJToken(uc);

            if (!(uc is TypeAUnitComponent ucTypeA))
            {
                return jObject;
            }

            // PositionType

            jObject[nameof(TypeAUnitComponent.PositionType)] =
                ucTypeA.PositionType.ToString();

            // EntranceType

            jObject[nameof(TypeAUnitComponent.EntranceType)] =
                ucTypeA.EntranceType.ToString();

            // StaircaseAnchorPoint

            jObject[nameof(TypeAUnitComponent.StaircaseAnchorPoint)] =
                SaveJToken(ucTypeA.StaircaseAnchorPoint);

            // BalconyAnchorPoint

            jObject[nameof(TypeAUnitComponent.BalconyAnchorPoint)] =
                SaveJToken(ucTypeA.BalconyAnchorPoint);

            // BalconyLength

            jObject[nameof(TypeAUnitComponent.BalconyLength)] =
                ucTypeA.BalconyLength;

            return jObject;
        }

        #endregion
    }
}
